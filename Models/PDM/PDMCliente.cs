using System.ComponentModel.DataAnnotations;

namespace psipri.com.br.Models.PDM
{
    public class PDMCliente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telefone { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? Observacoes { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.Now;

        public virtual ICollection<PDMVenda> Vendas { get; set; } = new List<PDMVenda>();
    }
}
