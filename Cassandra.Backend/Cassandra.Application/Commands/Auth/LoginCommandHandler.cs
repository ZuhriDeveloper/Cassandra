using Cassandra.Application.Contracts.Auth;
using Cassandra.Application.Contracts.Dealers;

namespace Cassandra.Application.Commands.Auth;

public class LoginCommandHandler(
    IUserAuthRepository userRepository,
    ITokenService tokenService,
    IDealerQueryRepository dealerQueryRepository)
{
    public async Task<LoginResult> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.FindByEmailAsync(command.Email, cancellationToken);
        if (user is null)
            return LoginResult.Fail("Invalid email or password.");

        if (await userRepository.IsLockedOutAsync(user.Id))
            return LoginResult.Fail("Account is temporarily locked. Please try again later.");

        var passwordValid = await userRepository.CheckPasswordAsync(user.Id, command.Password);
        if (!passwordValid)
        {
            await userRepository.RecordFailedAccessAsync(user.Id);
            return LoginResult.Fail("Invalid email or password.");
        }

        // A dealer-scoped user whose dealer has been deactivated may not sign in. A SuperAdmin
        // (no dealer) is unaffected. Checked after password verification so dealer status is
        // never revealed to an unauthenticated guess.
        if (user.DealerId is Guid dealerId)
        {
            var dealer = await dealerQueryRepository.GetByIdAsync(dealerId, cancellationToken);
            if (dealer is null || !dealer.IsActive)
                return LoginResult.Fail("Your dealer account is inactive. Please contact your administrator.");
        }

        await userRepository.ResetAccessFailedCountAsync(user.Id);

        var roles = await userRepository.GetRolesAsync(user.Id, cancellationToken);
        var token = tokenService.GenerateToken(user, roles);
        return LoginResult.Ok(token, user.Email, user.FullName, roles, user.DealerId);
    }
}
