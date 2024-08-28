using JSNLog;
using Serilog;
using SimpleInjector;
using StarterProject.App.Infrastructure;

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

// configuration
builder.Configuration
    .AddJsonFile("appsettings.json", true, true)
    .AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Add support to logging request with SERILOG
app.UseSerilogRequestLogging();

// Configure JSNLog
var jsnlogConfiguration = new JsnlogConfiguration
{
    ajaxAppenders = new List<AjaxAppender> {
            new AjaxAppender {
                name = "appender1",
                storeInBufferLevel = "TRACE", // Log messages with severity smaller than TRACE are ignored
                level = "WARN", // Log messages with severity equal or greater than TRACE and lower than WARN are stored in the internal buffer, but not sent to the server
                                // Log messages with severity equal or greater than WARN and lower than FATAL are sent to the server on their own
                sendWithBufferLevel = "FATAL", // Log messages with severity equal or greater than FATAL are sent to the server, along with all messages stored in the internal buffer
                bufferSize = 20, // Stores the last up to 20 debug messages in browser memory,
                batchSize = 20,
                batchTimeout = 2000, // Logs are guaranteed to be sent within this period (in ms), even if the batch size has not been reached yet
                maxBatchSize = 50 // When the server is unreachable and log messages are being stored until it is reachable again, this is the maximum number of messages that will be stored. Cannot be smaller than batchSize
            }
        },
    loggers = new List<Logger> { // Get the loggers to use the new appender
            new Logger {
                appenders = "appender1"
            }
        },
    insertJsnlogInHtmlResponses = false, // There's an outstanding bug setting this to true so the workaround is using the jl-javascript-logger-definitions tag helper in _Layout.cshtml, via the reference in _ViewImports.cshtml
    productionLibraryPath = null // We're using a fallback from the CDN with hashing, so do not use this
};
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
app.UseJSNLog(new CustomLoggingAdapter(loggerFactory), jsnlogConfiguration);

var componentSetup = new StarterProject.App.ComponentSetup(container);
componentSetup.RegisterComponents();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.Services.UseSimpleInjector(container);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

container.Verify();

await app.RunAsync();
