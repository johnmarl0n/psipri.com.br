using System.ComponentModel.DataAnnotations;

namespace psipri.com.br.Models.PDM
{
    public class PDMReceita
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da receita é obrigatório.")]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descricao { get; set; }

        public string? Instrucoes { get; set; }

        [Required(ErrorMessage = "A margem de lucro é obrigatória.")]
        public decimal MargemLucro { get; set; } = 0;

        [Required(ErrorMessage = "O rendimento de unidades é obrigatório.")]
        public int RendimentoUnidades { get; set; } = 1;

        public byte[]? ImagemThumb { get; set; }

        [StringLength(50)]
        public string? ImagemMimeType { get; set; }

        public bool Ativa { get; set; } = true;

        public DateTime CriadoEm { get; set; } = DateTime.Now;

        public virtual ICollection<PDMReceitaItem> ReceitaItems { get; set; } = new List<PDMReceitaItem>();
        public virtual ICollection<PDMProducao> Producoes { get; set; } = new List<PDMProducao>();
        public virtual ICollection<PDMVendaItem> VendaItems { get; set; } = new List<PDMVendaItem>();
    }
}
