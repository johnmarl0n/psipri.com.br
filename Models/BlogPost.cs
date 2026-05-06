using System;
using System.ComponentModel.DataAnnotations;

namespace psipri.com.br.Models
{
    /// <summary>
    /// Represents a blog post entry in the system.
    /// </summary>
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [StringLength(500)]
        public string Summary { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsPublished { get; set; } = false;
    }
}
