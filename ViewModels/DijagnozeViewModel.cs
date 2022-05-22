using System.Collections.Generic;
using KBC.Model;

namespace KBC.ViewModels
{
    public class DijagnozeViewModel
    {
        public IEnumerable<SifDijagnozaMKB10> Dijagnoze { get; set; }
        public PagingInfo PagingInfo { get; set; }
        
        public string search { get; set; }
    }
}