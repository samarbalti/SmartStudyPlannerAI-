using Microsoft.AspNetCore.Mvc;
using SmartStudyPlannerAI.Models.DTOs;
using SmartStudyPlannerAI.Services.Interfaces;

namespace SmartStudyPlannerAI.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;

    public AccountController(IAuthService authService, IEmailService emailService)
    {
        _authService = authService;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var (user, token) = await _authService.LoginAsync(dto);
        if (user == null)
        {
            ModelState.AddModelError("", "Email ou mot de passe incorrect.");
            return View(dto);
        }

        Response.Cookies.Append("AuthToken", token!, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = DateTime.Now.AddDays(7)
        });

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var (success, message) = await _authService.RegisterAsync(dto);
        if (!success)
        {
            ModelState.AddModelError("", message);
            return View(dto);
        }

        var (user, token) = await _authService.LoginAsync(new LoginDTO
        {
            Email = dto.Email,
            Password = dto.Password
        });

        if (user != null && !string.IsNullOrEmpty(token))
        {
            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTime.Now.AddDays(7)
            });

            return RedirectToAction("Index", "Dashboard");
        }

        TempData["Success"] = message;
        return RedirectToAction("Login");
    }

    [HttpGet]
    public async Task<IActionResult> VerifyEmail(string token)
    {
        var success = await _authService.VerifyEmailAsync(token);
        TempData[success ? "Success" : "Error"] = success 
            ? "Email vérifié avec succès ! Vous pouvez maintenant vous connecter." 
            : "Token de vérification invalide.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        try
        {
            var success = await _authService.RequestPasswordResetOTPAsync(email);
            if (!success)
            {
                TempData["Error"] = "Aucun compte trouvé avec cet email.";
                return RedirectToAction("ForgotPassword");
            }

            TempData["Success"] = "Code de vérification envoyé à votre email.";
            TempData["Email"] = email;
            return RedirectToAction("VerifyOTP");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("ForgotPassword");
        }
    }

    [HttpGet]
    public IActionResult VerifyOTP()
    {
        var email = TempData["Email"]?.ToString();
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("ForgotPassword");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> VerifyOTP(string email, string otpCode)
    {
        var success = await _authService.VerifyPasswordResetOTPAsync(email, otpCode);
        if (!success)
        {
            ModelState.AddModelError("", "Code de vérification invalide ou expiré.");
            return View();
        }
        TempData["Success"] = "Code de vérification confirmé. Entrez votre nouveau mot de passe.";
        TempData["Email"] = email;
        return RedirectToAction("SetNewPassword");
    }

    [HttpGet]
    public IActionResult SetNewPassword()
    {
        var email = TempData["Email"]?.ToString();
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("ForgotPassword");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SetNewPassword(string email, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            ModelState.AddModelError("", "Les mots de passe ne correspondent pas.");
            return View();
        }

        var success = await _authService.ResetPasswordWithOTPAsync(email, newPassword);
        if (!success)
        {
            ModelState.AddModelError("", "Erreur lors de la réinitialisation du mot de passe.");
            return View();
        }
        TempData["Success"] = "Mot de passe réinitialisé avec succès ! Vous pouvez maintenant vous connecter.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        ViewBag.Token = token;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(string token, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            ModelState.AddModelError("", "Les mots de passe ne correspondent pas.");
            return View();
        }

        var success = await _authService.ResetPasswordAsync(token, newPassword);
        TempData[success ? "Success" : "Error"] = success 
            ? "Mot de passe réinitialisé avec succès !" 
            : "Token invalide ou expiré.";
        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken", new CookieOptions { Path = "/" });
        return RedirectToAction("Login");
    }
}