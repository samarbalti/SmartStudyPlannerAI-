namespace SmartStudyPlannerAI.Helpers;

public static class EmailHelper
{
    public static string GenerateEmailTemplate(string title, string content, string? actionLink = null, string? actionText = null)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background: #f4f4f4; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; color: white; }}
        .content {{ padding: 30px; }}
        .button {{ display: inline-block; padding: 12px 30px; background: #667eea; color: white; text-decoration: none; border-radius: 25px; margin: 20px 0; }}
        .footer {{ background: #f8f9fa; padding: 20px; text-align: center; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>📚 Smart Study Planner AI</h1>
        </div>
        <div class='content'>
            <h2>{title}</h2>
            <p>{content}</p>
            {(actionLink != null ? $"<a href='{actionLink}' class='button'>{actionText}</a>" : "")}
        </div>
        <div class='footer'>
            <p>© 2026 Smart Study Planner AI. Tous droits réservés.</p>
        </div>
    </div>
</body>
</html>";
    }
}