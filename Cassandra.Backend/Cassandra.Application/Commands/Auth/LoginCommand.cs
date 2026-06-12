namespace Cassandra.Application.Commands.Auth;

public record LoginCommand(string Email, string Password);
