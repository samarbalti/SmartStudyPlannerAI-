using SmartStudyPlannerAI.Models.Entities;

namespace SmartStudyPlannerAI.Models.ViewModels;

public class StudyPlanViewModel
{
    public StudyPlan? StudyPlan { get; set; }
    public List<StudySession> Sessions { get; set; } = new();
    public List<Subject> AvailableSubjects { get; set; } = new();
}

public class StudySession
{
    public string SubjectName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Notes { get; set; }
}