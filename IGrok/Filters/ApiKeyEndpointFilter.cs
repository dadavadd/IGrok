using IGrok.Configurations;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace IGrok.Filters;

public class ApiKeyEndpointFilter(IOptions<AdminSettings> adminSettings) : IEndpointFilter
{
	private const string ApiKeyHeaderName = "X-Api-Key";

	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
		{
			return TypedResults.Unauthorized();
		}

		if (!SecureEquals(extractedApiKey.ToString(), adminSettings.Value.ApiKey))
		{ 
			return TypedResults.Unauthorized(); 
		}

		return await next(context);
	}

	private static bool SecureEquals(string a, string b)
	{
		var aBytes = Encoding.UTF8.GetBytes(a);
		var bBytes = Encoding.UTF8.GetBytes(b);

		if (aBytes.Length != bBytes.Length)
			return false;

		return CryptographicOperations.FixedTimeEquals(aBytes, bBytes);
	}
}