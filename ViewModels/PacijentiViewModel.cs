#pragma warning disable CS1591

using System.Collections.Generic;

namespace KBC.ViewModels
{
    public class PacijentiViewModel
    {
        public IEnumerable<PacijentViewModel> Pacijenti { get; set; }
        public PagingInfo PagingInfo { get; set; }

        public string search { get; set; }
    }
}