namespace SmartStudyPlannerAI.Models.Entities;

public class PasswordResetOTP
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string OtpCode { get; set; } = string.Empty; // 6-digit OTP
    public bool IsUsed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } // 10 minutes from creation

    public User User { get; set; } = null!;
}
