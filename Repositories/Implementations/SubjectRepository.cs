using Microsoft.EntityFrameworkCore;
using SmartStudyPlannerAI.Data;
using SmartStudyPlannerAI.Models.Entities;
using SmartStudyPlannerAI.Repositories.Interfaces;

namespace SmartStudyPlannerAI.Repositories.Implementations;

public class SubjectRepository : Repository<Subject>, ISubjectRepository
{
    public SubjectRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Subject>> GetByUserIdAsync(int userId)
    {
        return await _dbSet.Where(s => s.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<Subject>> GetByCategoryAsync(int userId, string category)
    {
        return await _dbSet.Where(s => s.UserId == userId && s.Category == category).ToListAsync();
    }
}