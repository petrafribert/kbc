using System.Collections.Generic;

namespace KBC.ViewModels;

public class PacijentPreglediViewModel
{
    public PacijentViewModel pacijent { get; set; }
    public IEnumerable<PovijestPregledaViewModel> PovijestiPregleda { get; set; }
    public PagingInfo PagingInfo { get; set; }
}