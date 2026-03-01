using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using TodoApi.DTOs.ResponseDTOs;

namespace TodoApi.Middlewares.ExceptionMiddleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;
            _env = env;
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
            var statusCode = exception switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            var problem = new ProblemResponse
            {
                StatusCode = statusCode,
                Title = GetTitle(statusCode),
                Instance = context.Request.Path,
                Method = context.Request.Method
            };

            // Only show details in Development
            if (_env.IsDevelopment())
            {
                problem.Message = exception.Message;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var jsonResponse = JsonSerializer.Serialize(problem);
            await context.Response.WriteAsync(jsonResponse);
        }

        private static string GetTitle(int statusCode) => statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Resource Not Found",
            500 => "Internal Server Error",
            _ => "An error occurred"
        };
    }
}
