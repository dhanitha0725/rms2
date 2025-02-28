using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using Domain.Common;
using FluentValidation;

namespace WebAPI.Middlewares
{
    public class GlobalExceptionHandlingMiddleware (Serilog.ILogger logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleUnknownExceptionAsync(context, ex);
            }
        }

        // handle validation exceptions
        private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
        {
            logger.Warning (ex, "Validation exception occurred.");

            var validationErrors = ex.Errors.Select(error => new
            {
                Field = error.PropertyName,
                Message = error.ErrorMessage
            });

            var error = new Error( "One or more validation errors occurred.");
            var result = Result.Failure(error);

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = new
            {
                result.IsSuccess,
                result.Error.Message,
                Errors = validationErrors
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }

        // handle unknown exceptions
        private async Task HandleUnknownExceptionAsync(HttpContext context, Exception ex)
        {
            logger.Error(ex, "An unexpected error occurred.");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Type = "Server Error",
                Title = "Server Error",
                Detail = ex.Message
            };

            var result = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(result);
        }
    }
}
