using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using psipri.com.br.Data;
using psipri.com.br.Models;
using psipri.com.br.Services;
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

            var videoSetting = await _context.SiteContents.FirstOrDefaultAsync(c => c.Key == "InstitutionalVideoUrl");
            var videoAspect = await _context.SiteContents.FirstOrDefaultAsync(c => c.Key == "InstitutionalVideoAspectRatio");
            ViewBag.InstitutionalVideoUrl = videoSetting?.Value;
            ViewBag.InstitutionalVideoId = YouTubeHelper.ExtractVideoId(videoSetting?.Value);
            ViewBag.InstitutionalVideoEmbedUrl = YouTubeHelper.GetEmbedUrl(videoSetting?.Value);
            ViewBag.InstitutionalVideoProvider = YouTubeHelper.GetProvider(videoSetting?.Value);
            ViewBag.InstitutionalVideoAspectRatio = videoAspect?.Value ?? (YouTubeHelper.IsLikelyVertical(videoSetting?.Value) ? "vertical" : "vertical");

            return View();
        }

        /// <summary>
        /// Handles hero image upload with strict validation.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadHero(IFormFile heroFile)
        {
            if (heroFile != null && heroFile.Length > 0)
            {
                // Max file size: 5MB
                if (heroFile.Length > 5 * 1024 * 1024)
                {
                    TempData["Error"] = "A imagem selecionada excede o tamanho máximo permitido de 5MB.";
                    return RedirectToAction(nameof(Index));
                }

                // Allowed extensions allowlist
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(heroFile.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    TempData["Error"] = "Formato de arquivo não permitido. Use imagens .jpg, .png ou .webp.";
                    return RedirectToAction(nameof(Index));
                }

                // MIME type check
                if (!heroFile.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Error"] = "O arquivo enviado não é uma imagem válida.";
                    return RedirectToAction(nameof(Index));
                }

                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                var fileName = $"hero_{DateTime.UtcNow.Ticks}{extension}";
                var filePath = Path.Combine(uploadsDir, fileName);

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

        // --- Institutional Video Management ---

        /// <summary>
        /// <summary>
        /// View for managing the institutional video link and aspect ratio.
        /// </summary>
        public async Task<IActionResult> Video()
        {
            var video = await _context.SiteContents.FirstOrDefaultAsync(c => c.Key == "InstitutionalVideoUrl");
            var aspect = await _context.SiteContents.FirstOrDefaultAsync(c => c.Key == "InstitutionalVideoAspectRatio");

            ViewBag.VideoUrl = video?.Value ?? "";
            ViewBag.VideoId = YouTubeHelper.ExtractVideoId(video?.Value);
            ViewBag.VideoEmbedUrl = YouTubeHelper.GetEmbedUrl(video?.Value);
            ViewBag.VideoProvider = YouTubeHelper.GetProvider(video?.Value);
            ViewBag.AspectRatio = aspect?.Value ?? (YouTubeHelper.IsLikelyVertical(video?.Value) ? "vertical" : "vertical");

            return View();
        }

        /// <summary>
        /// Updates the institutional video URL and aspect ratio from YouTube or Instagram.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVideo(string? videoUrl, string? aspectRatio)
        {
            if (string.IsNullOrWhiteSpace(videoUrl))
            {
                var existingUrl = await _context.SiteContents.FirstOrDefaultAsync(c => c.Key == "InstitutionalVideoUrl");
                if (existingUrl != null)
                {
                    _context.SiteContents.Remove(existingUrl);
                }

                var existingAspect = await _context.SiteContents.FirstOrDefaultAsync(c => c.Key == "InstitutionalVideoAspectRatio");
                if (existingAspect != null)
                {
                    _context.SiteContents.Remove(existingAspect);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Vídeo institucional removido do site!";
                return RedirectToAction(nameof(Index));
            }

            var embedUrl = YouTubeHelper.GetEmbedUrl(videoUrl);
            var videoId = YouTubeHelper.ExtractVideoId(videoUrl);

            if (string.IsNullOrEmpty(embedUrl) || string.IsNullOrEmpty(videoId))
            {
                ModelState.AddModelError("videoUrl", "Link não reconhecido. Certifique-se de colar uma URL válida do YouTube (vídeo ou Shorts) ou do Instagram (Reel).");
                ViewBag.VideoUrl = videoUrl;
                ViewBag.VideoId = null;
                ViewBag.VideoEmbedUrl = null;
                ViewBag.AspectRatio = aspectRatio ?? "vertical";
                return View("Video");
            }

            var cleanAspect = (aspectRatio == "horizontal") ? "horizontal" : "vertical";

            // Save or update URL
            var videoSetting = await _context.SiteContents.FirstOrDefaultAsync(c => c.Key == "InstitutionalVideoUrl");
            if (videoSetting == null)
            {
                videoSetting = new SiteContent { Key = "InstitutionalVideoUrl", Value = videoUrl.Trim() };
                _context.SiteContents.Add(videoSetting);
            }
            else
            {
                videoSetting.Value = videoUrl.Trim();
                _context.SiteContents.Update(videoSetting);
            }

            // Save or update Aspect Ratio
            var aspectSetting = await _context.SiteContents.FirstOrDefaultAsync(c => c.Key == "InstitutionalVideoAspectRatio");
            if (aspectSetting == null)
            {
                aspectSetting = new SiteContent { Key = "InstitutionalVideoAspectRatio", Value = cleanAspect };
                _context.SiteContents.Add(aspectSetting);
            }
            else
            {
                aspectSetting.Value = cleanAspect;
                _context.SiteContents.Update(aspectSetting);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Vídeo institucional e formato de exibição salvos com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Removes the institutional video.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveVideo()
        {
            var video = await _context.SiteContents.FirstOrDefaultAsync(c => c.Key == "InstitutionalVideoUrl");
            if (video != null)
            {
                _context.SiteContents.Remove(video);
            }

            var aspect = await _context.SiteContents.FirstOrDefaultAsync(c => c.Key == "InstitutionalVideoAspectRatio");
            if (aspect != null)
            {
                _context.SiteContents.Remove(aspect);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Vídeo institucional removido com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}
