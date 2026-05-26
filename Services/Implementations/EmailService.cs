using SmartStudyPlannerAI.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Text.Json;

namespace SmartStudyPlannerAI.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _smtpUsername = _configuration["Mailtrap:SmtpUsername"] ?? string.Empty;
        _smtpPassword = _configuration["Mailtrap:SmtpPassword"] ?? string.Empty;
        _smtpHost = _configuration["Mailtrap:SmtpHost"] ?? "sandbox.smtp.mailtrap.io";
        _smtpPort = int.TryParse(_configuration["Mailtrap:SmtpPort"], out var p) ? p : 587;
        _fromEmail = _configuration["Mailtrap:FromEmail"] ?? "no-reply@example.com";
        _fromName = _configuration["Mailtrap:FromName"] ?? "Smart Study Planner";
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(_smtpUsername) || string.IsNullOrWhiteSpace(_smtpPassword))
            throw new InvalidOperationException("Mailtrap SMTP credentials are missing from configuration (Mailtrap:SmtpUsername / Mailtrap:SmtpPassword).");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_fromName, _fromEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        message.Body = new TextPart("plain")
        {
            Text = body
        };

        using var client = new SmtpClient();
        try
        {
            var secureOption = _smtpPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable;
            await client.ConnectAsync(_smtpHost, _smtpPort, secureOption);
            await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"SMTP send failed: {ex.Message}", ex);
        }
    }

    public async Task SendVerificationEmailAsync(string to, string verificationLink)
    {
        var body = $"Bienvenue sur Smart Study Planner AI !\n\nMerci de vous être inscrit. Veuillez vérifier votre email en utilisant ce lien :\n{verificationLink}\n\nSi vous n'avez pas créé ce compte, ignorez cet email.";

        await SendEmailAsync(to, "Vérification de votre email - Smart Study Planner", body);
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetLink)
    {
        var body = $"Réinitialisation de mot de passe\n\nVous avez demandé une réinitialisation de mot de passe. Utilisez ce lien :\n{resetLink}\n\nCe lien expire dans 24 heures.";

        await SendEmailAsync(to, "Réinitialisation de mot de passe - Smart Study Planner", body);
    }

    public async Task SendOTPEmailAsync(string to, string otpCode)
    {
        var body = $"Code de vérification\n\nUtilisez ce code pour réinitialiser votre mot de passe : {otpCode}\n\nCe code expire dans 10 minutes.\n\nNe partagez jamais ce code avec quelqu'un d'autre.";

        await SendEmailAsync(to, "Code de vérification - Smart Study Planner", body);
    }
}