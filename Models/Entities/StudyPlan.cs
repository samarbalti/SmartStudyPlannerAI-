using System.ComponentModel.DataAnnotations;

namespace SmartStudyPlannerAI.Models.Entities;

public class StudyPlan
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string PlanData { get; set; } = string.Empty; // JSON des sessions d'étude
    public bool IsActive { get; set; } = true;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}