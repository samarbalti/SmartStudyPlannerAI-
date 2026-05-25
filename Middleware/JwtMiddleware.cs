using SmartStudyPlannerAI.Helpers;

namespace SmartStudyPlannerAI.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = JwtHelper.GetUserIdFromToken(context);
        if (userId.HasValue)
        {
            context.Items["UserId"] = userId.Value;
        }

        await _next(context);
    }
}