using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace psipri.com.br.Models.PDM
{
    public class PDMProducao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReceitaId { get; set; }

        [ForeignKey("ReceitaId")]
        public virtual PDMReceita? Receita { get; set; }

        [Required(ErrorMessage = "A quantidade produzida é obrigatória.")]
        public int QuantidadeProduzida { get; set; }

        [Required]
        public decimal CustoTotalProducao { get; set; }

        [StringLength(500)]
        public string? Observacoes { get; set; }

        public DateTime DataProducao { get; set; } = DateTime.Now;
    }
}
