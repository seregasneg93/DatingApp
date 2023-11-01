using DatingApp.Errors;
using System.Net;
using System.Text.Json;

namespace DatingApp.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        // RequestDelegate делегат ошибки IHostEnvironment чтобы понимать в какой стадии разработки находимся, дебаг или релиз
        public ExceptionMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            { 
                // берем контекст и отлавливем ошибку
                await _requestDelegate(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message); // чтобы в терминале была ошибка видна
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                var responce = _env.IsDevelopment() ? new ApiException(httpContext.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                                                    : new ApiException(httpContext.Response.StatusCode,"Внутренняя ошибка сервера");

                // гарания того что ответ будет отправлет в формате json camelCase
                var options = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(responce, options);
                await httpContext.Response.WriteAsync(json);
            }
        }
    }
}
