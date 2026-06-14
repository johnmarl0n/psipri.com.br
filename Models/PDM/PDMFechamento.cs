using System.ComponentModel.DataAnnotations;

namespace psipri.com.br.Models.PDM
{
    public class PDMFechamento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DataFechamento { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string Periodo { get; set; } = string.Empty;

        [Required]
        public decimal ValorTotalEstoque { get; set; }

        [StringLength(500)]
        public string? Observacoes { get; set; }

        public virtual ICollection<PDMFechamentoItem> FechamentoItems { get; set; } = new List<PDMFechamentoItem>();
    }
}
