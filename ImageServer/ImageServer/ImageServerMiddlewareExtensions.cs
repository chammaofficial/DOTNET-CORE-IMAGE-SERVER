using Microsoft.AspNetCore.Builder;

namespace ImageServer
{
    public static class ImageServerMiddlewareExtensions
    {
        public static IApplicationBuilder UseImageServerMiddleware(this IApplicationBuilder app, string rootPath, int CacheControlMaxTime)
        {
            return app.UseMiddleware<ImageServerMiddleware>(rootPath,CacheControlMaxTime);
        }
    }
}
