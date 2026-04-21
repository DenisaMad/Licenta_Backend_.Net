using Backend.DataAbstraction.Security;
using Backend.Domain.User;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Backend.DataAbstraction.Database;
using Backend.DataAbstraction.BearerTokens;

namespace Backend.Middleware
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthTokensService authTokensService, IMongoDataBase mongoDataBase)
        {
            // Only perform checks if the standard JwtBearer authentication succeeded
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader != null && authHeader.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();

                    var userIdStr = context.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

                    if (!string.IsNullOrEmpty(userIdStr))
                    {
                        // Check if it's blacklisted in GeneratedTokens list/table
                        bool isBlacklisted = await authTokensService.CheckBlacklistedToken(token, TokenType.ACCESS_TOKEN, userIdStr);
                        if (isBlacklisted)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync("Token is blacklisted.");
                            return;
                        }

                        // Verify it matches the active AccessToken in User table
                        var userCollection = mongoDataBase.GetCollection<User>();
                        var user = await userCollection.Find(Builders<User>.Filter.Eq(u => u.Id, userIdStr)).FirstOrDefaultAsync();

                        if (user == null || user.AccessToken != token)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync("Token is no longer active.");
                            return;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Invalid token structure (missing userId).");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
