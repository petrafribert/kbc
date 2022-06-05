#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace KBC.Model
{
    public partial class Pacijent
    {
        public Pacijent()
        {
            PovijestPregledas = new HashSet<Pregled>();
        }


        [Display(Name = "MBO pacijenta", Prompt = "Unesite MBO pacijenta")]
        [Required(ErrorMessage = "MBO pacijenta je obavezno polje")]
        [Range(100000000, 999999999, ErrorMessage = "MBO pacijenta mora biti duljine 9")]
        public int MBO { get; set; }

        [Display(Name = "Ime pacijenta", Prompt = "Unesite ime pacijenta")]
        [Required(ErrorMessage = "Ime pacijenta je obavezno polje")]
        [RegularExpression("^[A-Z][a-zA-Z-]*", ErrorMessage = "Ime mora počinjati velikim slovom i sadržavati samo slova i znak -")]
        public string Ime { get; set; }

        [Display(Name = "Prezime pacijenta", Prompt = "Unesite prezime pacijenta")]
        [Required(ErrorMessage = "Prezime pacijenta je obavezno polje")]
        [RegularExpression("^[A-Z][a-zA-Z-]*", ErrorMessage = "Prezime mora počinjati velikim slovom i sadržavati samo slova i znak -")]
        public string Prezime { get; set; }

        [Display(Name = "Datum rođenja pacijenta", Prompt = "Unesite datum rođenja pacijenta")]
        [Required(ErrorMessage = "Datum rođenja pacijenta je obavezno polje")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DatumRodjenja { get; set; }

        public virtual ICollection<Pregled> PovijestPregledas { get; set; }


    }
}
