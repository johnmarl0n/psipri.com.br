using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using psipri.com.br.Models.PDM.Enums;

namespace psipri.com.br.Models.PDM
{
    public class PDMProduto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public virtual PDMCategoria? Categoria { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "A unidade de medida é obrigatória.")]
        public UnidadeMedida UnidadeMedida { get; set; }

        [Required(ErrorMessage = "A quantidade em estoque é obrigatória.")]
        public decimal QuantidadeEstoque { get; set; } = 0;

        [Required(ErrorMessage = "O estoque mínimo é obrigatório.")]
        public decimal EstoqueMinimo { get; set; } = 0;

        [Required(ErrorMessage = "O preço de custo é obrigatório.")]
        public decimal PrecoCusto { get; set; } = 0;

        [Required(ErrorMessage = "A margem é obrigatória.")]
        public decimal Margem { get; set; } = 0;

        public byte[]? ImagemThumb { get; set; }

        [StringLength(50)]
        public string? ImagemMimeType { get; set; }

        public bool Ativo { get; set; } = true;

        public DateTime CriadoEm { get; set; } = DateTime.Now;

        public DateTime AtualizadoEm { get; set; } = DateTime.Now;

        [NotMapped]
        public decimal PrecoVenda => PrecoCusto * (1 + Margem / 100);

        public virtual ICollection<PDMReceitaItem> ReceitaItems { get; set; } = new List<PDMReceitaItem>();
        public virtual ICollection<PDMFechamentoItem> FechamentoItems { get; set; } = new List<PDMFechamentoItem>();
        public virtual ICollection<PDMHistoricoPreco> HistoricoPrecos { get; set; } = new List<PDMHistoricoPreco>();
    }
}
