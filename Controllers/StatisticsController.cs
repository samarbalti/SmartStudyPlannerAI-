using Microsoft.AspNetCore.Mvc;
using SmartStudyPlannerAI.Helpers;
using SmartStudyPlannerAI.Services.Interfaces;

namespace SmartStudyPlannerAI.Controllers;

public class StatisticsController : Controller
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = JwtHelper.GetUserIdFromToken(HttpContext);
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var statistics = await _statisticsService.GetUserStatisticsAsync(userId.Value);
        return View(statistics);
    }

    [HttpGet]
    public async Task<IActionResult> GetChartData()
    {
        var userId = JwtHelper.GetUserIdFromToken(HttpContext);
        if (!userId.HasValue) return Unauthorized();

        var weeklyData = await _statisticsService.GetWeeklyStudyTimeAsync(userId.Value);
        var subjectProgress = await _statisticsService.GetSubjectProgressAsync(userId.Value);

        return Json(new
        {
            weeklyLabels = weeklyData.Select(w => w.Day),
            weeklyData = weeklyData.Select(w => w.Hours),
            subjectLabels = subjectProgress.Select(s => s.SubjectName),
            subjectData = subjectProgress.Select(s => s.ProgressPercentage)
        });
    }
}