using Microsoft.EntityFrameworkCore;
using MvcCrudApp.Data;
using Microsoft.ApplicationInsights.Extensibility;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Configure Key Vault for production
if (!builder.Environment.IsDevelopment())
{
    var keyVaultEndpoint = new Uri("https://demowebmvc-keyvault.vault.azure.net/");
    builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
}

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddApplicationInsightsTelemetry();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddApplicationInsights();

builder.Services.Configure<TelemetryConfiguration>(config =>
{
    config.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=Index}/{id?}");

app.Run();
