using Microsoft.EntityFrameworkCore;
using AuthService.API.Data;

namespace AuthService.API.Middleware
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuthDbContext _dbContext;

        public TokenBlacklistMiddleware(RequestDelegate next, AuthDbContext dbContext)
        {
            _next = next;
            _dbContext = dbContext;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                var isBlacklisted = await _dbContext.TokenBlackLists.AnyAsync(t => t.Token == token);
                if (isBlacklisted)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token is blacklisted.");
                    return;
                }
            }

            await _next(context);
        }
    }
}