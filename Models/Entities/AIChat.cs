namespace SmartStudyPlannerAI.Models.Entities;

public class AIChat
{
    public int Id { get; set; }
    public string UserMessage { get; set; } = string.Empty;
    public string AIResponse { get; set; } = string.Empty;
    public string? ConversationId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}