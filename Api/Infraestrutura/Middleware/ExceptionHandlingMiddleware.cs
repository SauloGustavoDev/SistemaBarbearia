using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Api.Infraestrutura.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        public readonly RequestDelegate _next;
        public readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // passa para o próximo middleware
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var result = new
                {
                    status = context.Response.StatusCode,
                    message = ex.Message
                };

                await context.Response.WriteAsJsonAsync(result);
            }
        }
    }
}
