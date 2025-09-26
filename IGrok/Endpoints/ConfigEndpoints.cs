using IGrok.DTOs.Configs;
using IGrok.DTOs.Shared;
using IGrok.Models;
using IGrok.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace IGrok.Endpoints;

public static class ConfigEndpoints
{
    public static void MapConfigEndpoints(this IEndpointRouteBuilder app)
    {
        var configsGroup = app.MapGroup("/api/v1/configs")
            .WithTags("Configs")
            .RequireAuthorization();

        configsGroup.MapGet("/", async Task<Results<Ok<PaginatedResponse<Config>>, UnauthorizedHttpResult>> 
            (IConfigService configService, ClaimsPrincipal user, int pageNumber = 1, int pageSize = 10) =>
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return TypedResults.Unauthorized();
            }

            var userId = int.Parse(userIdClaim);
            var configs = await configService.GetConfigsByUserIdAsync(userId, pageNumber, pageSize);
            return TypedResults.Ok(configs);
        })
        .Produces<PaginatedResponse<Config>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        configsGroup.MapGet("/{id}", async Task<Results<Ok<Config>, NotFound, ForbidHttpResult, UnauthorizedHttpResult>>
            (int id, IConfigService configService, ClaimsPrincipal user) =>
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return TypedResults.Unauthorized();
            }

            var userId = int.Parse(userIdClaim);
            var config = await configService.GetConfigByIdAsync(id, userId);
            
            return TypedResults.Ok(config);
        })
        .Produces<Config>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status401Unauthorized);

        configsGroup.MapPost("/", async Task<Results<Created<Config>, UnauthorizedHttpResult>>
            (CreateConfigRequest request, IConfigService configService, ClaimsPrincipal user) =>
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return TypedResults.Unauthorized();
            }

            var userId = int.Parse(userIdClaim);
            var config = await configService.CreateConfigAsync(userId, request.Name, request.JsonContent);
            return TypedResults.Created($"/api/v1/configs/{config.Id}", config);
        })
        .Produces<Config>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status401Unauthorized);

        configsGroup.MapPut("/{id}", async Task<Results<NoContent, NotFound, ForbidHttpResult, UnauthorizedHttpResult>> 
            (int id, UpdateConfigRequest request, IConfigService configService, ClaimsPrincipal user) =>
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return TypedResults.Unauthorized();
            }

            var userId = int.Parse(userIdClaim);
            await configService.UpdateConfigAsync(id, userId, request.Name, request.JsonContent);
            return TypedResults.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status401Unauthorized);

        configsGroup.MapDelete("/{id}", async Task<Results<NoContent, NotFound, ForbidHttpResult, UnauthorizedHttpResult>> 
            (int id, IConfigService configService, ClaimsPrincipal user) =>
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return TypedResults.Unauthorized();
            }

            var userId = int.Parse(userIdClaim);
            await configService.DeleteConfigAsync(id, userId);
            return TypedResults.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}