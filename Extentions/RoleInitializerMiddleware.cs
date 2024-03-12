using Microsoft.AspNetCore.Identity;

namespace MindScribe.Extentions
{
    public class RoleInitializerMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleInitializerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, RoleManager<IdentityRole> roleManager)
        {
            // Создание ролей, если они отсутствуют
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("Moderator"))
            {
                await roleManager.CreateAsync(new IdentityRole("Moderator"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            await _next(context);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RoleInitializerMiddlewareExtensions
    {
        public static IApplicationBuilder UseRoleInitializerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RoleInitializerMiddleware>();
        }
    }
}
