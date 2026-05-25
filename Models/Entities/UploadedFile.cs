namespace SmartStudyPlannerAI.Models.Entities;

public class UploadedFile
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty; // PDF, Image, etc.
    public long FileSize { get; set; }
    public string? ExtractedText { get; set; }
    public int UserId { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<Summary> Summaries { get; set; } = new List<Summary>();
}