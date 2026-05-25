using Newtonsoft.Json;
using SmartStudyPlannerAI.Data;
using SmartStudyPlannerAI.Models.DTOs;
using SmartStudyPlannerAI.Models.Entities;
using SmartStudyPlannerAI.Models.ViewModels;
using SmartStudyPlannerAI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SmartStudyPlannerAI.Services.Implementations;

public class StudyPlanService : IStudyPlanService
{
    private readonly ApplicationDbContext _context;
    private readonly IAIService _aiService;

    public StudyPlanService(ApplicationDbContext context, IAIService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<StudyPlan?> GenerateStudyPlanAsync(int userId, DateTime startDate, DateTime endDate)
    {
        var subjects = await _context.Subjects.Where(s => s.UserId == userId).ToListAsync();
        var exams = await _context.Exams.Where(e => e.UserId == userId && e.ExamDate >= startDate && e.ExamDate <= endDate).ToListAsync();
        var tasks = await _context.Tasks.Where(t => t.UserId == userId && t.Status != "Completed").ToListAsync();

        var subjectsText = string.Join(", ", subjects.Select(s => s.Name));
        var examsText = string.Join(", ", exams.Select(e => $"{e.Title} ({e.ExamDate:dd/MM})"));

        var prompt = $@"Génère un planning d'étude intelligent pour un étudiant.

Période: du {startDate:dd/MM/yyyy} au {endDate:dd/MM/yyyy}
Matières: {subjectsText}
Examens à venir: {examsText}
Tâches en attente: {tasks.Count}

Génère un planning quotidien et hebdomadaire optimisé. Format JSON:
{{
  ""sessions"": [
    {{""subject"": ""Nom matière"", ""date"": ""YYYY-MM-DD"", ""startTime"": ""HH:MM"", ""endTime"": ""HH:MM"", ""notes"": ""...""}}
  ]
}}";

        var aiResponse = await _aiService.ChatAsync(userId, new AIChatDTO { Message = prompt });

        var plan = new StudyPlan
        {
            UserId = userId,
            Title = $"Planning du {startDate:dd/MM} au {endDate:dd/MM}",
            Description = $"Planning généré automatiquement avec IA. {subjects.Count} matière(s), {exams.Count} examen(s).",
            StartDate = startDate,
            EndDate = endDate,
            PlanData = aiResponse,
            IsActive = true
        };

        // Désactiver l'ancien plan actif
        var activePlan = await _context.StudyPlans.FirstOrDefaultAsync(p => p.UserId == userId && p.IsActive);
        if (activePlan != null)
        {
            activePlan.IsActive = false;
            await _context.SaveChangesAsync();
        }

        _context.StudyPlans.Add(plan);
        await _context.SaveChangesAsync();

        return plan;
    }

    public async Task<StudyPlan?> GetActivePlanAsync(int userId)
    {
        return await _context.StudyPlans
            .FirstOrDefaultAsync(p => p.UserId == userId && p.IsActive);
    }

    public async Task<List<StudyPlan>> GetUserPlansAsync(int userId)
    {
        return await _context.StudyPlans
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<StudyPlan?> CreatePlanAsync(int userId, StudyPlanDTO dto)
    {
        var plan = new StudyPlan
        {
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            PlanData = dto.PlanData ?? "{}",
            IsActive = dto.IsActive
        };

        _context.StudyPlans.Add(plan);
        await _context.SaveChangesAsync();
        return plan;
    }

    public async Task<bool> DeletePlanAsync(int planId, int userId)
    {
        var plan = await _context.StudyPlans.FindAsync(planId);
        if (plan == null || plan.UserId != userId) return false;

        _context.StudyPlans.Remove(plan);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<StudyPlanViewModel?> GetPlanDetailsAsync(int planId)
    {
        var plan = await _context.StudyPlans.FindAsync(planId);
        if (plan == null) return null;

        var sessions = new List<StudySession>();
        try
        {
            dynamic planData = JsonConvert.DeserializeObject(plan.PlanData)!;
            if (planData?.sessions != null)
            {
                foreach (var session in planData.sessions)
                {
                    sessions.Add(new StudySession
                    {
                        SubjectName = session.subject,
                        Date = DateTime.Parse(session.date.ToString()),
                        StartTime = TimeSpan.Parse(session.startTime.ToString()),
                        EndTime = TimeSpan.Parse(session.endTime.ToString()),
                        Notes = session.notes
                    });
                }
            }
        }
        catch { /* Si le parsing échoue, retourner des sessions vides */ }

        var subjects = await _context.Subjects.Where(s => s.UserId == plan.UserId).ToListAsync();

        return new StudyPlanViewModel
        {
            StudyPlan = plan,
            Sessions = sessions,
            AvailableSubjects = subjects
        };
    }
}