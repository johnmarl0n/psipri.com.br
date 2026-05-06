using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using psipri.com.br.Data;
using psipri.com.br.Models;
using System.Diagnostics;

namespace psipri.com.br.Controllers
{
    /// <summary>
    /// Controller for the public-facing landing page.
    /// Handles data fetching for the About section and Blog preview.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Main landing page action.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Fetch dynamic About section
            var about = await _context.SiteContents.FirstOrDefaultAsync(c => c.Key == "AboutMe");
            ViewBag.AboutContent = about?.Value ?? "Conteúdo sobre a psicóloga em breve.";

            // Fetch Hero Image
            var hero = await _context.SiteContents.FirstOrDefaultAsync(c => c.Key == "HeroImage");
            ViewBag.HeroImage = hero?.Value;

            // Fetch latest 3 published blog posts
            var posts = await _context.BlogPosts
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.CreatedAt)
                .Take(3)
                .ToListAsync();

            return View(posts);
        }

        /// <summary>
        /// View a single blog post.
        /// </summary>
        public async Task<IActionResult> Post(int id)
        {
            var post = await _context.BlogPosts.FirstOrDefaultAsync(p => p.Id == id && p.IsPublished);
            if (post == null) return NotFound();
            return View(post);
        }

        /// <summary>
        /// Error page handler.
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
