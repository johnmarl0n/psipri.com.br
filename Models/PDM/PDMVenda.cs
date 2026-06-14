using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace psipri.com.br.Models.PDM
{
    public class PDMVenda
    {
        [Key]
        public int Id { get; set; }

        public int? ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual PDMCliente? Cliente { get; set; }

        [Required(ErrorMessage = "A data da venda é obrigatória.")]
        public DateTime DataVenda { get; set; } = DateTime.Now;

        [Required]
        public decimal ValorTotal { get; set; } = 0;

        [Required]
        public decimal Desconto { get; set; } = 0;

        [Required]
        public decimal ValorFinal { get; set; } = 0;

        [Required(ErrorMessage = "A forma de pagamento é obrigatória.")]
        [StringLength(50)]
        public string FormaPagamento { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Observacoes { get; set; }

        public virtual ICollection<PDMVendaItem> VendaItems { get; set; } = new List<PDMVendaItem>();
    }
}
