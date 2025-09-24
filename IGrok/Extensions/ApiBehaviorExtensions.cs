using System.Text.Json.Serialization;

namespace IGrok.Extensions;

public static class ApiBehaviorExtensions
{
    public static IServiceCollection AddApiBehavior(this IServiceCollection services)
    {
        services.AddValidation();
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
                if (ctx.ProblemDetails is HttpValidationProblemDetails validation)
                {
                    ctx.ProblemDetails.Detail =
                        $"Validation failed: {validation.Errors.Values.Sum(v => v.Length)} error(s)";
                }

                ctx.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
            };
        });

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }
}
