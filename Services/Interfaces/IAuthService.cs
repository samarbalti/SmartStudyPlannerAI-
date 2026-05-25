using SmartStudyPlannerAI.Models.DTOs;
using SmartStudyPlannerAI.Models.Entities;

namespace SmartStudyPlannerAI.Services.Interfaces;

public interface IAuthService
{
    Task<(User? user, string? token)> LoginAsync(LoginDTO dto);
    Task<(bool success, string message)> RegisterAsync(RegisterDTO dto);
    Task<bool> VerifyEmailAsync(string token);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(string token, string newPassword);
    Task<User?> GetCurrentUserAsync(int userId);
    string GenerateJwtToken(User user);
}