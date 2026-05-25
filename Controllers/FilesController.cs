using Microsoft.AspNetCore.Mvc;
using SmartStudyPlannerAI.Helpers;
using SmartStudyPlannerAI.Services.Interfaces;

namespace SmartStudyPlannerAI.Controllers;

public class FilesController : Controller
{
    private readonly IFileService _fileService;
    private readonly IAIService _aiService;

    public FilesController(IFileService fileService, IAIService aiService)
    {
        _fileService = fileService;
        _aiService = aiService;
    }

    private int? GetUserId() => JwtHelper.GetUserIdFromToken(HttpContext);

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var files = await _fileService.GetUserFilesAsync(userId.Value);
        return View(files);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized();

        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Veuillez sélectionner un fichier.";
            return RedirectToAction("Index");
        }

        var uploadedFile = await _fileService.UploadFileAsync(file, userId.Value);
        if (uploadedFile == null)
        {
            TempData["Error"] = "Erreur lors du téléchargement.";
            return RedirectToAction("Index");
        }

        TempData["Success"] = $"Fichier '{uploadedFile.OriginalName}' téléchargé avec succès !";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Summarize(int fileId)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized();

        var summary = await _aiService.SummarizeFileAsync(fileId, userId.Value);
        return Json(new { success = true, summary });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized();

        var success = await _fileService.DeleteFileAsync(id, userId.Value);
        return Json(new { success });
    }
}