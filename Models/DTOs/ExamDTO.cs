using System.ComponentModel.DataAnnotations;

namespace SmartStudyPlannerAI.Models.DTOs;

public class ExamDTO
{
    public int? Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public DateTime ExamDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string? Location { get; set; }
    public int? SubjectId { get; set; }
}