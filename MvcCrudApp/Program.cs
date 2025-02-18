using Microsoft.EntityFrameworkCore;
using MvcCrudApp.Data;
using Microsoft.ApplicationInsights.Extensibility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var keyVaultUri = builder.Configuration["ConnectionStrings:DefaultConnection"];

// Configure Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(keyVaultUri));

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.Configure<TelemetryConfiguration>(config =>
{
    config.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
});

builder.Services.AddControllersWithViews();


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
