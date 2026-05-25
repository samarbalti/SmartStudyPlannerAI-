using SmartStudyPlannerAI.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace SmartStudyPlannerAI.Services.Interfaces;

public interface IFileService
{
    Task<UploadedFile?> UploadFileAsync(IFormFile file, int userId);
    Task<string?> ExtractTextFromPdfAsync(string filePath);
    Task<string?> ExtractTextFromImageAsync(string filePath);
    Task<bool> DeleteFileAsync(int fileId, int userId);
    Task<UploadedFile?> GetFileAsync(int fileId);
    Task<List<UploadedFile>> GetUserFilesAsync(int userId);
}