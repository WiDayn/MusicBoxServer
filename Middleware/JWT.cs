using Microsoft.IdentityModel.Tokens;
using MusicBoxServer.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace MusicBoxServer.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ApiResponseController response = new();

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var _secret = _configuration["JwtSettings:Secret"];
            if (context.Request.Path.StartsWithSegments("/user/login") || context.Request.Path.StartsWithSegments("/user/register") || context.Request.Path.StartsWithSegments("/external"))
            {
                await _next(context);
            }
            else
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (token != null && AttachUserIdToContext(context, token, _secret))
                {
                    await _next(context);
                }
                else
                {
                    // 令牌不存在或令牌无效，构造统一的 API 响应
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var apiResponse = new ApiResponse<object>(StatusCodes.Status401Unauthorized, null, "Unauthorized");
                    var jsonResponse = JsonSerializer.Serialize(apiResponse);

                    await context.Response.WriteAsync(jsonResponse);
                    return; // 提前结束请求
                }
            }
        }

        private bool AttachUserIdToContext(HttpContext context, string token, string _secret)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "UserID").Value;

                // Attach the user ID to the context on successful jwt validation
                context.Items["UserId"] = userId;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
