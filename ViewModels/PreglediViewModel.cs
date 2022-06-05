#pragma warning disable CS1591

using System.Collections.Generic;

namespace KBC.ViewModels
{
    public class PreglediViewModel
    {
        public IEnumerable<PovijestPregledaViewModel> PovijestiPregleda { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}