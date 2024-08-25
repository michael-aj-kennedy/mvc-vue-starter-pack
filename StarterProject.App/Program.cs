using StarterProject.Shared.Configuration;
using StarterProject.Shared.Extensions;
using JSNLog;
using Serilog;
using SimpleInjector;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

//Add support to logging request with SERILOG
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// configuration
builder.Configuration
    .AddJsonFile("appsettings.json", true, true)
    .AddEnvironmentVariables();

var container = new Container();
var componentSetup = new StarterProject.App.ComponentSetup(container);
componentSetup.RegisterComponents();

app.UseHttpsRedirection();

// Configure JSNLog
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
app.UseJSNLog(new LoggingAdapter(loggerFactory));

container.RegisterInstance(loggerFactory.CreateLogger(""));

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
