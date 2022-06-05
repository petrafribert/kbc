#pragma warning disable CS1591

namespace KBC.Model
{
    public class PrijavaInfo
    {
        public int MBO { get; set; }
        public string PID { get; set; }
        public string DatumPregleda { get; set; }
        public bool Zavrsio { get; set; }
        public bool MozeLiSePreuzeti { get; set; }
        public string Doktor { get; set; }
    }
}