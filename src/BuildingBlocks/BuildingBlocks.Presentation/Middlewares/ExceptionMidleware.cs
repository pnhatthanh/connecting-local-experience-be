using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Presentation.Results;
using FluentValidation;
using BuildingBlocks.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Presentation.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(exception, "An error occurred: {ErrorMessage}", exception.Message);
            
            context.Response.ContentType = "application/json";
            var errorResponse = new ErrorResponse();

            switch (exception)
            {
                case FluentValidationException fluentValidationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = "Validation failed";
                    errorResponse.Errors = fluentValidationException.Errors.ToDictionary(
                        error => error.Key, 
                        error => error.Value.ToList());
                    break;
                case BaseException customException:
                    context.Response.StatusCode = (int)customException.StatusCode;
                    errorResponse.StatusCode = (int)customException.StatusCode;
                    errorResponse.Message = customException.Message;
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "An unexpected error occurred.";
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
