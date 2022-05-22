

#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KBC.ViewModels;

namespace KBC.Model
{
    public partial class Pregled
    {
        public int Id { get; set; }

        [Display(Name = "Datum pregleda", Prompt = "Unesite datum pregleda")]
        [Required(ErrorMessage = "Datum pregleda je obvezno polje")]
        public DateTime DatumPregleda { get; set; }

        [Display(Name = "Anamneza", Prompt = "Unesite anamnezu")]
        [Required(ErrorMessage = "Anamneza je obvezno polje")]
        public string Anamneza { get; set; }

        [Display(Name = "Terapija", Prompt = "Unesite terapiju")]
        [Required(ErrorMessage = "Terapija je obvezno polje")]
        public string Terapija { get; set; }

        [Display(Name = "Matični broj osiguranika", Prompt = "Odaberite matični broj osiguranika")]
        [Required(ErrorMessage = "Matični broj osiguranika je obvezno polje")]
        [Range(100000000,999999999, ErrorMessage = "MBO pacijenta mora biti duljine 9")]
        public int PacijentMbo { get; set; }

        [Display(Name = "Šifra dijagnoze", Prompt = "Odaberite šifru dijagnoze")]
        // [Required(ErrorMessage = "Šifra dijagnoze je obvezno polje")]
        public string DijagnozaMkb10 { get; set; }

        public virtual Pacijent Pacijent { get; set; }
        
        public virtual SifDijagnozaMKB10 Dijagnoza { get; set; }
    }
}
