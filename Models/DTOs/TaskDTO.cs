using System.ComponentModel.DataAnnotations;

namespace SmartStudyPlannerAI.Models.DTOs;

public class TaskDTO
{
    public int? Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "Pending";
    public DateTime? DueDate { get; set; }
    public int? SubjectId { get; set; }
}