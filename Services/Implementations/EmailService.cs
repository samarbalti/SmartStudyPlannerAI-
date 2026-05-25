using MailKit.Net.Smtp;
using MimeKit;
using SmartStudyPlannerAI.Services.Interfaces;

namespace SmartStudyPlannerAI.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Smart Study Planner", _configuration["Email:SenderEmail"]));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_configuration["Email:SmtpServer"], int.Parse(_configuration["Email:SmtpPort"]!), MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_configuration["Email:SenderEmail"], _configuration["Email:SenderPassword"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendVerificationEmailAsync(string to, string verificationLink)
    {
        var body = $@"
            <h2>Bienvenue sur Smart Study Planner AI !</h2>
            <p>Merci de vous être inscrit. Veuillez cliquer sur le lien ci-dessous pour vérifier votre email :</p>
            <a href='{verificationLink}' style='padding:10px 20px;background:#3498db;color:white;text-decoration:none;border-radius:5px;'>Vérifier mon email</a>
            <p>Si vous n'avez pas créé ce compte, ignorez cet email.</p>";

        await SendEmailAsync(to, "Vérification de votre email - Smart Study Planner", body);
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetLink)
    {
        var body = $@"
            <h2>Réinitialisation de mot de passe</h2>
            <p>Vous avez demandé une réinitialisation de mot de passe. Cliquez sur le lien ci-dessous :</p>
            <a href='{resetLink}' style='padding:10px 20px;background:#e74c3c;color:white;text-decoration:none;border-radius:5px;'>Réinitialiser mon mot de passe</a>
            <p>Ce lien expire dans 24 heures.</p>";

        await SendEmailAsync(to, "Réinitialisation de mot de passe - Smart Study Planner", body);
    }
}