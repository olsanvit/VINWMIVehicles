using ApexCharts;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.SessionStorage;
using MercenariesAndBeasts.Infrastructure;
using MercenariesAndBeasts.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Services;
using SharedServices;
using SharedServices.Services;
using System.Security.Claims;
using VINWMIVehicles.Components;
using VINWMIVehicles.Data;
using VINWMIVehicles.Services;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30,
                  outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
var openAiKey = builder.Configuration["OpenAI:ApiKey"] ?? "";

// Identity DB (AppUser, roles, external logins)
builder.Services.AddDbContext<AppDbContextVin>(o => o.UseNpgsql(connectionString));

// Vehicle data DB
builder.Services.AddDbContextFactory<AppDbContextCar>(o => o.UseNpgsql(connectionString));

// Vehicle domain DB (VIN, WMI, manufacturers, brands, models)
builder.Services.AddDbContextFactory<AppDbContextVehicle>(o =>
    o.UseNpgsql(connectionString,
        npgsql => npgsql.CommandTimeout(60)));
builder.Services.AddScoped<AppDbContextVehicle>(sp =>
    sp.GetRequiredService<IDbContextFactory<AppDbContextVehicle>>().CreateDbContext());

// Identity + Google OAuth (Google keys optional — from appsettings.Development.json)
builder.Services.AddMabAuth<AppDbContextVin>(builder.Configuration);

// Shared UI services
builder.Services.AddScoped<AlertService>();
builder.Services.AddSingleton<ThemeService>(_ => new ThemeService(builder.Configuration));
builder.Services.AddBlazoredModal();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddApexCharts();

// Vehicle services
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<ErrorService<AppDbContextCar>>();
builder.Services.AddScoped<EfCoreService<AppDbContextCar>>();
builder.Services.AddScoped<ChatGPTWMI>();
builder.Services.AddScoped<ChatGptAsker>(_ => new ChatGptAsker(apiKey: openAiKey, isSimple: false));
builder.Services.AddHttpClient<NhtsaService>();
builder.Services.AddScoped<INhtsaService, NhtsaService>();
builder.Services.AddScoped<IVehicleSearchService, VehicleSearchService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(MercenariesAndBeasts.Infrastructure.Components.Account.Login).Assembly);

// ── Google OAuth external login endpoints ──────────────────────────────────

// 1) POST — kick off the Google challenge
app.MapPost("/Identity/Account/ExternalLogin", async (
    HttpContext http,
    SignInManager<AppUser> signInManager) =>
{
    var provider  = http.Request.Form["provider"].ToString();
    var returnUrl = http.Request.Form["returnUrl"].ToString() ?? "/";
    var callback  = $"/Identity/Account/ExternalLogin/Callback?returnUrl={Uri.EscapeDataString(returnUrl)}";
    var props     = signInManager.ConfigureExternalAuthenticationProperties(provider, callback);
    return Results.Challenge(props, new[] { provider });
}).DisableAntiforgery();

// 2) GET — handle the Google callback
app.MapGet("/Identity/Account/ExternalLogin/Callback", async (
    HttpContext http,
    string? returnUrl,
    SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager) =>
{
    returnUrl ??= "/";
    var info = await signInManager.GetExternalLoginInfoAsync();
    if (info is null)
        return Results.Redirect("/login?error=external");

    var signIn = await signInManager.ExternalLoginSignInAsync(
        info.LoginProvider, info.ProviderKey, isPersistent: true);

    if (signIn.Succeeded)
        return Results.Redirect(returnUrl);

    // First-time Google login — auto-create account
    var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
    if (string.IsNullOrWhiteSpace(email))
        return Results.Redirect("/login?error=noemail");

    var user = new AppUser { UserName = email, Email = email };
    var created = await userManager.CreateAsync(user);
    if (created.Succeeded)
    {
        await userManager.AddLoginAsync(user, info);
        await signInManager.SignInAsync(user, isPersistent: true);
        return Results.Redirect(returnUrl);
    }

    // User with that email already exists — link the login
    var existing = await userManager.FindByEmailAsync(email);
    if (existing is not null)
    {
        await userManager.AddLoginAsync(existing, info);
        await signInManager.SignInAsync(existing, isPersistent: true);
        return Results.Redirect(returnUrl);
    }

    return Results.Redirect("/login?error=external");
});

app.Run();
