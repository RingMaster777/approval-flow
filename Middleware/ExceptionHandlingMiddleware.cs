using ApprovalFlow.Application.Common.Exceptions;
using FluentValidation;
using System.Text.Json;

namespace ApprovalFlow.Middleware;

public class ExceptionHandlingMiddleware
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
        context.Response.ContentType = "application/json";

        object response = exception switch
        {
            NotFoundException notFoundEx => new
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = notFoundEx.Message
            },
            BusinessRuleException businessEx => new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = businessEx.Message
            },
            ValidationException validationEx => new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Validation failed",
                Errors = validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            },
            _ => new
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An unexpected error occurred"
            }
        };

        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var statusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            BusinessRuleException => StatusCodes.Status400BadRequest,
            ValidationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
