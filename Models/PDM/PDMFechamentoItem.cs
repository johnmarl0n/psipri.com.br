using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace psipri.com.br.Models.PDM
{
    public class PDMFechamentoItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FechamentoId { get; set; }

        [ForeignKey("FechamentoId")]
        public virtual PDMFechamento? Fechamento { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [ForeignKey("ProdutoId")]
        public virtual PDMProduto? Produto { get; set; }

        [Required]
        public decimal QuantidadeEstoque { get; set; }

        [Required]
        public decimal PrecoCusto { get; set; }

        [Required]
        public decimal ValorTotal { get; set; }
    }
}
