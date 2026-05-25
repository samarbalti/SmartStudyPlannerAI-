using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using SmartStudyPlannerAI.Data;
using SmartStudyPlannerAI.Models.DTOs;
using SmartStudyPlannerAI.Models.Entities;
using SmartStudyPlannerAI.Repositories.Interfaces;
using SmartStudyPlannerAI.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartStudyPlannerAI.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(IUserRepository userRepository, IConfiguration configuration, IEmailService emailService)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<(User? user, string? token)> LoginAsync(LoginDTO dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return (null, null);

        user.LastLogin = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var token = GenerateJwtToken(user);
        return (user, token);
    }

    public async Task<(bool success, string message)> RegisterAsync(RegisterDTO dto)
    {
        if (await _userRepository.EmailExistsAsync(dto.Email))
            return (false, "Cet email est déjà utilisé.");

        var verificationToken = Guid.NewGuid().ToString();
        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            EmailVerificationToken = verificationToken,
            Role = "Student"
        };

        await _userRepository.AddAsync(user);

        var verificationLink = $"{_configuration["Jwt:Issuer"]}/Account/VerifyEmail?token={verificationToken}";
        await _emailService.SendVerificationEmailAsync(user.Email, verificationLink);

        return (true, "Inscription réussie ! Vérifiez votre email.");
    }

    public async Task<bool> VerifyEmailAsync(string token)
    {
        var user = await _userRepository.GetByVerificationTokenAsync(token);
        if (user == null) return false;

        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return false;

        user.PasswordResetToken = Guid.NewGuid().ToString();
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(24);
        await _userRepository.UpdateAsync(user);

        var resetLink = $"{_configuration["Jwt:Issuer"]}/Account/ResetPassword?token={user.PasswordResetToken}";
        await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        var user = await _userRepository.GetByResetTokenAsync(token);
        if (user == null || user.PasswordResetTokenExpiry < DateTime.UtcNow) return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<User?> GetCurrentUserAsync(int userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    public string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(double.Parse(_configuration["Jwt:ExpireDays"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}