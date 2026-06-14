using System.ComponentModel.DataAnnotations;

namespace psipri.com.br.Models.PDM
{
    public class PDMConfiguracao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Chave { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Valor { get; set; } = string.Empty;
    }
}
