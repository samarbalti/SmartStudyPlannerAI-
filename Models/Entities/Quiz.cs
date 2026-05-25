namespace SmartStudyPlannerAI.Models.Entities;

public class Quiz
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string QuestionsData { get; set; } = string.Empty; // JSON des questions/réponses
    public string QuizType { get; set; } = "QCM"; // QCM, Q/R, Exercices
    public int? SubjectId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Subject? Subject { get; set; }
}