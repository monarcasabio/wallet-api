using WalletSystem.Core.Domain.Exceptions;

public class ExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try { await _next(context); }
        catch (Exception ex) when (ex is WalletNotFoundException or InsufficientBalanceException or DomainException)
        {
            var status = ex switch
            {
                WalletNotFoundException => StatusCodes.Status404NotFound,
                InsufficientBalanceException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status400BadRequest
            };

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";
            var problem = new { detail = ex.Message };
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}