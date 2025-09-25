using IGrok.DTOs;
using IGrok.DTOs.Admin;
using IGrok.Filters;
using IGrok.Models;
using IGrok.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace IGrok.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var adminGroup = app.MapGroup("/api/v1/admin")
            .WithTags("Admin")
            .AddEndpointFilter<ApiKeyEndpointFilter>();

        adminGroup.MapGet("/users", async Task<Results<Ok<List<User>>, BadRequest<ProblemDetails>>> 
            (IUserService userService, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        {
            var users = await userService.GetUsersAsync(page, pageSize);
            return TypedResults.Ok(users);
        })
        .Produces<List<User>>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .WithSummary("Returns paginated list of users.");

        adminGroup.MapPost("/users", async Task<Results<Created<ValidationResponse>, Conflict<ProblemDetails>, ValidationProblem>>
            (CreateUserRequest request, IUserService userService) =>
        {
            var createdUser = await userService.CreateUserAsync(request.Key, request.SubscriptionDays);
            return TypedResults.Created((string?)null, createdUser);
        })
        .Produces<ValidationResponse>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
        .ProducesValidationProblem()
        .WithSummary("Creates a new user license key.");

        adminGroup.MapPut("/users/{key}/hwid", async Task<Results<NoContent, NotFound, ValidationProblem>>
            (string key, HwidUpdateRequest request, IUserService userService) =>
        {
            await userService.UpdateUserHwidAsync(key, request.NewHwid);
            return TypedResults.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .ProducesValidationProblem()
        .WithSummary("Updates or resets a user's HWID.");

        adminGroup.MapPatch("/users/{key}/status", async Task<Results<NoContent, NotFound>>
            (string key, [FromQuery] bool isActive, IUserService userService) =>
        {
            await userService.SetUserActivationStatusAsync(key, isActive);
            return TypedResults.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .WithSummary("Activates or deactivates a user account.");

        adminGroup.MapDelete("/users/{key}", async Task<NoContent>
            (string key, IUserService userService) =>
        {
            await userService.DeleteUserAsync(key);
            return TypedResults.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .WithSummary("Permanently deletes a user. This action cannot be undone.");

    }
}
