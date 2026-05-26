using Microsoft.AspNetCore.SignalR;
using SmartStudyPlannerAI.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SmartStudyPlannerAI.Hubs;

public class NotificationHub : Hub
{
    private int? GetUserIdFromContext(HubCallerContext context)
    {
        var httpContext = context.GetHttpContext();
        if (httpContext == null) return null;

        // Try to get from JWT Helper first (which reads cookies)
        var userId = JwtHelper.GetUserIdFromToken(httpContext);
        if (userId.HasValue) return userId;

        // Try to get from Authorization header (for SignalR token via accessTokenFactory)
        var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length);
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var id))
                    return id;
            }
            catch { }
        }

        return null;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserIdFromContext(Context);
        if (userId.HasValue)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId.Value.ToString());
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserIdFromContext(Context);
        if (userId.HasValue)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.Value.ToString());
        }
        await base.OnDisconnectedAsync(exception);
    }
}