// © 2020 Partners HealthCare Technology & Innovation Department
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CovidPass.Common.Extensions;
using Microsoft.Extensions.Logging;
using CovidPass.Common.Models;
using System.Text.Json;

namespace CovidPass.Back.Middleware
{
    public class ExceptionConverterMiddleware
    {
        private readonly ILogger<ExceptionConverterMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly JsonSerializerOptions _jsonSettings = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ExceptionConverterMiddleware(RequestDelegate next, ILogger<ExceptionConverterMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                _logger.LogError("Unhandled exception" + e.ToString());
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var jsonString = JsonSerializer.Serialize(new CommonResponse
                    {
                        IsError = true,
                        Message = e.ToString()
                    }, _jsonSettings);
                    await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                }
            }
        }
    }
}
