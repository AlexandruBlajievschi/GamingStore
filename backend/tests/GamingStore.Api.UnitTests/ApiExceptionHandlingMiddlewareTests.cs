using System.Net;
using System.Text.Json;
using GamingStore.Api.Middleware;
using GamingStore.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GamingStore.Api.UnitTests;

public sealed class ApiExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WritesBadRequestProblemDetails_WhenDomainValidationFails()
    {
        var middleware = new ApiExceptionHandlingMiddleware(_ =>
            throw new DomainValidationException("Invalid domain state."));
        var context = CreateHttpContext();

        await middleware.InvokeAsync(context);

        var problemDetails = await ReadProblemDetailsAsync(context);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        Assert.Equal("Domain validation failed.", problemDetails.Title);
        Assert.Equal("Invalid domain state.", problemDetails.Detail);
    }

    [Fact]
    public async Task InvokeAsync_WritesNotFoundProblemDetails_WhenResourceIsMissing()
    {
        var middleware = new ApiExceptionHandlingMiddleware(_ =>
            throw new ResourceNotFoundException("Missing resource."));
        var context = CreateHttpContext();

        await middleware.InvokeAsync(context);

        var problemDetails = await ReadProblemDetailsAsync(context);
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
        Assert.Equal("Resource was not found.", problemDetails.Title);
        Assert.Equal("Missing resource.", problemDetails.Detail);
    }

    [Fact]
    public async Task InvokeAsync_WritesUnauthorizedProblemDetails_WhenAuthenticationFails()
    {
        var middleware = new ApiExceptionHandlingMiddleware(_ =>
            throw new AuthenticationFailedException("Invalid email or password."));
        var context = CreateHttpContext();

        await middleware.InvokeAsync(context);

        var problemDetails = await ReadProblemDetailsAsync(context);
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.Equal("Authentication failed.", problemDetails.Title);
        Assert.Equal("Invalid email or password.", problemDetails.Detail);
    }

    [Fact]
    public async Task InvokeAsync_AllowsSuccessfulRequestToContinue()
    {
        var middleware = new ApiExceptionHandlingMiddleware(context =>
        {
            context.Response.StatusCode = StatusCodes.Status204NoContent;

            return Task.CompletedTask;
        });
        var context = CreateHttpContext();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status204NoContent, context.Response.StatusCode);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        return context;
    }

    private static async Task<ProblemDetails> ReadProblemDetailsAsync(HttpContext context)
    {
        context.Response.Body.Position = 0;

        var problemDetails = await JsonSerializer.DeserializeAsync<ProblemDetails>(
            context.Response.Body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        return problemDetails ?? throw new InvalidOperationException("ProblemDetails could not be read.");
    }
}
