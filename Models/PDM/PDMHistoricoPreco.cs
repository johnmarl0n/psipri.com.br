using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace psipri.com.br.Models.PDM
{
    public class PDMHistoricoPreco
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [ForeignKey("ProdutoId")]
        public virtual PDMProduto? Produto { get; set; }

        [Required]
        public decimal PrecoAnterior { get; set; }

        [Required]
        public decimal PrecoNovo { get; set; }

        public DateTime DataAlteracao { get; set; } = DateTime.Now;
    }
}
