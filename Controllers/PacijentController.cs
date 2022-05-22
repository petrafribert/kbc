using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KBC.Areas.AutoComplete.Models;
using KBC.Model;
using KBC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KBC.Controllers
{
    public class PacijentController : Controller
    {
        private readonly KBCGrupaContext ctx;
        private readonly AppSettings _appSettings;

        public PacijentController(KBCGrupaContext ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            this.ctx = ctx;
            _appSettings = optionsSnapshot.Value;
        }

        [HttpGet]
        public IActionResult Create()
        {
            PrepareDropDownLists();
            return View("CreatePacijent");
        }

        private void PrepareDropDownLists()
        {
            var dijagnoze = ctx.SifDijagnozaMKB10
                .OrderBy(d => d.mkb10)
                .Select(d => new { d.Dijagnoza, MKB10 = d.mkb10 })
                .ToList();
            ViewBag.Dijagnoze = new SelectList(dijagnoze, nameof(DijagnozaMKB10.Id), nameof(DijagnozaMKB10.Label));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pacijent pacijent)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(pacijent);
                    ctx.SaveChanges();
                    TempData[Constants.Message] = $"Pacijent {pacijent.Ime} uspješno dodan.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index2));
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View("CreatePacijent", pacijent);
                }
            }
            else
            {
                return View("CreatePacijent", pacijent);
            }
        }


        [HttpGet]
        public IActionResult Edit(int MBO, int page = 1, int sort = 1, bool ascending = true)
        {
            var pacijent = ctx.Pacijenti
                .AsNoTracking()
                .Where(d => d.MBO == MBO)
                .SingleOrDefault();
            if (pacijent == null)
            {
                return NotFound($"Ne postoji pacijent s MBO = {MBO}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                PrepareDropDownLists();
                return View("EditPacijent", pacijent);
            }
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int MBO, int page = 1, int sort = 1, bool ascending = true)
        {
            try
            {
                Pacijent pacijent = await ctx.Pacijenti.FindAsync(MBO);
                if (pacijent == null)
                {
                    return NotFound($"Ne postoji pacijent s MBO = {MBO}");
                }

                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                bool ok = await TryUpdateModelAsync<Pacijent>(pacijent, "", d => d.Ime, d => d.MBO, d=> d.Prezime, d => d.DatumRodjenja);
                if (ok)
                {
                    try
                    {
                        TempData[Constants.Message] = $"Pacijent {pacijent.Ime} uspješno ažuriran.";
                        TempData[Constants.ErrorOccurred] = false;
                        await ctx.SaveChangesAsync();
                        return RedirectToAction(nameof(Index2), new { page, sort, ascending });
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                        return View("EditPacijent", pacijent);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o pacijentu nije moguće povezati s forme.");
                    return View("EditPacijent", pacijent);
                }
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), new { MBO, page, sort, ascending });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int MBO, int page = 1, int sort = 1, bool ascending = true)
        {
            var pacijent = ctx.Pacijenti.Find(MBO);
            if (pacijent == null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    string ime = pacijent.Ime;
                    string prezime = pacijent.Prezime;
                    ctx.Remove(pacijent);
                    ctx.SaveChanges();
                    TempData[Constants.Message] = $"Pacijent {ime} {prezime} uspješno obrisan.";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] =
                        $"Pogreška prilikom brisanja pacijenta." + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                }

                return RedirectToAction(nameof(Index2), new { page, sort, ascending });
            }
        }

        // public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
        // {
        //     int pagesize = _appSettings.PageSize;
        //     var query = ctx.Pacijenti.AsNoTracking();
        //
        //     int count = query.Count();
        //
        //     var pagingInfo = new PagingInfo
        //     {
        //         CurrentPage = page,
        //         Sort = sort,
        //         Ascending = ascending,
        //         ItemsPerPage = pagesize,
        //         TotalItems = count
        //     };
        //
        //     if (page > pagingInfo.TotalPages)
        //     {
        //         return RedirectToAction(nameof(Index), new { page = pagingInfo.TotalPages, sort, ascending });
        //     }
        //
        //     Expression<Func<Pacijent, object>> orderSelector = null;
        //     switch (sort)
        //     {
        //         case 1:
        //             orderSelector = d => d.MBO;
        //             break;
        //         case 2:
        //             orderSelector = d => d.Ime;
        //             break;
        //         case 3:
        //             orderSelector = d => d.Prezime;
        //             break;
        //         case 4:
        //             orderSelector = d => d.DatumRodjenja;
        //             break;
        //     }
        //
        //     if (orderSelector != null)
        //     {
        //         query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
        //     }
        //
        //
        //     var pacijenti = query
        //         .Select(o => new PacijentViewModel2
        //         {
        //             MBO = o.MBO,
        //             Ime = o.Ime,
        //             Prezime = o.Prezime,
        //             DatumRodjenja = o.DatumRodjenja
        //         })
        //         .Skip((page - 1) * pagesize)
        //         .Take(pagesize)
        //         .ToList();
        //
        //     var model = new PacijentiViewModel2
        //     {
        //         Pacijenti = pacijenti,
        //         PagingInfo = pagingInfo
        //     };
        //     return View("", model);
        // }

        public IActionResult Index2(int page = 1, int sort = 1, bool ascending = true, string search = "")
        {
            int pagesize = _appSettings.PageSize;
            var query = ctx.Pacijenti.AsNoTracking();

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
                return RedirectToAction(nameof(Index2), new { page = pagingInfo.TotalPages, sort, ascending });
            }

            Expression<Func<Pacijent, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = d => d.MBO;
                    break;
                case 2:
                    orderSelector = d => d.Ime;
                    break;
                case 3:
                    orderSelector = d => d.Prezime;
                    break;
                case 4:
                    orderSelector = d => d.DatumRodjenja;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(k => k.Ime.ToLower().Contains(search.ToLower()) || k.Prezime.ToLower().Contains(search.ToLower()));
            }

            var pacijenti = query
                .Select(o => new PacijentViewModel
                {
                    MBO = o.MBO,
                    Ime = o.Ime,
                    Prezime = o.Prezime,
                    DatumRodjenja = o.DatumRodjenja
                })
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();

            foreach (PacijentViewModel pacijent in pacijenti)
            {
                var povijestPregleda = ctx.Pregledi
                    .Where(o => o.PacijentMbo == pacijent.MBO)
                    .OrderBy(o => o.PacijentMbo)
                    .Select(o => new PovijestPregledaViewModel
                    {
                        Id = o.Id,
                        DatumPregleda = o.DatumPregleda,
                        Anamneza = o.Anamneza,
                        Terapija = o.Terapija,
                        MBO = o.PacijentMbo,
                        MKB10 = o.DijagnozaMkb10
                    })
                    .ToList();
                pacijent.PovijestiPregleda = povijestPregleda;
            }

            var model = new PacijentiViewModel
            {
                Pacijenti = pacijenti,
                PagingInfo = pagingInfo,
                search = search
            };
            return View("Pacijenti", model);
        }

        public IActionResult PovijestPregleda(int MBO, int page = 1, int sort = 1, bool ascending = true)
        {
            ViewBag.Page = page;
            ViewBag.Sort = sort;
            ViewBag.Ascending = ascending;
            
            int pagesize = _appSettings.PageSize;


            var pacijent = ctx.Pacijenti
                .Where(o => o.MBO == MBO)
                .Select(o => new PacijentViewModel
                {
                    MBO = o.MBO,
                    Ime = o.Ime,
                    Prezime = o.Prezime,
                    DatumRodjenja = o.DatumRodjenja
                })
                .FirstOrDefault();
            if (pacijent == null)
            {
                return NotFound($"Pacijent sa MBO = {MBO} ne postoji.");
            }
            else
            {
                var povijestiPregleda = ctx.Pregledi
                    .Where(o => o.PacijentMbo == pacijent.MBO)
                    .OrderBy(o => o.DatumPregleda)
                    .Select(o => new PovijestPregledaViewModel()
                    {
                        Id = o.Id,
                        DatumPregleda = o.DatumPregleda,
                        Anamneza = o.Anamneza,
                        Terapija = o.Terapija,
                        MBO = o.PacijentMbo,
                        MKB10 = o.DijagnozaMkb10
                    })
                    .ToList();
                
                int count = povijestiPregleda.Count;

                var pagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    Sort = sort,
                    Ascending = ascending,
                    ItemsPerPage = pagesize,
                    TotalItems = count
                };
                
                var pregledi = new PacijentPreglediViewModel()
                {
                    pacijent = pacijent,
                    PovijestiPregleda = povijestiPregleda,
                    PagingInfo = pagingInfo
                };
                return View(pregledi);
            }
        }
        
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public IActionResult Add(Pacijent pacijent)
        // {
        //     if (pacijent == null)
        //     {
        //         return NotFound($"Pacijent sa MBO = {pacijent.MBO} ne postoji.");
        //     }
        //     ctx.Add(pacijent);
        //     ctx.SaveChanges();
        //
        //     return View(Index);
        // }
    }
}