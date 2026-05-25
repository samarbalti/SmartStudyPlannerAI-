using SmartStudyPlannerAI.Models.DTOs;
using SmartStudyPlannerAI.Models.Entities;
using SmartStudyPlannerAI.Models.ViewModels;

namespace SmartStudyPlannerAI.Services.Interfaces;

public interface IStudyPlanService
{
    Task<StudyPlan?> GenerateStudyPlanAsync(int userId, DateTime startDate, DateTime endDate);
    Task<StudyPlan?> GetActivePlanAsync(int userId);
    Task<List<StudyPlan>> GetUserPlansAsync(int userId);
    Task<StudyPlan?> CreatePlanAsync(int userId, StudyPlanDTO dto);
    Task<bool> DeletePlanAsync(int planId, int userId);
    Task<StudyPlanViewModel?> GetPlanDetailsAsync(int planId);
}