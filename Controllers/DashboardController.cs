using Microsoft.AspNetCore.Mvc;
using SmartStudyPlannerAI.Helpers;
using SmartStudyPlannerAI.Models.ViewModels;
using SmartStudyPlannerAI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SmartStudyPlannerAI.Data;

namespace SmartStudyPlannerAI.Controllers;

public class DashboardController : Controller
{
    private readonly IStatisticsService _statisticsService;
    private readonly IStudyPlanService _studyPlanService;
    private readonly ApplicationDbContext _context;

    public DashboardController(IStatisticsService statisticsService, IStudyPlanService studyPlanService, ApplicationDbContext context)
    {
        _statisticsService = statisticsService;
        _studyPlanService = studyPlanService;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = JwtHelper.GetUserIdFromToken(HttpContext);
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var user = await _context.Users.FindAsync(userId.Value);
        if (user == null) return RedirectToAction("Login", "Account");

        var statistics = await _statisticsService.GetUserStatisticsAsync(userId.Value);
        var recentTasks = await _context.Tasks
            .Where(t => t.UserId == userId.Value)
            .OrderByDescending(t => t.CreatedAt)
            .Take(5)
            .ToListAsync();

        var upcomingExams = await _context.Exams
            .Where(e => e.UserId == userId.Value && e.ExamDate >= DateTime.Now)
            .OrderBy(e => e.ExamDate)
            .Take(5)
            .ToListAsync();

        var subjects = await _context.Subjects
            .Where(s => s.UserId == userId.Value)
            .ToListAsync();

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId.Value && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .Take(10)
            .ToListAsync();

        var activePlan = await _studyPlanService.GetActivePlanAsync(userId.Value);

        var viewModel = new DashboardViewModel
        {
            User = user,
            Statistics = statistics,
            RecentTasks = recentTasks,
            UpcomingExams = upcomingExams,
            Subjects = subjects,
            RecentNotifications = notifications,
            ActiveStudyPlan = activePlan
        };

        return View(viewModel);
    }
}