using System.ComponentModel.DataAnnotations;

namespace SmartStudyPlannerAI.Models.DTOs;

public class StudyPlanDTO
{
    public int? Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? PlanData { get; set; }
    public bool IsActive { get; set; } = true;
}