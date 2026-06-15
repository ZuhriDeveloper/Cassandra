using System.Security.Claims;
using Cassandra.WebUi;
using Cassandra.WebUi.Components;
using Cassandra.WebUi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMudServices();
builder.Services.AddScoped<ThemeState>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

// Lets BearerTokenHandler reach the circuit-scoped AuthenticationStateProvider from the
// IHttpClientFactory handler scope, so the JWT is attached on interactive (SignalR) requests.
builder.Services.AddCircuitServicesAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var apiBaseUrl = builder.Configuration["ApiService:BaseUrl"] ?? "https://localhost:7072";

builder.Services.AddHttpClient<AuthService>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));

// Ready for authenticated API clients: register them with
//   builder.Services.AddHttpClient<MyApiClient>(...).AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddTransient<BearerTokenHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

app.MapPost("/account/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).RequireAuthorization();

// Login form posts here — handles HttpContext.SignInAsync which requires a real HTTP request.
// DisableAntiforgery: token consistency is tricky across pre-render / circuit boundary;
// login CSRF risk is acceptable for an internal app.
app.MapPost("/account/login-handler", async (
    HttpContext ctx,
    IFormCollection form,
    AuthService auth) =>
{
    var email     = form["email"].ToString();
    var password  = form["password"].ToString();
    var returnUrl = form["returnUrl"].ToString();

    var result = await auth.LoginAsync(email, password);
    if (!result.Succeeded)
    {
        var msg = Uri.EscapeDataString(result.ErrorMessage ?? "Email atau kata sandi salah.");
        return Results.Redirect($"/login?error={msg}");
    }

    var data = result.Data!;
    var claims = new List<Claim>
    {
        new(ClaimTypes.Email, data.Email),
        new(ClaimTypes.Name, data.FullName ?? data.Email),
        new("access_token", data.Token),
    };
    claims.AddRange(data.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);
    await ctx.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal,
        new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) });

    // Validate returnUrl is a local relative path before redirecting
    if (!string.IsNullOrWhiteSpace(returnUrl)
        && returnUrl.StartsWith('/')
        && !returnUrl.StartsWith("//")
        && Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
        return Results.Redirect(returnUrl);

    return Results.Redirect("/");
}).DisableAntiforgery();

// Called by BearerTokenHandler when the API returns 401 (token expired / invalid).
app.MapGet("/account/session-expired", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login?expired=1");
});

app.MapDefaultEndpoints();

app.Run();
