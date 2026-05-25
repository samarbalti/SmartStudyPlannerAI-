namespace SmartStudyPlannerAI.Models.DTOs;

public class StatisticsDTO
{
    public int TotalStudyHours { get; set; }
    public int CompletedTasks { get; set; }
    public int TotalTasks { get; set; }
    public int UpcomingExams { get; set; }
    public List<SubjectProgress> SubjectProgresses { get; set; } = new();
    public List<WeeklyStudyTime> WeeklyStudyTimes { get; set; } = new();
}

public class SubjectProgress
{
    public string SubjectName { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public double ProgressPercentage => TotalTasks > 0 ? (CompletedTasks / (double)TotalTasks) * 100 : 0;
}

public class WeeklyStudyTime
{
    public string Day { get; set; } = string.Empty;
    public int Hours { get; set; }
}