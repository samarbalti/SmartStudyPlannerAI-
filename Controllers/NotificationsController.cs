using Microsoft.AspNetCore.Mvc;
using SmartStudyPlannerAI.Helpers;
using SmartStudyPlannerAI.Services.Interfaces;

namespace SmartStudyPlannerAI.Controllers;

public class NotificationsController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var userId = JwtHelper.GetUserIdFromToken(HttpContext);
        if (!userId.HasValue) return Unauthorized();

        var notifications = await _notificationService.GetUserNotificationsAsync(userId.Value);
        return Json(new { success = true, notifications });
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = JwtHelper.GetUserIdFromToken(HttpContext);
        if (!userId.HasValue) return Unauthorized();

        await _notificationService.MarkAllAsReadAsync(userId.Value);
        return Json(new { success = true });
    }
}