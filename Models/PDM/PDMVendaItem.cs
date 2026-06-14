using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace psipri.com.br.Models.PDM
{
    public class PDMVendaItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VendaId { get; set; }

        [ForeignKey("VendaId")]
        public virtual PDMVenda? Venda { get; set; }

        [Required]
        public int ReceitaId { get; set; }

        [ForeignKey("ReceitaId")]
        public virtual PDMReceita? Receita { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "O preço unitário é obrigatório.")]
        public decimal PrecoUnitario { get; set; }

        [Required]
        public decimal Subtotal { get; set; }
    }
}
