using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KBC.Model;
using KBC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KBC.Controllers
{
    public class PregledController : Controller
    {
        private readonly KBCGrupaContext ctx;
        private readonly AppSettings appSettings;

        public PregledController(KBCGrupaContext ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            this.ctx = ctx;
            appSettings = optionsSnapshot.Value;
        }

        [HttpGet]
        public IActionResult Create()
        {
            PrepareDropDownLists();
            return View("CreatePregled");
        }
        
        [HttpGet]
        public IActionResult CreateWithPatient(int pacijent) 
        {
            PrepareDropDownLists();
            var pregled = new Pregled();
            pregled.PacijentMbo = pacijent;
            return View("CreatePregled", pregled);
        }

        private void PrepareDropDownLists()
        {
            var pacijenti = ctx.Pacijenti
                .OrderBy(d => d.MBO)
                .Select(d => new { MBO = d.MBO, Ime = d.MBO + " " + d.Ime + " " + d.Prezime })
                .ToList();
            ViewBag.Pacijenti = new SelectList(pacijenti, nameof(Pacijent.MBO), nameof(Pacijent.Ime));
            
            var dijagnoze = ctx.SifDijagnozaMKB10
                .OrderBy(d => d.mkb10)
                .Select(d => new { d.mkb10, Dijagnoza = d.mkb10 + " - " + d.Dijagnoza })
                .ToList();
            ViewBag.Dijagnoze = new SelectList(dijagnoze, nameof(SifDijagnozaMKB10.mkb10), nameof(SifDijagnozaMKB10.Dijagnoza));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pregled pregled)
        {
            Console.WriteLine("FROM CREATE " + pregled.DijagnozaMkb10);

            if (ModelState.IsValid)
            {
                try
                {
                    Console.Write("FROM PATIENT " + pregled.PacijentMbo);
                    ctx.Add(pregled);
                    ctx.SaveChanges();
                    TempData[Constants.Message] = $"Povijest pregleda {pregled.Id} uspješno dodan.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    PrepareDropDownLists();
                    if (pregled.PacijentMbo == 0)
                    {
                        return View("CreatePregled");
                    }

                    var pregledView = new Pregled();
                    pregledView.PacijentMbo = pregled.PacijentMbo;
                    return View("CreatePregled", pregledView);
                }

            }
            else
            {
                PrepareDropDownLists();
                if (pregled.PacijentMbo == 0)
                {
                    return View("CreatePregled");
                }

                var pregledView = new Pregled();
                pregledView.PacijentMbo = pregled.PacijentMbo;
                return View("CreatePregled", pregledView);
                
            }

        }


        [HttpGet]
        public IActionResult Edit(int Id, int page = 1, int sort = 1, bool ascending = true)
        {
            var povijest = ctx.Pregledi
                .AsNoTracking()
                .Where(p => p.Id == Id)
                .SingleOrDefault();
            if (povijest == null)
            {
                return NotFound($"Ne postoji povijest pregleda s oznakom {Id}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                PrepareDropDownLists();
                return View("EditPregled", povijest);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(int Id, int page = 1, int sort = 1, bool ascending = true)
        {
            try
            {
                Pregled povijest = await ctx.Pregledi.FindAsync(Id);
                
                if (povijest == null)
                {
                    return NotFound($"Ne postoji povijest pregleda s oznakom {Id}");
                }
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                bool ok = await TryUpdateModelAsync<Pregled>(povijest, "", d => d.Id, d => d.Anamneza, d => d.Terapija, d => d.DatumPregleda, 
                            d => d.PacijentMbo, d => d.DijagnozaMkb10);
                if (ok)
                {
                    try
                    {
                        TempData[Constants.Message] = $"Povijest pregleda {povijest.Id} uspješno ažurirana.";
                        TempData[Constants.ErrorOccurred] = false;
                        await ctx.SaveChangesAsync();
                        return RedirectToAction(nameof(Index), new { page, sort, ascending });
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                        return View("EditPregled", povijest);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o povijesti pregleda nije moguće povezati s forme.");
                    return View("EditPregled", povijest);
                }
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), new { Id, page, sort, ascending });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int Id, int page = 1, int sort = 1, bool ascending = true)
        {
            var povijest = ctx.Pregledi.Find(Id);
            if (povijest == null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    int naziv = povijest.Id;
                    ctx.Remove(povijest);
                    ctx.SaveChanges();
                    TempData[Constants.Message] = $"Povijest pregleda {naziv} uspješno obrisana.";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = $"Pogreška prilikom brisanja povijesti pregleda." + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                }
                return RedirectToAction(nameof(Index), new { page, sort, ascending });
            }
        }

        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        {
            int pagesize = appSettings.PageSize;
            var query = ctx.Pregledi.AsNoTracking();

            int count = query.Count();

            var pagingInfo = new PagingInfo
            {
                CurrentPage = page,
                Sort = sort,
                Ascending = ascending,
                ItemsPerPage = pagesize,
                TotalItems = count
            };

            if (page > pagingInfo.TotalPages)
            {
                return RedirectToAction(nameof(Index), new { page = pagingInfo.TotalPages, sort, ascending });
            }

            Expression<Func<Pregled, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = d => d.Id;
                    break;
                case 2:
                    orderSelector = d => d.PacijentMbo;
                    break;
                case 3:
                    orderSelector = d => d.DatumPregleda;
                    break;
                case 4:
                    orderSelector = d => d.Anamneza;
                    break;
                case 5:
                    orderSelector = d => d.Terapija;
                    break;
                case 6:
                    orderSelector = d => d.DijagnozaMkb10;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }


            var povijesti = query
                .Select(o => new PovijestPregledaViewModel
                {
                    Id = o.Id,
                    DatumPregleda = o.DatumPregleda,
                    Anamneza = o.Anamneza,
                    Terapija = o.Terapija,
                    MBO = o.PacijentMbo,
                    MKB10 = o.DijagnozaMkb10
                })
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            var model = new PreglediViewModel
            {
                PovijestiPregleda = povijesti,
                PagingInfo = pagingInfo
            };
            return View(viewName: "Pregledi", model: model);
        }
    }
}
