using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KBC.ViewModels
{
    public class PacijentViewModel
    {
        public int MBO { get; set; }

        public string Ime { get; set; }

        public string Prezime { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DatumRodjenja { get; set; }

        public IEnumerable<PovijestPregledaViewModel> PovijestiPregleda { get; set; }
    }
}