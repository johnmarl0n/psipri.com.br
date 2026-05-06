using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using psipri.com.br.Models;

namespace psipri.com.br.Data
{
    /// <summary>
    /// Database context for the application, integrating Identity for security.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Table for blog posts.
        /// </summary>
        public DbSet<BlogPost> BlogPosts { get; set; }

        /// <summary>
        /// Table for dynamic site content (e.g., "About" section).
        /// </summary>
        public DbSet<SiteContent> SiteContents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed initial site content if needed
            builder.Entity<SiteContent>().HasData(
                new SiteContent { Id = 1, Key = "AboutMe", Value = "<p>Sou uma psicóloga especializada em Psicologia Jurídica...</p>" }
            );
        }
    }
}
