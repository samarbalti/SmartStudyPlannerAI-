using System.ComponentModel.DataAnnotations;

namespace SmartStudyPlannerAI.Models.Entities;

public class Exam
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public DateTime ExamDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string? Location { get; set; }
    public int UserId { get; set; }
    public int? SubjectId { get; set; }
    public bool NotificationSent { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Subject? Subject { get; set; }
}