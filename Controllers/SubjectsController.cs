using Microsoft.AspNetCore.Mvc;
using SmartStudyPlannerAI.Data;
using SmartStudyPlannerAI.Helpers;
using SmartStudyPlannerAI.Models.DTOs;
using SmartStudyPlannerAI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace SmartStudyPlannerAI.Controllers;

public class SubjectsController : Controller
{
    private readonly ApplicationDbContext _context;

    public SubjectsController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int? GetUserId() => JwtHelper.GetUserIdFromToken(HttpContext);

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var subjects = await _context.Subjects
            .Where(s => s.UserId == userId.Value)
            .ToListAsync();

        return View(subjects);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(SubjectDTO dto)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid) return View(dto);

        var subject = new Subject
        {
            UserId = userId.Value,
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category,
            Color = dto.Color ?? "#3498db"
        };

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId.Value);

        if (subject == null) return NotFound();

        var dto = new SubjectDTO
        {
            Id = subject.Id,
            Name = subject.Name,
            Description = subject.Description,
            Category = subject.Category,
            Color = subject.Color
        };

        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, SubjectDTO dto)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId.Value);

        if (subject == null) return NotFound();

        subject.Name = dto.Name;
        subject.Description = dto.Description;
        subject.Category = dto.Category;
        subject.Color = dto.Color ?? subject.Color;

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId.Value);

        if (subject != null)
        {
            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
}