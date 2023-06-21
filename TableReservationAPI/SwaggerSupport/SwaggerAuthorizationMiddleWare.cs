using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TableReservationAPI.SwaggerSupport
{

    public class SwaggerAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Func<HttpContext, Func<Task>, Task> _getTokenFunc;

        public SwaggerAuthorizationMiddleware(RequestDelegate next, Func<HttpContext, Func<Task>, Task> getTokenFunc)
        {
            _next = next;
            _getTokenFunc = getTokenFunc;
        }

        public async Task Invoke(HttpContext context)
        {
            await _getTokenFunc(context, () => _next(context));
        }
    }

    public static class SwaggerAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseSwaggerAuthorization(this IApplicationBuilder app, Func<HttpContext, Func<Task>, Task> getTokenFunc)
        {
            return app.UseMiddleware<SwaggerAuthorizationMiddleware>(getTokenFunc);
        }
    }
}
