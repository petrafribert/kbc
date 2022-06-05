#pragma warning disable CS1591

using System.Threading.Tasks;
using KBC.ViewModels;
using KBC.ViewModels.JTable;
using Microsoft.AspNetCore.Mvc;

namespace KBC.ApiControllers.JTable
{
    [Route("jtable/pacijent/[action]")]
    public class PacijentJTableController : JTableController<ApiPacijentController, int, PacijentViewModel2>
    {
        public PacijentJTableController(ApiPacijentController controller) : base(controller)
        {
        }

        [HttpPost]
        public async Task<JTableAjaxResult> Update([FromForm] PacijentViewModel2 model)
        {
            return await base.UpdateItem(model.MBO, model);
        }

        [HttpPost]
        public async Task<JTableAjaxResult> Delete([FromForm] int MBO)
        {
            return await base.DeleteItem(MBO);
        }
    }
}