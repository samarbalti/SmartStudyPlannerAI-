using Microsoft.Extensions.Hosting;
using SmartStudyPlannerAI.Data;
using SmartStudyPlannerAI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SmartStudyPlannerAI.Services.Implementations;

public class NotificationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<NotificationBackgroundService> _logger;

    public NotificationBackgroundService(IServiceProvider services, ILogger<NotificationBackgroundService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var notifier = scope.ServiceProvider.GetRequiredService<INotificationService>();

                // Exams within 3 days
                var upcomingExams = await db.Exams
                    .Where(e => e.ExamDate >= DateTime.UtcNow && e.ExamDate <= DateTime.UtcNow.AddDays(3))
                    .ToListAsync(stoppingToken);

                _logger.LogInformation($"Found {upcomingExams.Count} upcoming exams");

                foreach (var exam in upcomingExams)
                {
                    // avoid duplicate notifications with same title/content
                    var exists = await db.Notifications
                        .AnyAsync(n => n.UserId == exam.UserId && n.Type == "Exam" && n.Message.Contains(exam.Title), stoppingToken);
                    if (!exists)
                    {
                        _logger.LogInformation($"Creating exam notification for user {exam.UserId}: {exam.Title}");
                        await notifier.CreateExamReminderAsync(exam.UserId, exam);
                    }
                }

                // Tasks due within 1 hour
                var imminentTasks = await db.Tasks
                    .Where(t => t.DueDate.HasValue && t.DueDate > DateTime.UtcNow && t.DueDate <= DateTime.UtcNow.AddHours(1) && t.Status != "Completed")
                    .ToListAsync(stoppingToken);

                _logger.LogInformation($"Found {imminentTasks.Count} imminent tasks");

                foreach (var task in imminentTasks)
                {
                    var exists = await db.Notifications
                        .AnyAsync(n => n.UserId == task.UserId && n.Type == "Task" && n.Message.Contains(task.Title), stoppingToken);
                    if (!exists)
                    {
                        _logger.LogInformation($"Creating task notification for user {task.UserId}: {task.Title}");
                        await notifier.CreateTaskReminderAsync(task.UserId, task);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in NotificationBackgroundService");
            }

            // Check every 10 seconds for faster feedback during testing
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
