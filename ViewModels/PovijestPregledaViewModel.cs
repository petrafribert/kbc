#pragma warning disable CS1591

using System;

namespace KBC.ViewModels
{
    public class PovijestPregledaViewModel
    {
        public int Id { get; set; }

        public DateTime DatumPregleda { get; set; }

        public string Anamneza { get; set; }

        public string Terapija { get; set; }

        public int MBO { get; set; }

        public string MKB10 { get; set; }
    }
}