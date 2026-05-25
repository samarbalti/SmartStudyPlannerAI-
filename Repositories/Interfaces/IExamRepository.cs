using SmartStudyPlannerAI.Models.Entities;

namespace SmartStudyPlannerAI.Repositories.Interfaces;

public interface IExamRepository : IRepository<Exam>
{
    Task<IEnumerable<Exam>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Exam>> GetUpcomingExamsAsync(int userId, int daysAhead = 7);
}