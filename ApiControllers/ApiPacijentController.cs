using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KBC.Model;
using KBC.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog.Web;

namespace KBC.ApiControllers
{

    /// <summary>
    /// Web API servis za rad s pacijentima
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]
    public class ApiPacijentController : ControllerBase, ICustomController<int, PacijentViewModel2>
    {

        private readonly KBCGrupaContext ctx;

        private static Dictionary<string, Expression<Func<Pacijent, object>>> orderSelectors = new Dictionary<string, Expression<Func<Pacijent, object>>>
        {
            [nameof(PacijentViewModel2.MBO).ToLower()] = m => m.MBO,
            [nameof(PacijentViewModel2.Ime).ToLower()] = m => m.Ime,
            [nameof(PacijentViewModel2.Prezime).ToLower()] = m => m.Prezime,
            [nameof(PacijentViewModel2.DatumRodjenja).ToLower()] = m => m.DatumRodjenja,
        };

        public ApiPacijentController(KBCGrupaContext ctx)
        {
            this.ctx = ctx;
        }

        /// <summary>
        /// Vraća broj svih pacijenata filtriran prema mbo-u pacijenta
        /// </summary>
        /// <param name="filter">Opcionalni filter za ime pacijenta</param>
        /// <returns></returns>
        [HttpGet("count", Name = "BrojPacijenata")]
        public async Task<int> Count([FromQuery] string filter)
        {

            var query = ctx.Pacijenti.AsQueryable();

            // if (!string.IsNullOrWhiteSpace(filter))
            // {
            //     query = query.Where(p => p.Ime.Contains(filter) || p.Prezime.Contains(filter));
            // }

            int count = await query.CountAsync();

            return count;
        }

        /// <summary>
        /// Dohvat pacijenata (opcionalno filtrirano po imenu pacijenta).
        /// Broj pacijenata, poredak, početna pozicija određeni s loadParams.
        /// </summary>
        /// <param name="loadParams">Postavke za straničenje i filter</param>
        /// <returns></returns>
        [HttpGet(Name = "DohvatiPacijente")]
        public async Task<List<PacijentViewModel2>> GetAll([FromQuery] LoadParams loadParams)
        {

            //ovo možda treba obrisati?
            var query = ctx.Pacijenti.AsQueryable();
                // .Include(p => p.Ime + p.Prezime).AsQueryable();
            // if (!string.IsNullOrWhiteSpace(loadParams.Filter))
            // {
            //     query = query.Where(p => p.Ime.Contains(loadParams.Filter) || p.Prezime.Contains(loadParams.Filter));
            // }

            if (loadParams.SortColumn != null)
            {
                if (orderSelectors.TryGetValue(loadParams.SortColumn.ToLower(), out var expr))
                {
                    query = loadParams.Descending ? query.OrderByDescending(expr) : query.OrderBy(expr);
                }
            }

            var list = await query.Select(k => new PacijentViewModel2
            {
                MBO = k.MBO,
                Ime = k.Ime,
                Prezime = k.Prezime,
                DatumRodjenja = k.DatumRodjenja
            })
                                   .Skip(loadParams.StartIndex)
                                   .Take(loadParams.Rows)
                                   .ToListAsync();

            return list;
        }

        /// <summary>
        /// Vraća pacijenta čiji je mbo jednak vrijednosti parametra mbo
        /// </summary>
        /// <param name="MBO">MBO</param>
        /// <returns></returns>
        [HttpGet("{MBO}", Name = "DohvatiPacijenta")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PacijentViewModel2>> Get(int MBO)
        {

            var pacijent = await ctx.Pacijenti
                                    .Include(p => p.MBO) //obrisati?
                                    .Where(k => k.MBO == MBO)
                                    .Select(k => new PacijentViewModel2
                                    {
                                        MBO = k.MBO,
                                        Ime = k.Ime,
                                        Prezime = k.Prezime,
                                        DatumRodjenja = k.DatumRodjenja
                                    })
                                    .FirstOrDefaultAsync();

            if (pacijent == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"No data for MBO = {MBO}");
            }
            else
            {
                return pacijent;
            }
        }

        /// <summary>
        /// Brisanje pacijenta određenog s mbo
        /// </summary>
        /// <param name="MBO">Vrijednost primarnog ključa (mbo pacijenta)</param>
        /// <returns></returns>
        /// <response code="204">Ako je pacijent uspješno obrisan</response>
        /// <response code="404">Ako pacijent s poslanim mbo-m ne postoji</response>      
        [HttpDelete("{MBO}", Name = "ObrisiPacijenta")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int MBO)
        {

            var pacijent = await ctx.Pacijenti.FindAsync(MBO);
            if (pacijent == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid MBO = {MBO}");
            }
            else
            {
                ctx.Remove(pacijent);
                await ctx.SaveChangesAsync();
                return NoContent();
            }
        }

        /// <summary>
        /// Ažurira pacijenta
        /// </summary>
        /// <param name="MBO">parametar čija vrijednost jednoznačno identificira pacijenta</param>
        /// <param name="model">Podaci o pacijentu. MBO se mora podudarati s parametrom MBO</param>
        /// <returns></returns>
        [HttpPut("{MBO}", Name = "AzurirajPacijenta")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int MBO, PacijentViewModel2 model)
        {

            if (model.MBO != MBO)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different MBOs {MBO} vs {model.MBO}");
            }
            else
            {
                var Pacijent = await ctx.Pacijenti.FindAsync(MBO);
                if (Pacijent == null)
                {
                    return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid MBO = {MBO}");
                }

                Pacijent.Ime = model.Ime;
                Pacijent.Prezime = model.Prezime;
                Pacijent.DatumRodjenja = model.DatumRodjenja;
                await ctx.SaveChangesAsync();
                return NoContent();
            }

        }

        /// <summary>
        /// Stvara novog pacijenta opisanog poslanim modelom
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost(Name = "DodajPacijenta")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(PacijentViewModel2 model)
        {

            Pacijent Pacijent = new Pacijent
            {
                MBO = model.MBO,
                Ime = model.Ime,
                Prezime = model.Prezime,
                DatumRodjenja = model.DatumRodjenja
            };

            ctx.Add(Pacijent);
            await ctx.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { MBO = Pacijent.MBO }, Pacijent);

        }

    }
}
