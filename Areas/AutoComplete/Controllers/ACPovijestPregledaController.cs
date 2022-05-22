using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KBC.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PovijestPregleda = KBC.Areas.AutoComplete.Models.PovijestPregleda;

namespace KBC.Areas.AutoComplete.Controllers
{
    [Area("AutoComplete")]
    public class ACPovijestPregledaController : Controller
    {

        private readonly KBCGrupaContext ctx;
        private readonly AppSettings appData;

        public ACPovijestPregledaController(KBCGrupaContext ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            this.ctx = ctx;
            appData = optionsSnapshot.Value;
        }

        public async Task<IEnumerable<PovijestPregleda>> Get(string term)
        {
            var query = ctx.Pregledi
                .Select(t => new PovijestPregleda
                {
                    Id = t.Id,
                    Label = t.Anamneza
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
