using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Cassandra.Web.Services;

public class JwtAuthStateProvider(LocalStorageService localStorage) : AuthenticationStateProvider
{
    private const string TokenKey = "authToken";
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await localStorage.GetItemAsync(TokenKey);
            if (string.IsNullOrWhiteSpace(token))
                return Anonymous;

            var principal = ParseToken(token);
            return principal is null ? Anonymous : new AuthenticationState(principal);
        }
        catch
        {
            return Anonymous;
        }
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await localStorage.SetItemAsync(TokenKey, token);
        var principal = ParseToken(token);
        if (principal is not null)
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await localStorage.RemoveItemAsync(TokenKey);
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    private static ClaimsPrincipal? ParseToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
                return null;

            var jwt = handler.ReadJwtToken(token);
            if (jwt.ValidTo < DateTime.UtcNow)
                return null;

            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            return new ClaimsPrincipal(identity);
        }
        catch
        {
            return null;
        }
    }
}

