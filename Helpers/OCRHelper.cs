namespace SmartStudyPlannerAI.Helpers;

public static class OCRHelper
{
    public static string CleanExtractedText(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        return text
            .Replace("\r\n", "\n")
            .Replace("\n\n", "\n")
            .Replace("  ", " ")
            .Trim();
    }

    public static List<string> ExtractKeySentences(string text, int maxSentences = 5)
    {
        var sentences = text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => s.Length > 20)
            .OrderByDescending(s => s.Length)
            .Take(maxSentences)
            .ToList();
        return sentences;
    }
}