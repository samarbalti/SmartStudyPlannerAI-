using Microsoft.AspNetCore.Mvc;
using SmartStudyPlannerAI.Data;
using SmartStudyPlannerAI.Helpers;
using SmartStudyPlannerAI.Models.DTOs;
using SmartStudyPlannerAI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace SmartStudyPlannerAI.Controllers;

public class ExamsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ExamsController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int? GetUserId() => JwtHelper.GetUserIdFromToken(HttpContext);

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var exams = await _context.Exams
            .Where(e => e.UserId == userId.Value)
            .OrderBy(e => e.ExamDate)
            .ToListAsync();

        ViewBag.Subjects = await _context.Subjects.Where(s => s.UserId == userId.Value).ToListAsync();
        return View(exams);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        ViewBag.Subjects = await _context.Subjects.Where(s => s.UserId == userId.Value).ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ExamDTO dto)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
        {
            ViewBag.Subjects = await _context.Subjects.Where(s => s.UserId == userId.Value).ToListAsync();
            return View(dto);
        }

        var exam = new Exam
        {
            UserId = userId.Value,
            Title = dto.Title,
            Description = dto.Description,
            ExamDate = dto.ExamDate,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Location = dto.Location,
            SubjectId = dto.SubjectId
        };

        _context.Exams.Add(exam);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var exam = await _context.Exams
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId.Value);

        if (exam == null) return NotFound();

        ViewBag.Subjects = await _context.Subjects.Where(s => s.UserId == userId.Value).ToListAsync();

        var dto = new ExamDTO
        {
            Id = exam.Id,
            Title = exam.Title,
            Description = exam.Description,
            ExamDate = exam.ExamDate,
            StartTime = exam.StartTime,
            EndTime = exam.EndTime,
            Location = exam.Location,
            SubjectId = exam.SubjectId
        };

        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, ExamDTO dto)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var exam = await _context.Exams
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId.Value);

        if (exam == null) return NotFound();

        exam.Title = dto.Title;
        exam.Description = dto.Description;
        exam.ExamDate = dto.ExamDate;
        exam.StartTime = dto.StartTime;
        exam.EndTime = dto.EndTime;
        exam.Location = dto.Location;
        exam.SubjectId = dto.SubjectId;

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var exam = await _context.Exams
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId.Value);

        if (exam != null)
        {
            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
}