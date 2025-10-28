using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Presentation.Results;
using FluentValidation;
using BuildingBlocks.Application.Exceptions;

namespace BuildingBlocks.Presentation.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
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
            var errorResponse = new ErrorResponse();

            switch (exception)
            {
                case FluentValidationException fluentValidationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = "Validation failed";
                    errorResponse.Errors = fluentValidationException.Errors.ToDictionary(
                        error => error.Key, 
                        error => error.Value.ToList<string>());
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
