#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KBC.Model;
using KBC.Util;

namespace KBC.Controllers
{
    public class PrijavaPregledaController : Controller
    {
        private const string AdminGroup = "Admin";
        private const string PacijentiGroup = "Pacijenti";

        public async Task<IActionResult> Index(string user)
        {
            DashboardData data = new DashboardData();
            data.ProcessInstances = await CamundaUtil.GetPrijave();
            data.MojiPreglediBezTermina = await CamundaUtil.GetTasks(user);
            if (await CamundaUtil.IsUserInGroup(user, AdminGroup))
            {
                data.AdminTasks = await CamundaUtil.UnAssignedGroupTasks(AdminGroup);
            }

            if (await CamundaUtil.IsUserInGroup(user, PacijentiGroup))
            {
                data.MojiPreglediPonudeniTermin = await CamundaUtil.AssignedTasks(user);
            }
            

            data.user = user;

            return View("Dashboard", data);
        }

        [HttpPost]
        public async Task<IActionResult> PreuzmiPregled(string user, string pid)
        {
            await CamundaUtil.PreuzmiPregled(pid, user);
            return RedirectToAction(nameof(Index), new { user });
        }

        [HttpPost]
        public async Task<IActionResult> PickTask(string user, string taskId)
        {
            await CamundaUtil.PickTask(taskId, user);
            return RedirectToAction(nameof(Index), new { user });
        }

        [HttpPost]
        public async Task<IActionResult> ZavrsiZadatak(string user, string taskId)
        {
            await CamundaUtil.ZavrsiZadatak(taskId);
            return RedirectToAction(nameof(Index), new { user });
        }

        [HttpPost]
        public async Task<IActionResult> ZavrsiPrijavu(string user, string taskId, DateTime datum)
        {
            
            await CamundaUtil.ZavrsiPrijavu(taskId, datum.ToString("dd-MM-yyyy"));
            return RedirectToAction(nameof(Index), new { user });
        }

        [HttpPost]
        public async Task<IActionResult> DodijeliDoktora(string user, string doktor, string taskId)
        {
            await CamundaUtil.DodijeliDoktora(taskId, doktor);
            return RedirectToAction(nameof(Index), new { user });
        }
        
        [HttpPost]
        public async Task<IActionResult> PotvrdiTermin(string user, string odluka, string taskId)
        {
            await CamundaUtil.PotvrdiTermin(taskId, odluka == "DA" ? true : false);
            return RedirectToAction(nameof(Index), new { user });
        }

        [HttpGet]
        public IActionResult Start(string Pacijent)
        {
            return View("KreirajPrijavu");
        }

        [HttpPost]
        public async Task<IActionResult> Start(int MBO)
        {
            var pid = await CamundaUtil.KreirajPrijavuPregleda(MBO);

            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult<string>> Diagram()
        {
            var xml = await CamundaUtil.GetXmlDefinition();
            return xml;
        }

    }
}
