using SmartStudyPlannerAI.Models.Entities;

namespace SmartStudyPlannerAI.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByVerificationTokenAsync(string token);
    Task<User?> GetByResetTokenAsync(string token);
    Task<bool> EmailExistsAsync(string email);
}