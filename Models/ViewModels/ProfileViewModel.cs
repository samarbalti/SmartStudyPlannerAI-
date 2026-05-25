namespace SmartStudyPlannerAI.Models.ViewModels;

public class ProfileViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public string? Goals { get; set; }
    public int TotalSubjects { get; set; }
    public int TotalTasks { get; set; }
    public int TotalExams { get; set; }
    public DateTime MemberSince { get; set; }
}