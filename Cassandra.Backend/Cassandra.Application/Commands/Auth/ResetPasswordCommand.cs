namespace Cassandra.Application.Commands.Auth;

public record ResetPasswordCommand(string Email, string Token, string NewPassword);
