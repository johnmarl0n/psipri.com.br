using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace psipri.com.br.Models.PDM
{
    public class PDMReceitaItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReceitaId { get; set; }

        [ForeignKey("ReceitaId")]
        public virtual PDMReceita? Receita { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [ForeignKey("ProdutoId")]
        public virtual PDMProduto? Produto { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        public decimal Quantidade { get; set; }
    }
}
