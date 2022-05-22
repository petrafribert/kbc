using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KBC.Areas.AutoComplete.Models;
using KBC.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KBC.Areas.AutoComplete.Controllers
{
    [Area("AutoComplete")]
    public class ACDijagnozaMKB10Controller : Controller
    {

        private readonly KBCGrupaContext ctx;
        private readonly AppSettings appData;

        public ACDijagnozaMKB10Controller(KBCGrupaContext ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            this.ctx = ctx;
            appData = optionsSnapshot.Value;
        }

        public async Task<IEnumerable<DijagnozaMKB10>> Get(string term)
        {
            var query = ctx.SifDijagnozaMKB10
                .Select(t => new DijagnozaMKB10
                {
                    Id = t.mkb10,
                    Label = t.Dijagnoza
                })
                .Where(l => l.Label.Contains(term));

            var list = await query.OrderBy(l => l.Label)
                .ThenBy(l => l.Id)
                .Take(appData.AutoCompleteCount)
                .ToListAsync();
            return list;
        }

    }
}
