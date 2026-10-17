using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using System.Threading.RateLimiting;
using psipri.com.br.Data;
using psipri.com.br.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Configuration and Services ---

// Configure Forwarded Headers for Cloudflare and reverse proxies (IIS ARR)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Register Email Service
builder.Services.AddTransient<IEmailService, EmailService>();

// Register Pingo de Mel Stock Closing Background Job
builder.Services.AddHostedService<PDMStockClosingJob>();

// Configure Database connection (SQL Server)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure Identity for secure authentication with brute-force lockout
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;

    // Anti-Brute Force Protection
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Configure Cookies for session management (SameAsRequest avoids SSL-termination mismatches on reverse proxy/Cloudflare)
builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Configure Antiforgery protection (SameAsRequest ensures forms render when SSL is terminated at proxy/Cloudflare)
builder.Services.AddAntiforgery(options => {
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Configure Native Rate Limiter to protect against Brute Force & DoS
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Login rate limiter: Max 5 attempts per minute per IP address
    options.AddPolicy("LoginLimiter", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    // Contact form rate limiter: Max 3 submissions per 5 minutes per IP address
    options.AddPolicy("ContactLimiter", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(5),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
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

        // Seed Default User (only if not already created)
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
            var initialPass = builder.Configuration["AdminSeed:InitialPassword"] ?? "Pri@2024!";
            await userManager.CreateAsync(adminUser, initialPass);
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
        else if (string.IsNullOrWhiteSpace(heroSetting.Value))
        {
            heroSetting.Value = "/uploads/hero_priscila.jpg";
            context.SiteContents.Update(heroSetting);
        }

        // Seed / Update AboutMe with complete professional biography and credentials
        var aboutSetting = await context.SiteContents.FirstOrDefaultAsync(c => c.Key == "AboutMe");
        string defaultAboutContent = @"<div class=""about-bio-lead"">
    <h3 class=""serif"" style=""color: var(--primary-color); font-size: 1.8rem; margin-bottom: 18px; line-height: 1.3;"">
        Acolhimento empático com sólido rigor técnico e científico
    </h3>
    <p style=""font-size: 1.05rem; line-height: 1.8; color: var(--text-dark); margin-bottom: 18px;"">
        Sou <strong>Priscila Batista</strong> (CRP 06/159741), psicóloga clínica e jurídica. Minha prática profissional é fundamentada no compromisso ético de oferecer um espaço seguro, acolhedor e livre de julgamentos, onde cada pessoa possa compreender suas dores, ressignificar conflitos e desenvolver autonomia emocional.
    </p>
    <p style=""font-size: 1rem; line-height: 1.75; color: var(--text-muted); margin-bottom: 25px;"">
        Na clínica individual, atuo com jovens e adultos no enfrentamento da ansiedade, depressão, síndrome do pânico, sobrecarga cotidiana e transições de vida. Paralelamente, como <strong>Assistente Técnica Pericial</strong>, atuo em demandas do Poder Judiciário, realizando perícias, formulação de quesitos e emissão de laudos técnicos pautados com rigor pelas resoluções do Conselho Federal de Psicologia.
    </p>
</div>

<div class=""credentials-grid"">
    <div class=""credential-item"">
        <div class=""credential-icon""><i class=""fas fa-id-card""></i></div>
        <div>
            <strong>CRP 06/159741</strong>
            <span>Registro profissional ativo no Conselho Regional de Psicologia de São Paulo (6ª Região).</span>
        </div>
    </div>
    <div class=""credential-item"">
        <div class=""credential-icon""><i class=""fas fa-brain""></i></div>
        <div>
            <strong>Psicoterapia Clínica</strong>
            <span>Atendimento para jovens e adultos com práticas baseadas em evidências científicas.</span>
        </div>
    </div>
    <div class=""credential-item"">
        <div class=""credential-icon""><i class=""fas fa-balance-scale""></i></div>
        <div>
            <strong>Psicologia Jurídica & Perícia</strong>
            <span>Assistência técnica judicial, elaboração de quesitos e pareceres fundamentados.</span>
        </div>
    </div>
    <div class=""credential-item"">
        <div class=""credential-icon""><i class=""fas fa-laptop-medical""></i></div>
        <div>
            <strong>Presencial & Online</strong>
            <span>Consultório acolhedor em Campinas e teleconsultas seguras para todo o Brasil.</span>
        </div>
    </div>
</div>";

        if (aboutSetting == null)
        {
            aboutSetting = new psipri.com.br.Models.SiteContent { Key = "AboutMe", Value = defaultAboutContent };
            context.SiteContents.Add(aboutSetting);
        }
        else if (string.IsNullOrWhiteSpace(aboutSetting.Value) || !aboutSetting.Value.Contains("06/159741"))
        {
            aboutSetting.Value = defaultAboutContent;
            context.SiteContents.Update(aboutSetting);
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

// Process forwarded headers from reverse proxy (Cloudflare / IIS ARR) first
app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Security Headers Middleware (Clickjacking, MIME Sniffing, Referrer Policy)
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
    await next();
});

app.UseStaticFiles();

app.UseRouting();

// Rate Limiting Middleware
app.UseRateLimiter();

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

// Completely block all access to self-registration
app.Map("/Identity/Account/Register", context => {
    context.Response.Redirect("/Identity/Account/Login");
    return Task.CompletedTask;
});

app.Run();
