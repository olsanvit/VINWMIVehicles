using Microsoft.EntityFrameworkCore;
using Services;
using SharedServices;
using SharedServices.Services;
using VINWMIVehicles.Components;
using VINWMIVehicles.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
var openAiKey = builder.Configuration["OpenAI:ApiKey"] ?? "";

builder.Services.AddDbContextFactory<AppDbContextCar>(o =>
    o.UseNpgsql(connectionString));

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
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
