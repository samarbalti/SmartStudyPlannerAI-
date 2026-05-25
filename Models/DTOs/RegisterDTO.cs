using System.ComponentModel.DataAnnotations;

namespace SmartStudyPlannerAI.Models.DTOs;

public class RegisterDTO
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}