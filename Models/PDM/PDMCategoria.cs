using System.ComponentModel.DataAnnotations;

namespace psipri.com.br.Models.PDM
{
    public class PDMCategoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(50)]
        public string Icone { get; set; } = "fa-tag"; // CSS class for FontAwesome icons

        public virtual ICollection<PDMProduto> Produtos { get; set; } = new List<PDMProduto>();
    }
}
