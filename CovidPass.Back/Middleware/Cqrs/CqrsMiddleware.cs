// © 2020 Partners HealthCare Technology & Innovation Department
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CovidPass.Common.Extensions;
using CovidPass.Common.Models;
using CovidPass.Core.Infrastructure.Models;

namespace CovidPass.Back.Middleware.Cqrs
{
    public class CqrsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerOptions _jsonSettings = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public CqrsMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                if (e is BadRequestException badRequest)
                {
                    context.Response.StatusCode = 400;
                    context.Response.ContentType = "application/json";
                    var jsonString = JsonSerializer.Serialize(new CommonResponse
                    {
                        IsError = true,
                        Message = e.GetFullExceptionMessage()
                    }, _jsonSettings);
                    await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                }
                else if (e is NotFoundException notFound)
                {
                    context.Response.StatusCode = 404;
                }
                else throw;
            }
        }
    }
}
