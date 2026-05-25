using SmartStudyPlannerAI.Models.DTOs;
using SmartStudyPlannerAI.Models.Entities;

namespace SmartStudyPlannerAI.Models.ViewModels;

public class DashboardViewModel
{
    public User User { get; set; } = null!;
    public StatisticsDTO Statistics { get; set; } = new();
    public List<TaskItem> RecentTasks { get; set; } = new();
    public List<Exam> UpcomingExams { get; set; } = new();
    public List<Subject> Subjects { get; set; } = new();
    public List<Notification> RecentNotifications { get; set; } = new();
    public StudyPlan? ActiveStudyPlan { get; set; }
}