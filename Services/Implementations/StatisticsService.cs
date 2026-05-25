using Microsoft.EntityFrameworkCore;
using SmartStudyPlannerAI.Data;
using SmartStudyPlannerAI.Models.DTOs;
using SmartStudyPlannerAI.Services.Interfaces;

namespace SmartStudyPlannerAI.Services.Implementations;

public class StatisticsService : IStatisticsService
{
    private readonly ApplicationDbContext _context;

    public StatisticsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StatisticsDTO> GetUserStatisticsAsync(int userId)
    {
        var totalTasks = await _context.Tasks.CountAsync(t => t.UserId == userId);
        var completedTasks = await _context.Tasks.CountAsync(t => t.UserId == userId && t.Status == "Completed");
        var upcomingExams = await _context.Exams.CountAsync(e => e.UserId == userId && e.ExamDate > DateTime.Now);

        return new StatisticsDTO
        {
            TotalStudyHours = await CalculateTotalStudyHours(userId),
            CompletedTasks = completedTasks,
            TotalTasks = totalTasks,
            UpcomingExams = upcomingExams,
            SubjectProgresses = await GetSubjectProgressAsync(userId),
            WeeklyStudyTimes = await GetWeeklyStudyTimeAsync(userId)
        };
    }

    public async Task<List<SubjectProgress>> GetSubjectProgressAsync(int userId)
    {
        var subjects = await _context.Subjects.Where(s => s.UserId == userId).ToListAsync();
        var progressList = new List<SubjectProgress>();

        foreach (var subject in subjects)
        {
            var total = await _context.Tasks.CountAsync(t => t.SubjectId == subject.Id);
            var completed = await _context.Tasks.CountAsync(t => t.SubjectId == subject.Id && t.Status == "Completed");

            progressList.Add(new SubjectProgress
            {
                SubjectName = subject.Name,
                TotalTasks = total,
                CompletedTasks = completed
            });
        }

        return progressList;
    }

    public async Task<List<WeeklyStudyTime>> GetWeeklyStudyTimeAsync(int userId)
    {
        var days = new[] { "Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi", "Samedi", "Dimanche" };
        var weeklyData = new List<WeeklyStudyTime>();

        // Simulation de données - dans une vraie app, vous stockeriez les sessions d'étude
        var random = new Random(userId);
        foreach (var day in days)
        {
            weeklyData.Add(new WeeklyStudyTime
            {
                Day = day,
                Hours = random.Next(0, 6)
            });
        }

        return weeklyData;
    }

    public async Task RecordStudySessionAsync(int userId, int? subjectId, int minutes)
    {
        // Implémentation pour enregistrer une session d'étude
        // Vous pourriez créer une table StudySessions pour stocker ces données
        await Task.CompletedTask;
    }

    private async Task<int> CalculateTotalStudyHours(int userId)
    {
        // Calcul basé sur les tâches complétées et le temps estimé
        var completedTasks = await _context.Tasks
            .Where(t => t.UserId == userId && t.Status == "Completed")
            .ToListAsync();

        // Estimation: 2 heures par tâche complétée (à adapter selon vos besoins)
        return completedTasks.Count * 2;
    }
}