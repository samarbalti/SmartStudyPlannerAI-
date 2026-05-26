using Newtonsoft.Json;
using SmartStudyPlannerAI.Data;
using SmartStudyPlannerAI.Models.DTOs;
using SmartStudyPlannerAI.Models.Entities;
using SmartStudyPlannerAI.Repositories.Interfaces;
using SmartStudyPlannerAI.Services.Interfaces;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SmartStudyPlannerAI.Services.Implementations;

public class AIService : IAIService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly INotificationService _notificationService;

    public AIService(IConfiguration configuration, ApplicationDbContext context, INotificationService notificationService)
    {
        _configuration = configuration;
        _context = context;
        _notificationService = notificationService;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["Groq:ApiKey"]}");
    }

    public async Task<string> ChatAsync(int userId, AIChatDTO dto, bool sendNotification = true)
    {
        var systemPrompt = @"Tu es un assistant étudiant intelligent et bienveillant. Tu aides les étudiants à :
- Organiser leurs études et leur temps
- Expliquer des concepts complexes de manière simple
- Donner des conseils de révision efficaces
- Répondre à des questions académiques
- Motiver et encourager l'étudiant

Réponds en français de manière claire, structurée et pédagogique.";

        var requestBody = new
        {
            model = _configuration["Groq:Model"],
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = dto.Message }
            },
            temperature = 0.7,
            max_tokens = 2048
        };

        var json = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_configuration["Groq:Endpoint"], content);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return $"Erreur API: {responseJson}";

        dynamic result = JsonConvert.DeserializeObject(responseJson)!;
        string aiResponse = result.choices[0].message.content;

        // Sauvegarder la conversation
        var chat = new AIChat
        {
            UserId = userId,
            UserMessage = dto.Message,
            AIResponse = aiResponse,
            ConversationId = dto.ConversationId ?? Guid.NewGuid().ToString()
        };
        _context.AIChats.Add(chat);
        await _context.SaveChangesAsync();

        // Optionally send a lightweight notification to inform the user that the AI response is ready
        if (sendNotification)
        {
            try
            {
                var preview = aiResponse.Length > 200 ? aiResponse.Substring(0, 200) + "..." : aiResponse;
                await _notificationService.CreateAINotificationAsync(userId, "Réponse IA prête", preview);
            }
            catch
            {
                // Swallow notification errors to avoid breaking chat
            }
        }

        return aiResponse;
    }

    public async Task<string> SummarizeTextAsync(string text, string? context = null)
    {
        var contextText = context != null ? $"Contexte: {context}" : "";
        var prompt = $@"Résume le texte suivant de manière concise et structurée. 
{contextText}

Texte à résumer:
{text}

Format de réponse:
- Résumé (3-5 phrases)
- Points clés (liste à puces)
- Questions de révision possibles";

        return await CallGroqAPI(prompt);
    }

    public async Task<string> SummarizeFileAsync(int fileId, int userId)
    {
        var file = await _context.UploadedFiles.FindAsync(fileId);
        if (file == null || file.UserId != userId) return "Fichier non trouvé.";

        var text = file.ExtractedText ?? "Aucun texte extrait.";
        var summary = await SummarizeTextAsync(text, $"Cours: {file.OriginalName}");

        var summaryEntity = new Summary
        {
            UserId = userId,
            FileId = fileId,
            Title = $"Résumé: {file.OriginalName}",
            Content = summary
        };

        _context.Summaries.Add(summaryEntity);
        await _context.SaveChangesAsync();

        return summary;
    }

    public async Task<Quiz> GenerateQuizAsync(int? subjectId, int userId, string quizType = "QCM")
    {
        var subject = subjectId.HasValue ? await _context.Subjects.FindAsync(subjectId.Value) : null;
        var targetText = subject != null ? $"la matière: {subject.Name}" : "révision générale";
        var prompt = $@"Génère un quiz de type {quizType} pour {targetText}.

Format JSON attendu:
{{
  ""title"": ""Titre du quiz"",
  ""questions"": [
    {{
      ""question"": ""Question?"",
      ""options"": [""A"", ""B"", ""C"", ""D""],
      ""correctAnswer"": 0,
      ""explanation"": ""Explication""
    }}
  ]
}}";

        var response = await CallGroqAPI(prompt);

        var quiz = new Quiz
        {
            UserId = userId,
            SubjectId = subjectId,
            Title = subject != null ? $"Quiz - {subject.Name}" : "Quiz de révision",
            QuestionsData = response,
            QuizType = quizType
        };

        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync();

        return quiz;
    }

    public async Task<string> GetStudyRecommendationsAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        var subjects = await _context.Subjects.Where(s => s.UserId == userId).ToListAsync();
        var tasks = await _context.Tasks.Where(t => t.UserId == userId && t.Status != "Completed").ToListAsync();
        var exams = await _context.Exams.Where(e => e.UserId == userId && e.ExamDate > DateTime.Now).ToListAsync();

        var subjectsText = string.Join(", ", subjects.Select(s => s.Name));
        var examsText = exams.Any()
            ? "Prochains examens: " + string.Join(", ", exams.Take(3).Select(e => $"{e.Title} ({e.ExamDate:dd/MM})"))
            : "";

        var context = $@"Étudiant: {user?.FullName}
Matières: {subjectsText}
Tâches en attente: {tasks.Count}
Examens à venir: {exams.Count}
{examsText}";

        var prompt = $@"En tant que conseiller pédagogique, analyse cette situation et donne des recommandations personnalisées:

{context}

Donne:
1. Un plan de révision suggéré pour la semaine
2. Les matières à prioriser
3. Des conseils de gestion du temps
4. Techniques de mémorisation adaptées";

        return await CallGroqAPI(prompt);
    }

    public async Task<string> AnalyzeImageAsync(string imagePath, string? prompt = null)
    {
        var userPrompt = prompt ?? "Analyse cette image et explique son contenu de manière pédagogique.";
        // Pour l'analyse d'image avec Groq, il faudrait encoder l'image en base64
        // Cette implémentation est simplifiée
        return await CallGroqAPI(userPrompt + "\n[Image: " + imagePath + "]");
    }

    public async Task<string> ExtractTextFromImageAsync(string imagePath)
    {
        // Utilisation de Tesseract OCR
        try
        {
            using var engine = new Tesseract.TesseractEngine("./tessdata", "fra+eng", Tesseract.EngineMode.Default);
            using var img = Tesseract.Pix.LoadFromFile(imagePath);
            using var page = engine.Process(img);
            return page.GetText();
        }
        catch (Exception ex)
        {
            return $"Erreur OCR: {ex.Message}";
        }
    }

    private async Task<string> CallGroqAPI(string prompt)
    {
        var requestBody = new
        {
            model = _configuration["Groq:Model"],
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.7,
            max_tokens = 2048
        };

        var json = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_configuration["Groq:Endpoint"], content);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return $"Erreur API Groq: {responseJson}";

        dynamic result = JsonConvert.DeserializeObject(responseJson)!;
        return result.choices[0].message.content;
    }
}