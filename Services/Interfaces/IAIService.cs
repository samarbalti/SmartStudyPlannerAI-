using SmartStudyPlannerAI.Models.DTOs;
using SmartStudyPlannerAI.Models.Entities;

namespace SmartStudyPlannerAI.Services.Interfaces;

public interface IAIService
{
    Task<string> ChatAsync(int userId, AIChatDTO dto, bool sendNotification = true);
    Task<string> SummarizeTextAsync(string text, string? context = null);
    Task<string> SummarizeFileAsync(int fileId, int userId);
    Task<Quiz> GenerateQuizAsync(int? subjectId, int userId, string quizType = "QCM");
    Task<string> GetStudyRecommendationsAsync(int userId);
    Task<string> AnalyzeImageAsync(string imagePath, string? prompt = null);
    Task<string> ExtractTextFromImageAsync(string imagePath);
}