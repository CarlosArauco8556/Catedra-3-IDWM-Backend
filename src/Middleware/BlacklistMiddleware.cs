using Catedra3IDWMBackend.src.Interfaces;

namespace Catedra3IDWMBackend.src.Middleware
{
    public class BlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public BlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

                public async Task InvokeAsync(HttpContext context)
        {
            var tokenService = context.RequestServices.GetService<ITokenService>();

            if (tokenService == null)
            {
                await _next(context);
                return;
            }

            if (context.Request.Headers.TryGetValue("Authorization", out var token))
            {
                var tokenValue = token.ToString().Replace("Bearer ", string.Empty);

                if (await tokenService.IsTokenBlacklistedAsync(tokenValue))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Token en la lista negra o invalido");
                    return;
                }
            }

            await _next(context);
        }
    }
}