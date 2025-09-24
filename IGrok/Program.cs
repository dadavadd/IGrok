using IGrok.Configurations;
using IGrok.Endpoints;
using IGrok.Extensions;
using IGrok.Handlers;
using IGrok.Services;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<AdminSettings>()
    .BindConfiguration(typeof(AdminSettings).Name)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<JwtOptions>()
    .BindConfiguration(typeof(JwtOptions).Name)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddPersistence(builder.Configuration)
    .AddRateLimiting()
    .AddApiBehavior()
    .AddAuthentificationAndAuthorization()
    .AddSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(new() { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });

app.UseExceptionHandler();
app.UseRateLimiter();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapAdminEndpoints();

app.Run();