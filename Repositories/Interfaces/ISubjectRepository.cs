using SmartStudyPlannerAI.Models.Entities;

namespace SmartStudyPlannerAI.Repositories.Interfaces;

public interface ISubjectRepository : IRepository<Subject>
{
    Task<IEnumerable<Subject>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Subject>> GetByCategoryAsync(int userId, string category);
}