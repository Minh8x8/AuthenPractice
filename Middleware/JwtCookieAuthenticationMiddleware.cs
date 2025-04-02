using AuthenPractice.Services;

namespace AuthenPractice.Middleware
{
    public class JwtCookieAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenService _tokenService;

        public JwtCookieAuthenticationMiddleware(RequestDelegate next, TokenService tokenService)
        {
            _next = next;
            _tokenService = tokenService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Cookies["jwt"];

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var claimsPrincipal = _tokenService.ValidateToken(token);
                    if (claimsPrincipal != null)
                    {
                        context.User = claimsPrincipal;
                    }
                }
                catch (Exception)
                {
                    // Token validation failed, you might want to log this
                    // Optionally clear the invalid cookie:
                    // context.Response.Cookies.Delete("jwt");
                }
            }

            await _next(context);
        }
    }

    // Extension method to easily add the middleware to the pipeline
    public static class JwtCookieAuthenticationExtensions
    {
        public static IApplicationBuilder UseJwtCookieAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtCookieAuthenticationMiddleware>();
        }
    }
}
