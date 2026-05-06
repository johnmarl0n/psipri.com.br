using System.ComponentModel.DataAnnotations;

namespace psipri.com.br.Models
{
    /// <summary>
    /// Represents dynamic site content that can be managed via the maintenance area.
    /// </summary>
    public class SiteContent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
