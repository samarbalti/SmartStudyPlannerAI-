using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.AspNetCore.Http;
using SmartStudyPlannerAI.Data;
using SmartStudyPlannerAI.Models.Entities;
using SmartStudyPlannerAI.Services.Interfaces;
using Tesseract;
using Microsoft.EntityFrameworkCore;

namespace SmartStudyPlannerAI.Services.Implementations;

public class FileService : IFileService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public FileService(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<UploadedFile?> UploadFileAsync(IFormFile file, int userId)
    {
        if (file == null || file.Length == 0) return null;

        var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsRoot))
            Directory.CreateDirectory(uploadsRoot);

        var userFolderName = $"user_{userId}";
        var userFolder = Path.Combine(uploadsRoot, userFolderName);
        if (!Directory.Exists(userFolder))
            Directory.CreateDirectory(userFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(userFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var uploadedFile = new UploadedFile
        {
            UserId = userId,
            FileName = uniqueFileName,
            OriginalName = file.FileName,
            FilePath = $"/uploads/{userFolderName}/{uniqueFileName}",
            FileType = Path.GetExtension(file.FileName).ToLower(),
            FileSize = file.Length
        };

        // Extraction automatique du texte
        if (uploadedFile.FileType == ".pdf")
        {
            uploadedFile.ExtractedText = await ExtractTextFromPdfAsync(filePath);
        }
        else if (new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" }.Contains(uploadedFile.FileType))
        {
            uploadedFile.ExtractedText = await ExtractTextFromImageAsync(filePath);
        }

        _context.UploadedFiles.Add(uploadedFile);
        await _context.SaveChangesAsync();

        return uploadedFile;
    }

    public async Task<string?> ExtractTextFromPdfAsync(string filePath)
    {
        try
        {
            using var pdfDoc = new PdfDocument(new PdfReader(filePath));
            var text = new System.Text.StringBuilder();
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                text.Append(PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i)));
            }
            return text.ToString();
        }
        catch (Exception ex)
        {
            return $"Erreur extraction PDF: {ex.Message}";
        }
    }

    public async Task<string?> ExtractTextFromImageAsync(string filePath)
    {
        try
        {
            var tessdataPath = Path.Combine(_environment.WebRootPath, "tessdata");
            if (!Directory.Exists(tessdataPath))
                Directory.CreateDirectory(tessdataPath);

            using var engine = new TesseractEngine(tessdataPath, "fra+eng", EngineMode.Default);
            using var img = Pix.LoadFromFile(filePath);
            using var page = engine.Process(img);
            return page.GetText();
        }
        catch (Exception ex)
        {
            return $"Erreur OCR: {ex.Message}";
        }
    }

    public async Task<bool> DeleteFileAsync(int fileId, int userId)
    {
        var file = await _context.UploadedFiles.FindAsync(fileId);
        if (file == null || file.UserId != userId) return false;

        var fullPath = Path.Combine(_environment.WebRootPath, file.FilePath.TrimStart('/'));
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        _context.UploadedFiles.Remove(file);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UploadedFile?> GetFileAsync(int fileId)
    {
        return await _context.UploadedFiles.FindAsync(fileId);
    }

    public async Task<List<UploadedFile>> GetUserFilesAsync(int userId)
    {
        return await _context.UploadedFiles.Where(f => f.UserId == userId).ToListAsync();
    }
}