using SmartStudyPlannerAI.Models.DTOs;

namespace SmartStudyPlannerAI.Services.Interfaces;

public interface IStatisticsService
{
    Task<StatisticsDTO> GetUserStatisticsAsync(int userId);
    Task<List<SubjectProgress>> GetSubjectProgressAsync(int userId);
    Task<List<WeeklyStudyTime>> GetWeeklyStudyTimeAsync(int userId);
    Task RecordStudySessionAsync(int userId, int? subjectId, int minutes);
}