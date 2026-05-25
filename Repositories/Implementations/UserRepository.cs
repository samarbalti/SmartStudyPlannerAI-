using Microsoft.EntityFrameworkCore;
using SmartStudyPlannerAI.Data;
using SmartStudyPlannerAI.Models.Entities;
using SmartStudyPlannerAI.Repositories.Interfaces;

namespace SmartStudyPlannerAI.Repositories.Implementations;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByVerificationTokenAsync(string token)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
    }

    public async Task<User?> GetByResetTokenAsync(string token)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.PasswordResetToken == token);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }
}