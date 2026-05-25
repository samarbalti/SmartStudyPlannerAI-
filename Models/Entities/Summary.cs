namespace SmartStudyPlannerAI.Models.Entities;

public class Summary
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? KeyPoints { get; set; } // JSON des points importants
    public int UserId { get; set; }
    public int? FileId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public UploadedFile? UploadedFile { get; set; }
}