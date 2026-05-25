using SmartStudyPlannerAI.Models.Entities;

namespace SmartStudyPlannerAI.Repositories.Interfaces;

public interface ITaskRepository : IRepository<TaskItem>
{
    Task<IEnumerable<TaskItem>> GetByUserIdAsync(int userId);
    Task<IEnumerable<TaskItem>> GetPendingTasksAsync(int userId);
    Task<IEnumerable<TaskItem>> GetOverdueTasksAsync(int userId);
}