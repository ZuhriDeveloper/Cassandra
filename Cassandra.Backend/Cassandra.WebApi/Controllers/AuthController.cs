using System.Security.Claims;
using Cassandra.Application.Commands.Auth;
using Cassandra.WebApi.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Cassandra.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting(AuthRateLimiting.GeneralPolicy)]
public class AuthController(
    LoginCommandHandler handler,
    IValidator<LoginCommand> validator,
    ChangePasswordCommandHandler changePasswordHandler,
    IValidator<ChangePasswordCommand> changePasswordValidator,
    ForgotPasswordCommandHandler forgotPasswordHandler,
    IValidator<ForgotPasswordCommand> forgotPasswordValidator,
    ResetPasswordCommandHandler resetPasswordHandler,
    IValidator<ResetPasswordCommand> resetPasswordValidator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var result = await handler.HandleAsync(command, cancellationToken);

        if (!result.Succeeded)
            return Unauthorized(new { message = result.ErrorMessage });

        return Ok(new { result.Token, result.Email, result.FullName, result.Roles, result.DealerId });
    }

    /// <summary>Changes the signed-in user's password. The user id is taken from the JWT, never the body.</summary>
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var command = new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword);

        var validation = await changePasswordValidator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var result = await changePasswordHandler.HandleAsync(command, cancellationToken);
        return result.Succeeded
            ? Ok(new { message = "Kata sandi berhasil diubah." })
            : BadRequest(new { errors = result.Errors });
    }

    /// <summary>
    /// Requests a password-reset email. Always responds 200 with the same generic message
    /// regardless of whether the email is registered (no account enumeration).
    /// </summary>
    [AllowAnonymous]
    [EnableRateLimiting(AuthRateLimiting.EmailPolicy)]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var command = new ForgotPasswordCommand(request.Email);

        var validation = await forgotPasswordValidator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await forgotPasswordHandler.HandleAsync(command, cancellationToken);

        return Ok(new { message = "Jika email terdaftar, tautan atur ulang kata sandi telah dikirim." });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var command = new ResetPasswordCommand(request.Email, request.Token, request.NewPassword);

        var validation = await resetPasswordValidator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var result = await resetPasswordHandler.HandleAsync(command, cancellationToken);
        return result.Succeeded
            ? Ok(new { message = "Kata sandi berhasil diatur ulang. Silakan masuk." })
            : BadRequest(new { errors = result.Errors });
    }
}

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Token, string NewPassword);
