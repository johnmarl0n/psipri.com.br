using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using psipri.com.br.Data;
using psipri.com.br.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Configuration and Services ---

// Register Email Service
builder.Services.AddTransient<IEmailService, EmailService>();

// Configure Database connection (SQLite)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Configure Identity for secure authentication
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Configure CSRF (Antiforgery)
builder.Services.AddAntiforgery(options => {
    options.HeaderName = "X-CSRF-TOKEN";
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// --- HTTP Request Pipeline ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication and Authorization are critical for the maintenance area
app.UseAuthentication();
app.UseAuthorization();

// Post-login redirect: If user is logged in and tries to access the login page, go to Admin.
app.Use(async (context, next) => {
    if (context.Request.Path.StartsWithSegments("/Identity/Account/Login") && 
        context.User.Identity?.IsAuthenticated == true)
    {
        context.Response.Redirect("/Admin");
        return;
    }
    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Identity Razor Pages (for login/logout)
app.MapRazorPages();

app.MapGet("/Identity/Account/Register", context => {
    context.Response.Redirect("/Identity/Account/Login");
    return Task.CompletedTask;
});

app.Run();
