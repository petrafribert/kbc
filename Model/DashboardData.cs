#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KBC.Model
{
    public class DashboardData
    {
        public List<PrijavaInfo> ProcessInstances { get; set; }
        public List<TaskInfo> MojiPreglediBezTermina { get; set; }
        public List<TaskInfo> AdminTasks { get; set; }

        public IEnumerable<PrijavaInfo> NoviPregledi
        {
            get
            {
                return ProcessInstances.Where(instance => !instance.Zavrsio);
            }
        }

        public List<TaskInfo> MojiPreglediPonudeniTermin
        {
            get;
            set;
        }

        public string user { get; set; }
    }
}
