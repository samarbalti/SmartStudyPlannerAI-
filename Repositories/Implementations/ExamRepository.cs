using Microsoft.EntityFrameworkCore;
using SmartStudyPlannerAI.Data;
using SmartStudyPlannerAI.Models.Entities;
using SmartStudyPlannerAI.Repositories.Interfaces;

namespace SmartStudyPlannerAI.Repositories.Implementations;

public class ExamRepository : Repository<Exam>, IExamRepository
{
    public ExamRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Exam>> GetByUserIdAsync(int userId)
    {
        return await _dbSet.Where(e => e.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<Exam>> GetUpcomingExamsAsync(int userId, int daysAhead = 7)
    {
        var futureDate = DateTime.Now.AddDays(daysAhead);
        return await _dbSet.Where(e => e.UserId == userId && e.ExamDate <= futureDate && e.ExamDate >= DateTime.Now)
                          .OrderBy(e => e.ExamDate)
                          .ToListAsync();
    }
}