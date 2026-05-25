using Microsoft.AspNetCore.Mvc;
using SmartStudyPlannerAI.Data;
using SmartStudyPlannerAI.Helpers;
using SmartStudyPlannerAI.Models.DTOs;
using SmartStudyPlannerAI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace SmartStudyPlannerAI.Controllers;

public class TasksController : Controller
{
    private readonly ApplicationDbContext _context;

    public TasksController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int? GetUserId() => JwtHelper.GetUserIdFromToken(HttpContext);

    public async Task<IActionResult> Index(string? status = null, string? priority = null)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var query = _context.Tasks.Where(t => t.UserId == userId.Value);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(t => t.Status == status);
        if (!string.IsNullOrEmpty(priority))
            query = query.Where(t => t.Priority == priority);

        var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        ViewBag.Subjects = await _context.Subjects.Where(s => s.UserId == userId.Value).ToListAsync();

        return View(tasks);
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
    public async Task<IActionResult> Create(TaskDTO dto)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
        {
            ViewBag.Subjects = await _context.Subjects.Where(s => s.UserId == userId.Value).ToListAsync();
            return View(dto);
        }

        var task = new TaskItem
        {
            UserId = userId.Value,
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            Status = dto.Status,
            DueDate = dto.DueDate,
            SubjectId = dto.SubjectId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId.Value);

        if (task == null) return NotFound();

        ViewBag.Subjects = await _context.Subjects.Where(s => s.UserId == userId.Value).ToListAsync();

        var dto = new TaskDTO
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            Status = task.Status,
            DueDate = task.DueDate,
            SubjectId = task.SubjectId
        };

        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, TaskDTO dto)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId.Value);

        if (task == null) return NotFound();

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Priority = dto.Priority;
        task.Status = dto.Status;
        task.DueDate = dto.DueDate;
        task.SubjectId = dto.SubjectId;

        if (dto.Status == "Completed" && task.CompletedAt == null)
            task.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized();

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId.Value);

        if (task == null) return NotFound();

        task.Status = task.Status == "Completed" ? "Pending" : "Completed";
        task.CompletedAt = task.Status == "Completed" ? DateTime.UtcNow : null;

        await _context.SaveChangesAsync();
        return Json(new { success = true, status = task.Status });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId.Value);

        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
}