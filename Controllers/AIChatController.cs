using Microsoft.AspNetCore.Mvc;
using SmartStudyPlannerAI.Helpers;
using SmartStudyPlannerAI.Models.DTOs;
using SmartStudyPlannerAI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SmartStudyPlannerAI.Data;

namespace SmartStudyPlannerAI.Controllers;

public class AIChatController : Controller
{
    private readonly IAIService _aiService;
    private readonly ApplicationDbContext _context;

    public AIChatController(IAIService aiService, ApplicationDbContext context)
    {
        _aiService = aiService;
        _context = context;
    }

    private int? GetUserId() => JwtHelper.GetUserIdFromToken(HttpContext);

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var chats = await _context.AIChats
            .Where(c => c.UserId == userId.Value)
            .OrderByDescending(c => c.CreatedAt)
            .Take(50)
            .ToListAsync();

        return View(chats);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] AIChatDTO dto)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized();

        var response = await _aiService.ChatAsync(userId.Value, dto);
        return Json(new { success = true, response });
    }

    [HttpPost]
    public async Task<IActionResult> SummarizeText([FromBody] string text)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized();

        var summary = await _aiService.SummarizeTextAsync(text);
        return Json(new { success = true, summary });
    }

    [HttpPost]
    public async Task<IActionResult> GenerateQuiz(int? subjectId, string quizType = "QCM")
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized();

        var quiz = await _aiService.GenerateQuizAsync(subjectId, userId.Value, quizType);
        return Json(new { success = true, quiz });
    }

    [HttpGet]
    public async Task<IActionResult> GetRecommendations()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized();

        var recommendations = await _aiService.GetStudyRecommendationsAsync(userId.Value);
        return Json(new { success = true, recommendations });
    }
}