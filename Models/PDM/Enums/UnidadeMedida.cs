using System.ComponentModel.DataAnnotations;

namespace psipri.com.br.Models.PDM.Enums
{
    public enum UnidadeMedida
    {
        [Display(Name = "Quilo (kg)")]
        KG,

        [Display(Name = "Mililitro (ml)")]
        ML,

        [Display(Name = "Unidade (un)")]
        UN,

        [Display(Name = "Metro (mt)")]
        MT
    }
}
