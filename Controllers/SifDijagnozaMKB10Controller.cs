#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KBC.Model;
using KBC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KBC.Controllers
{
    public class SifDijagnozaMKB10Controller : Controller
    {
        private readonly KBCGrupaContext ctx;
        private readonly AppSettings appSettings;

        public SifDijagnozaMKB10Controller(KBCGrupaContext ctx, IOptionsSnapshot<AppSettings> optionsSnapshot)
        {
            this.ctx = ctx;
            appSettings = optionsSnapshot.Value;
        }

        public IActionResult Index(int page = 1, int sort = 1, bool ascending = true, string search = "")
        {
            int pagesize = appSettings.PageSize;
            var query = ctx.SifDijagnozaMKB10.AsNoTracking();

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

            Expression<Func<SifDijagnozaMKB10, object>> orderSelector = null;
            switch (sort)
            {
                case 1:
                    orderSelector = d => d.mkb10;
                    break;
                case 2:
                    orderSelector = d => d.Dijagnoza;
                    break;
            }

            if (orderSelector != null)
            {
                query = ascending ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.Dijagnoza.ToLower().Contains(search.ToLower()));
            }


            var dijagnoze = query
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToList();
            var model = new DijagnozeViewModel
            {
                Dijagnoze = (IEnumerable<SifDijagnozaMKB10>)dijagnoze,
                PagingInfo = pagingInfo
            };
            return View("SifDijagnozaMKB10", model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("CreateSifDijagnozaMKB10");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SifDijagnozaMKB10 dijagnozaunos)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Add(dijagnozaunos);
                    ctx.SaveChanges();
                    TempData[Constants.Message] = $"Dijagnoza {dijagnozaunos.mkb10} uspješno dodana.";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                    return View("CreateSifDijagnozaMKB10", dijagnozaunos);
                }

            }
            else
            {
                return View("CreateSifDijagnozaMKB10", dijagnozaunos);
            }

        }


        [HttpGet]
        public IActionResult Edit(string MKB10, int page = 1, int sort = 1, bool ascending = true)
        {
            var odredenaDijagnoza = ctx.SifDijagnozaMKB10
                .AsNoTracking()
                .Where(d => d.mkb10 == MKB10)
                .SingleOrDefault();
            if (odredenaDijagnoza == null)
            {
                return NotFound($"Ne postoji dijagnoza s oznakom {MKB10}");
            }
            else
            {
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                return View("EditSifDijagnozaMKB10", odredenaDijagnoza);
            }
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string MKB10, int page = 1, int sort = 1, bool ascending = true)
        {
            try
            {
                SifDijagnozaMKB10 odredenaDijagnoza = await ctx.SifDijagnozaMKB10.Where(o => o.mkb10 == MKB10).FirstOrDefaultAsync();
                if (odredenaDijagnoza == null)
                {
                    return NotFound($"Ne postoji dijagnoza s oznakom {MKB10}.");
                }
                ViewBag.Page = page;
                ViewBag.Sort = sort;
                ViewBag.Ascending = ascending;
                bool ok = await TryUpdateModelAsync<SifDijagnozaMKB10>(odredenaDijagnoza, "", d => d.mkb10, d => d.Dijagnoza);
                if (ok)
                {
                    try
                    {
                        TempData[Constants.Message] = $"Dijagnoza {odredenaDijagnoza.mkb10} uspješno ažurirana.";
                        TempData[Constants.ErrorOccurred] = false;
                        await ctx.SaveChangesAsync();
                        return RedirectToAction(nameof(Index), new { page, sort, ascending });
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
                        return View("EditSifDijagnozaMKB10", odredenaDijagnoza);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Podatke o dijagnozi nije moguće povezati s forme.");
                    return View("EditSifDijagnozaMKB10", odredenaDijagnoza);
                }
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Edit), new { MKB10, page, sort, ascending });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string MKB10, int page = 1, int sort = 1, bool ascending = true)
        {
            var odredenaDijagnoza = ctx.SifDijagnozaMKB10.Find(MKB10);
            if (odredenaDijagnoza == null)
            {
                return NotFound("Nisam pronašao dijagnozu.");
            }
            else
            {
                try
                {
                    string mkb10 = odredenaDijagnoza.mkb10;
                    ctx.Remove(odredenaDijagnoza);
                    ctx.SaveChanges();
                    TempData[Constants.Message] = $"Dijagnoza {mkb10} uspješno obrisana.";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (Exception exc)
                {
                    TempData[Constants.Message] = $"Pogreška prilikom brisanja dijagnoze." + exc.CompleteExceptionMessage();
                    TempData[Constants.ErrorOccurred] = true;
                }
                return RedirectToAction(nameof(Index), new { page, sort, ascending });
            }
        }


    }
}