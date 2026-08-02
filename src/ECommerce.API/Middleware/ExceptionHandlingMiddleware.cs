using System.Net;
using System.Text.Json;
using ECommerce.Application.Common.Exceptions;
using ECommerce.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Middleware;

/// <summary>
/// Catches unhandled exceptions and converts them into a consistent
/// RFC 7807 ProblemDetails response with the appropriate HTTP status code.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problem = MapToProblemDetails(context, exception);

        if (problem.Status >= 500)
            _logger.LogError(exception, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);
        else
            _logger.LogWarning("{StatusCode} processing {Method} {Path}: {Message}", problem.Status, context.Request.Method, context.Request.Path, exception.Message);

        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, problem.GetType()));
    }

    private static ProblemDetails MapToProblemDetails(HttpContext context, Exception exception) => exception switch
    {
        ValidationException validationException => BuildValidationProblem(context, validationException),

        NotFoundException notFoundException => BuildProblem(
            context, StatusCodes.Status404NotFound, "Resource not found.", notFoundException.Message,
            "https://tools.ietf.org/html/rfc7231#section-6.5.4"),

        AuthenticationException authenticationException => BuildProblem(
            context, StatusCodes.Status401Unauthorized, "Authentication failed.", authenticationException.Message,
            "https://tools.ietf.org/html/rfc7235#section-3.1"),

        DomainException domainException => BuildProblem(
            context, StatusCodes.Status400BadRequest, "Business rule violation.", domainException.Message,
            "https://tools.ietf.org/html/rfc7231#section-6.5.1"),

        // More specific than DbUpdateException; must be matched first.
        DbUpdateConcurrencyException => BuildProblem(
            context, StatusCodes.Status409Conflict, "Concurrency conflict.",
            "The resource was modified by another request. Reload and try again.",
            "https://tools.ietf.org/html/rfc7231#section-6.5.8"),

        DbUpdateException => BuildProblem(
            context, StatusCodes.Status409Conflict, "Conflict with current resource state.",
            "The request could not be completed because it conflicts with the current state of the resource.",
            "https://tools.ietf.org/html/rfc7231#section-6.5.8"),

        _ => BuildProblem(
            context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.",
            "An unexpected error occurred. Please try again later.",
            "https://tools.ietf.org/html/rfc7231#section-6.6.1"),
    };

    private static ValidationProblemDetails BuildValidationProblem(HttpContext context, ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Instance = context.Request.Path,
        };
    }

    private static ProblemDetails BuildProblem(HttpContext context, int statusCode, string title, string detail, string type) =>
        new()
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = type,
            Instance = context.Request.Path,
        };
}
