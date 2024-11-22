using JKLHealthCare11810937.Hubs;
using JKLHealthCare11810937.Models;
using JKLHealthCare11810937.Services.Data;
using JKLHealthCare11810937.Services.Repository;
using JKLHealthCare11810937.Services.Security;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IKeyVaultService, KeyVaultService>();

builder.Services.AddDbContext<JKLHealthCareContext>(options =>
{
    var serviceProvider = builder.Services.BuildServiceProvider();
    var keyVaultService = serviceProvider.GetRequiredService<IKeyVaultService>();

    string connectionString = keyVaultService.GetSecret("DBConnectionString");
    options.UseSqlite(connectionString);
});
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IAdminSeedService, AdminSeedService>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IValidationService, ValidationService>();

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);

});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;

    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication("CookieAuthentication")
    .AddCookie("CookieAuthentication", options =>
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = "JKLHealthCareCookie";
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

    });
builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();

    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        var nonce = Guid.NewGuid().ToString();
        context.Response.Headers.Append("Content-Security-Policy", $"default-src 'self'; script-src 'self'; style-src 'self' 'nonce-{nonce}'; img-src 'self' data:; frame-ancestors 'none'; form-action 'self';");
        context.Items["CSPNonce"] = nonce;

        context.Response.Headers.Remove("X-Powered-By");
        context.Response.Headers.Remove("Server");

        await next();
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seedService = services.GetRequiredService<IAdminSeedService>();

    await seedService.SeedAdminUserAsync();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<AssignmentHub>("/assignmentHub");

app.UseSession();

app.Run();
