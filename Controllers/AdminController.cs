using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using psipri.com.br.Data;
using psipri.com.br.Models;
using System.Threading.Tasks;

namespace psipri.com.br.Controllers
{
    /// <summary>
    /// Controller for the administrative maintenance area.
    /// Access is restricted to authenticated users.
    /// </summary>
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Administrative dashboard overview.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var heroImage = await _context.SiteContents.FirstOrDefaultAsync(c => c.Key == "HeroImage");
            ViewBag.HeroImage = heroImage?.Value ?? "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?auto=format&fit=crop&q=80&w=800";
            return View();
        }

        /// <summary>
        /// Handles hero image upload.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadHero(IFormFile heroFile)
        {
            if (heroFile != null && heroFile.Length > 0)
            {
                var fileName = "hero_" + DateTime.Now.Ticks + Path.GetExtension(heroFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await heroFile.CopyToAsync(stream);
                }

                var heroSetting = await _context.SiteContents.FirstOrDefaultAsync(c => c.Key == "HeroImage");
                if (heroSetting == null)
                {
                    heroSetting = new SiteContent { Key = "HeroImage", Value = "/uploads/" + fileName };
                    _context.SiteContents.Add(heroSetting);
                }
                else
                {
                    heroSetting.Value = "/uploads/" + fileName;
                    _context.SiteContents.Update(heroSetting);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Imagem do topo atualizada com sucesso!";
            }
            return RedirectToAction(nameof(Index));
        }

        // --- Blog Management ---

        /// <summary>
        /// List all blog posts.
        /// </summary>
        public async Task<IActionResult> Blog()
        {
            var posts = await _context.BlogPosts.OrderByDescending(p => p.CreatedAt).ToListAsync();
            return View(posts);
        }

        /// <summary>
        /// View for creating a new blog post.
        /// </summary>
        public IActionResult CreatePost()
        {
            return View();
        }

        /// <summary>
        /// Handles the creation of a new blog post.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(BlogPost post)
        {
            if (string.IsNullOrEmpty(post.Title))
            {
                ModelState.AddModelError("Title", "O título é obrigatório.");
                return View(post);
            }

            post.CreatedAt = DateTime.Now;
            post.IsPublished = true; // Auto-publish for now
            _context.Add(post);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Postagem criada com sucesso!";
            return RedirectToAction(nameof(Blog));
        }

        /// <summary>
        /// View for editing an existing blog post.
        /// </summary>
        public async Task<IActionResult> EditPost(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null) return NotFound();
            return View(post);
        }

        /// <summary>
        /// Handles the update of a blog post.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(BlogPost post)
        {
            if (string.IsNullOrEmpty(post.Title))
            {
                ModelState.AddModelError("Title", "O título é obrigatório.");
                return View(post);
            }

            post.IsPublished = true;
            _context.Update(post);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Postagem atualizada com sucesso!";
            return RedirectToAction(nameof(Blog));
        }

        /// <summary>
        /// Deletes a blog post.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post != null)
            {
                _context.BlogPosts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Blog));
        }

        // --- Site Content Management ---

        /// <summary>
        /// View for editing the "About Me" section.
        /// </summary>
        public async Task<IActionResult> About()
        {
            var content = await _context.SiteContents.FirstOrDefaultAsync(c => c.Key == "AboutMe");
            ViewBag.AboutContent = content?.Value ?? "";
            return View();
        }

        /// <summary>
        /// Updates the "About Me" dynamic content.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAbout(string content)
        {
            var aboutSetting = await _context.SiteContents.FirstOrDefaultAsync(c => c.Key == "AboutMe");
            
            if (aboutSetting == null)
            {
                aboutSetting = new SiteContent { Key = "AboutMe", Value = content };
                _context.SiteContents.Add(aboutSetting);
            }
            else
            {
                aboutSetting.Value = content;
                _context.SiteContents.Update(aboutSetting);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Biografia atualizada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}
