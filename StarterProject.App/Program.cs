using JSNLog;
using Serilog;
using SimpleInjector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();

// setup simple injector
var container = new Container();
builder.Services.AddSimpleInjector(container, options =>
{
    // AddAspNetCore() wraps web requests in a Simple Injector scope and
    // allows request-scoped framework services to be resolved.
    options.AddAspNetCore()

        // Ensure activation of a specific framework type to be created by
        // Simple Injector instead of the built-in configuration system.
        // All calls are optional. You can enable what you need. For instance,
        // ViewComponents, PageModels, and TagHelpers are not needed when you
        // build a Web API.
        .AddControllerActivation()
        .AddViewComponentActivation()
        .AddPageModelActivation()
        .AddTagHelperActivation();
});

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

var componentSetup = new StarterProject.App.ComponentSetup(container);
componentSetup.RegisterComponents();

app.UseHttpsRedirection();

// Configure JSNLog
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
app.UseJSNLog(new LoggingAdapter(loggerFactory));

container.RegisterInstance(loggerFactory.CreateLogger(""));

app.Services.UseSimpleInjector(container);

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

container.Verify();

await app.RunAsync();
