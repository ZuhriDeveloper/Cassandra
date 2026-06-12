using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Cassandra.WebUi.Services;

/// <summary>
/// Attaches the JWT access token to every outgoing API request. Works in both SSR (reads from
/// HttpContext) and Interactive Server (reads from AuthenticationStateProvider, since HttpContext
/// is null after the SignalR handshake). On <see cref="HttpStatusCode.Unauthorized"/>: navigates
/// to /account/session-expired which signs out the cookie and redirects to login.
/// <para>
/// Attach to an authenticated API client with
/// <c>.AddHttpMessageHandler&lt;BearerTokenHandler&gt;()</c> when business API clients are added.
/// </para>
/// </summary>
public class BearerTokenHandler(
    IHttpContextAccessor accessor,
    CircuitServicesAccessor circuitServices,
    IServiceProvider serviceProvider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        // 1. SSR path — HttpContext is available on the initial request
        var token = accessor.HttpContext?.User.FindFirstValue("access_token");

        // 2. Interactive Server path — HttpContext is null after SignalR handshake.
        if (string.IsNullOrEmpty(token))
        {
            var provider = circuitServices.Services ?? serviceProvider;
            var authProvider = provider.GetService<AuthenticationStateProvider>();
            if (authProvider is not null)
            {
                var state = await authProvider.GetAuthenticationStateAsync();
                token = state.User.FindFirstValue("access_token");
            }
        }

        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, ct);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var nav = serviceProvider.GetService<NavigationManager>();
            nav?.NavigateTo("/account/session-expired", forceLoad: true);
        }

        return response;
    }
}
