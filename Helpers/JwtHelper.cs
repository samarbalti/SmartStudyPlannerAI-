using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SmartStudyPlannerAI.Helpers;

public static class JwtHelper
{
    public static int? GetUserIdFromToken(HttpContext context)
    {
        var token = context.Request.Cookies["AuthToken"] ?? 
                    context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token)) return null;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        }
        catch
        {
            return null;
        }
    }

    public static string? GetUserRoleFromToken(HttpContext context)
    {
        var token = context.Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(token)) return null;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
        catch
        {
            return null;
        }
    }
}