using Microsoft.AspNetCore.SignalR;
using SmartStudyPlannerAI.Data;
using SmartStudyPlannerAI.Hubs;
using SmartStudyPlannerAI.Models.Entities;
using SmartStudyPlannerAI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SmartStudyPlannerAI.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task CreateTaskReminderAsync(int userId, TaskItem task)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = "Rappel de tâche",
            Message = $"La tâche '{task.Title}' est à faire pour le {task.DueDate:dd/MM/yyyy}",
            Type = "Task"
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        await SendRealTimeNotificationAsync(userId, notification.Title, notification.Message);
    }

    public async Task CreateExamReminderAsync(int userId, Exam exam)
    {
        var daysUntil = (exam.ExamDate - DateTime.Now).Days;
        var notification = new Notification
        {
            UserId = userId,
            Title = "Alerte examen",
            Message = $"Examen '{exam.Title}' dans {daysUntil} jour(s) ({exam.ExamDate:dd/MM/yyyy})",
            Type = "Exam"
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        await SendRealTimeNotificationAsync(userId, notification.Title, notification.Message);
    }

    public async Task CreateAINotificationAsync(int userId, string title, string message)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = "AI"
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        await SendRealTimeNotificationAsync(userId, title, message);
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
            notification.IsRead = true;

        await _context.SaveChangesAsync();
    }

    public async Task SendRealTimeNotificationAsync(int userId, string title, string message)
    {
        await _hubContext.Clients.User(userId.ToString())
            .SendAsync("ReceiveNotification", new { title, message, timestamp = DateTime.Now });
    }
}