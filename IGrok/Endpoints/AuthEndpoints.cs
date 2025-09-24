using IGrok.DTOs;
using IGrok.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace IGrok.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var publicGroup = app.MapGroup("/api/v1/auth")
            .WithTags("Auth")
            .RequireRateLimiting("sliding-by-ip");

        publicGroup.MapPost("/login", async Task<Results<Ok<LoginResponse>, NotFound, ForbidHttpResult, ValidationProblem>>
            (LoginRequest request, IUserService userService) =>
        {
            var response = await userService.AuthenticateAsync(request.Key, request.Hwid);
            return TypedResults.Ok(response);
        })
        .Produces<LoginResponse>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
        .ProducesValidationProblem()
        .WithSummary("Authenticates a user and returns JWT tokens.");

        publicGroup.MapPost("/refresh", async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult>>
            (RefreshTokenRequest request, IUserService userService) =>
        {
            var response = await userService.RefreshTokenAsync(request);
            return TypedResults.Ok(response);
        })
        .Produces<LoginResponse>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
        .WithSummary("Refreshes an access token using a refresh token.");
    }
}