using Microsoft.EntityFrameworkCore;
using SmartStudyPlannerAI.Data;
using SmartStudyPlannerAI.Models.Entities;
using SmartStudyPlannerAI.Repositories.Interfaces;

namespace SmartStudyPlannerAI.Repositories.Implementations;

public class TaskRepository : Repository<TaskItem>, ITaskRepository
{
    public TaskRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<TaskItem>> GetByUserIdAsync(int userId)
    {
        return await _dbSet.Where(t => t.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetPendingTasksAsync(int userId)
    {
        return await _dbSet.Where(t => t.UserId == userId && t.Status != "Completed").ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync(int userId)
    {
        return await _dbSet.Where(t => t.UserId == userId && t.DueDate < DateTime.Now && t.Status != "Completed").ToListAsync();
    }
}