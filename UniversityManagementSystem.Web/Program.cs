using UniversityManagementSystem.Application.Extensions;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.Services;
using UniversityManagementSystem.Infrastructure.Extensions;
using UniversityManagementSystem.Infrastructure.Data;
using UniversityManagementSystem.Web.Hubs;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using System.IO;
using System.Net;
using System.Net.Sockets;

var builder = WebApplication.CreateBuilder(args);

// Dev convenience: if a configured port is already in use (e.g. a leftover
// instance from a previous run), automatically fall back to the next free
// port instead of crashing at startup.
var configuredUrls = builder.Configuration["ASPNETCORE_URLS"];
if (!string.IsNullOrWhiteSpace(configuredUrls))
{
    var finalUrls = new List<string>();
    foreach (var url in configuredUrls.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    {
        var freeUrl = EnsurePortIsFree(url);
        if (freeUrl != url)
        {
            Console.WriteLine($"WARNING: {url} is already in use.");
            Console.WriteLine($"         Falling back to {freeUrl}.");
        }
        finalUrls.Add(freeUrl);
    }
    builder.WebHost.UseUrls(finalUrls.ToArray());
}

// Configure EPPlus license (free for non-commercial use) BEFORE any ExcelPackage is created.
// EPPlus 8+ uses the static ExcelPackage.License property.
// - Personal use:  ExcelPackage.License.SetNonCommercialPersonal("Your Name")
// - Organization:  ExcelPackage.License.SetNonCommercialOrganization("Org Name")
// - Commercial:    ExcelPackage.License.SetCommercial("your-license-key")
// See https://epplussoftware.com/developers/licensenotsetexception
ExcelPackage.License.SetNonCommercialPersonal("University Management System");

// Add services to the container.
builder.Services.AddControllersWithViews();

// Infrastructure (DbContext, Identity, Repositories)
builder.Services.AddInfrastructure(builder.Configuration);

// Application (AutoMapper, Services)
builder.Services.AddApplication();

// SignalR for real-time communication
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificationBroadcaster, SignalRNotificationBroadcaster>();

// HttpContextAccessor for user context
builder.Services.AddHttpContextAccessor();

// User Management Service
builder.Services.AddScoped<IUserManagementService, UserManagementService>();

// Permission Service for authorization
builder.Services.AddScoped<UniversityManagementSystem.Web.Authorization.IPermissionService, UniversityManagementSystem.Web.Authorization.PermissionService>();

// File upload configuration (10 MB limit)
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
    options.MemoryBufferThreshold = int.MaxValue;
});

var app = builder.Build();

// Keep the runtime database schema in sync with the application. This is
// especially important for audit logging because writes must never target an
// older AuditLogs schema after a deployment.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Serve static files from wwwroot
app.UseStaticFiles();

// Serve uploads folder directly (for profile pictures, assignment files, etc.)
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map SignalR hubs
app.MapHub<ChatHub>("/chatHub");
app.MapHub<NotificationHub>("/notificationHub");

// Start the app. If the configured port is already in use (e.g. a leftover
// instance from a previous run), fail with a clear, actionable message
// instead of the cryptic Kestrel bind exception.
try
{
    app.Run();
}
catch (IOException ex) when (ex.Message.Contains("address already in use", StringComparison.OrdinalIgnoreCase))
{
    Console.Error.WriteLine();
    Console.Error.WriteLine("======================================================");
    Console.Error.WriteLine("  PORT ALREADY IN USE");
    Console.Error.WriteLine($"  {ex.Message}");
    Console.Error.WriteLine("  Close the other running instance, or free the port by running:");
    Console.Error.WriteLine("      kill-server.bat   (Windows cmd)");
    Console.Error.WriteLine("      ./kill-server.sh  (git-bash)");
    Console.Error.WriteLine("  Then start the app again.");
    Console.Error.WriteLine("======================================================");
    Environment.ExitCode = 1;
}

// --- Helpers for automatic port fallback ---

static bool IsPortInUse(int port)
{
    try
    {
        using var listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();
        return false;
    }
    catch (SocketException)
    {
        return true;
    }
}

static string EnsurePortIsFree(string url)
{
    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Port <= 0)
        return url;

    if (!IsPortInUse(uri.Port))
        return url;

    // Try the next 50 ports before giving up.
    for (int port = uri.Port + 1; port < uri.Port + 51; port++)
    {
        if (!IsPortInUse(port))
        {
            return new UriBuilder(uri) { Port = port }.Uri.ToString();
        }
    }

    return url; // Give up; the try/catch around app.Run() will surface the error.
}
