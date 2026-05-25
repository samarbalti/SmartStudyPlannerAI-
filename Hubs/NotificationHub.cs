using Microsoft.AspNetCore.SignalR;
using SmartStudyPlannerAI.Helpers;

namespace SmartStudyPlannerAI.Hubs;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = JwtHelper.GetUserIdFromToken(Context.GetHttpContext());
        if (userId.HasValue)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId.Value.ToString());
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = JwtHelper.GetUserIdFromToken(Context.GetHttpContext());
        if (userId.HasValue)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.Value.ToString());
        }
        await base.OnDisconnectedAsync(exception);
    }
}