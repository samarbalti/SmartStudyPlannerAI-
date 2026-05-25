using SmartStudyPlannerAI.Models.Entities;

namespace SmartStudyPlannerAI.Services.Interfaces;

public interface INotificationService
{
    Task CreateTaskReminderAsync(int userId, TaskItem task);
    Task CreateExamReminderAsync(int userId, Exam exam);
    Task CreateAINotificationAsync(int userId, string title, string message);
    Task<List<Notification>> GetUserNotificationsAsync(int userId);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(int userId);
    Task SendRealTimeNotificationAsync(int userId, string title, string message);
}