#pragma warning disable CS1591

#nullable disable

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KBC.Model
{
    public partial class SifDijagnozaMKB10
    {
        [Display(Name = "MKB10 šifra", Prompt = "Unesite MKB10 šifru")]
        [Required(ErrorMessage = "MKB10 šifra je obavezno polje")]
        public string mkb10 { get; set; }

        [Display(Name = "Dijagnoza", Prompt = "Unesite dijagnozu")]
        [Required(ErrorMessage = "Dijagnoza je obvezno polje")]
        public string Dijagnoza { get; set; }

        public virtual ICollection<Pregled> PovijestPregledas { get; set; }

    }
}