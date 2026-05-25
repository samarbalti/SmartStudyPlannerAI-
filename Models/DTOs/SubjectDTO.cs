using System.ComponentModel.DataAnnotations;

namespace SmartStudyPlannerAI.Models.DTOs;

public class SubjectDTO
{
    public int? Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Color { get; set; }
}