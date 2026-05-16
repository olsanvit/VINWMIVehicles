using ApexCharts;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.SessionStorage;
using MercenariesAndBeasts.Infrastructure;
using MercenariesAndBeasts.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using Services;
using SharedServices;
using SharedServices.Services;
using System.Security.Claims;
using VINWMIVehicles.Components;
using VINWMIVehicles.Services;

var builder = WebApplication.CreateBuilder(args);

Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Logs"));
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console(
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        shared: true,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .WriteTo.PostgreSQL(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
        tableName: "Logs",
        columnOptions: (IDictionary<string, ColumnWriterBase>?)null,
        needAutoCreateTable: true,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();

builder.Services.AddControllers();

var openAiKey = builder.Configuration["OpenAI:ApiKey"] ?? "";

// Vehicle + Identity DB — NpgsqlDataSourceBuilder (EnableDynamicJson + retry)
var cs = "";
#if DEBUG
cs = builder.Configuration.GetConnectionString("DefaultConnectionQNAP");
#else
cs = builder.Configuration.GetConnectionString("DefaultConnection");
#endif
var dsb = new NpgsqlDataSourceBuilder(cs);
dsb.EnableDynamicJson();
var dataSource = dsb.Build();

// AddMabDbContext = AddDbContextFactory + scoped AddDbContext (Identity potřebuje scoped)
builder.Services.AddMabDbContext<AppDbContextVehicle>(dataSource);

// Identity + Google OAuth (Google keys optional — z appsettings.Development.json)
builder.Services.AddMabAuth<AppDbContextVehicle>(builder.Configuration);

// Identity UI vyžaduje IEmailSender — no-op implementace (maily zatím neposíláme)
builder.Services.AddSingleton<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender,
    NoOpEmailSender>();

// Shared UI services
builder.Services.AddScoped<AlertService>();
builder.Services.AddSingleton<ThemeService>(_ => new ThemeService(builder.Configuration));
builder.Services.AddBlazoredModal();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddApexCharts();

// Vehicle services
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<ErrorService<AppDbContextVehicle>>();
builder.Services.AddScoped<EfCoreService<AppDbContextVehicle>>();
builder.Services.AddScoped<ChatGPTWMI>();
builder.Services.AddScoped<ChatGptAsker>(_ => new ChatGptAsker(apiKey: openAiKey, isSimple: false));
// Typed HTTP client — konfiguruje HttpClient a registruje NhtsaService jako INhtsaService v jednom
builder.Services.AddHttpClient<INhtsaService, NhtsaService>();
builder.Services.AddScoped<IVehicleSearchService, VehicleSearchService>();

// VIN Randomizer
builder.Services.AddScoped<VinRandomizerService>();
builder.Services.AddHostedService<VinRandomizerHostedService>();

builder.Services.AddHttpContextAccessor();

AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
    Log.Fatal(e.ExceptionObject as Exception, "UNHANDLED AppDomain exception");

TaskScheduler.UnobservedTaskException += (sender, e) =>
{
    Log.Fatal(e.Exception, "UNOBSERVED task exception");
    e.SetObserved();
};

var app = builder.Build();

if (string.IsNullOrWhiteSpace(builder.Configuration["OpenAI:ApiKey"]))
    Log.Warning("VINWMIVehicles: OpenAI ApiKey is not configured — AI features will fail");
if (string.IsNullOrWhiteSpace(builder.Configuration["Authentication:Google:ClientId"]))
    Log.Warning("VINWMIVehicles: Google OAuth ClientId is not configured — Google login will fail");

var pathBase = builder.Configuration["PathBase"];
if (!string.IsNullOrWhiteSpace(pathBase))
    app.UsePathBase(pathBase);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

if (!app.Environment.IsProduction())
    app.UseHttpsRedirection();

app.MapStaticAssets();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorPages();   // Identity UI scaffolded pages

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(MercenariesAndBeasts.Infrastructure.Components.Account.Login).Assembly);

app.MapControllers();

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

// ── Migrate DB + Seed admin ────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services    = scope.ServiceProvider;
    var db          = services.GetRequiredService<AppDbContextVehicle>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await db.Database.MigrateAsync();
    await SeedAdminAsync(userManager, roleManager);
}

app.Lifetime.ApplicationStopping.Register(() =>
    Log.Warning("Application stopping — flushing logs..."));

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// ── Seed helpers ──────────────────────────────────────────────────────────
static async Task SeedAdminAsync(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager)
{
    const string adminRole = "Admin";

    if (!await roleManager.RoleExistsAsync(adminRole))
        await roleManager.CreateAsync(new IdentityRole(adminRole));

    await EnsureAdminAsync(userManager, adminRole,
        email: "admin@local",
        username: "admin",
        password: "Admin123.");

    await EnsureAdminAsync(userManager, adminRole,
        email: "olsanskyvitek@gmail.com",
        username: "vitek",
        password: "Vitek575");
}

static async Task EnsureAdminAsync(
    UserManager<AppUser> userManager,
    string adminRole,
    string email,
    string username,
    string password)
{
    var user = await userManager.FindByEmailAsync(email);

    if (user is null)
    {
        user = new AppUser
        {
            UserName = username,
            Email = email,
            EmailConfirmed = true,
            IsAdmin = true
        };
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new Exception($"Failed to create user {email}: " +
                string.Join(", ", result.Errors.Select(e => e.Description)));
    }
    else if (!user.IsAdmin)
    {
        user.IsAdmin = true;
        await userManager.UpdateAsync(user);
    }

    if (!await userManager.IsInRoleAsync(user, adminRole))
        await userManager.AddToRoleAsync(user, adminRole);
}

// ── No-op IEmailSender ────────────────────────────────────────────────────
file sealed class NoOpEmailSender : Microsoft.AspNetCore.Identity.UI.Services.IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
        => Task.CompletedTask;
}
