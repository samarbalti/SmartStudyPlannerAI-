using Microsoft.AspNetCore.Mvc;
using SmartStudyPlannerAI.Helpers;
using SmartStudyPlannerAI.Services.Interfaces;

namespace SmartStudyPlannerAI.Controllers;

public class StudyPlanController : Controller
{
    private readonly IStudyPlanService _studyPlanService;

    public StudyPlanController(IStudyPlanService studyPlanService)
    {
        _studyPlanService = studyPlanService;
    }

    private int? GetUserId() => JwtHelper.GetUserIdFromToken(HttpContext);

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var plans = await _studyPlanService.GetUserPlansAsync(userId.Value);
        return View(plans);
    }

    [HttpGet]
    public IActionResult Generate()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Generate(DateTime startDate, DateTime endDate)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var plan = await _studyPlanService.GenerateStudyPlanAsync(userId.Value, startDate, endDate);
        if (plan == null)
        {
            TempData["Error"] = "Erreur lors de la génération du planning.";
            return View();
        }

        return RedirectToAction("Details", new { id = plan.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var viewModel = await _studyPlanService.GetPlanDetailsAsync(id);
        if (viewModel == null) return NotFound();

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        await _studyPlanService.DeletePlanAsync(id, userId.Value);
        return RedirectToAction("Index");
    }
}