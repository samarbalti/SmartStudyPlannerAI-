using System.ComponentModel.DataAnnotations;

namespace SmartStudyPlannerAI.Models.Entities;

public class TaskItem
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string Priority { get; set; } = "Medium"; // Low, Medium, High
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int UserId { get; set; }
    public int? SubjectId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Subject? Subject { get; set; }
}