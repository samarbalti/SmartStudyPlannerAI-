namespace SmartStudyPlannerAI.Models.DTOs;

public class AIChatDTO
{
    public string Message { get; set; } = string.Empty;
    public string? ConversationId { get; set; }
    public string? Context { get; set; }
}