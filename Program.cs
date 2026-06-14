using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using psipri.com.br.Data;
using psipri.com.br.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Configuration and Services ---

// Register Email Service
builder.Services.AddTransient<IEmailService, EmailService>();

// Register Pingo de Mel Stock Closing Background Job
builder.Services.AddHostedService<PDMStockClosingJob>();

// Configure Database connection (SQL Server)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

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

// Configure Cookies for the temporary URL (allowing HTTP)
builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Configure Antiforgery to work with the temporary URL
builder.Services.AddAntiforgery(options => {
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// --- Automatic Migrations and Seeding ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        // Seed Default User
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var adminEmail = "priscilabatista.dias@uol.com.br";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new IdentityUser { 
                UserName = adminEmail, 
                Email = adminEmail, 
                EmailConfirmed = true 
            };
            await userManager.CreateAsync(adminUser, "Pri@2024!");
        }
        else
        {
            // Update password to match the user's request
            var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
            await userManager.ResetPasswordAsync(adminUser, token, "Pri@2024!");
        }

        // Ensure ONLY this user exists (Apenas 1 usuário)
        var otherUsers = userManager.Users.Where(u => u.Email != adminEmail).ToList();
        foreach (var user in otherUsers)
        {
            await userManager.DeleteAsync(user);
        }

        // Update HeroImage to the new profile photo
        var heroSetting = await context.SiteContents.FirstOrDefaultAsync(c => c.Key == "HeroImage");
        if (heroSetting == null)
        {
            heroSetting = new psipri.com.br.Models.SiteContent { Key = "HeroImage", Value = "/uploads/hero_priscila.jpg" };
            context.SiteContents.Add(heroSetting);
        }
        else
        {
            heroSetting.Value = "/uploads/hero_priscila.jpg";
            context.SiteContents.Update(heroSetting);
        }
        await context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// --- HTTP Request Pipeline ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// app.UseHttpsRedirection();
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
