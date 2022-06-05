#pragma warning disable CS1591

using System;

namespace KBC.Model
{
    public class TaskInfo
    {
        public string TID { get; set; }
        public string TaskKey { get; set; }
        public string TaskName { get; set; }
        public int MBO { get; set; }
        public string PID { get; set; }
        public string DatumPregleda { get; set; }
        public string Doktor { get; set; }
    }
}