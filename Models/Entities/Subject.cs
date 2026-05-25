using System.ComponentModel.DataAnnotations;

namespace SmartStudyPlannerAI.Models.Entities;

public class Subject
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Color { get; set; } = "#3498db";
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}