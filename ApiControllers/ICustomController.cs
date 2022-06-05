#pragma warning disable CS1591

using System.Collections.Generic;
using System.Threading.Tasks;
using KBC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace KBC.ApiControllers
{
    public interface ICustomController<TKey, TModel>
    {

        public Task<int> Count([FromQuery] string filter);
        public Task<List<TModel>> GetAll([FromQuery] LoadParams loadParams);
        public Task<ActionResult<TModel>> Get(TKey id);
        public Task<IActionResult> Create(TModel model);
        public Task<IActionResult> Update(TKey id, TModel model);
        public Task<IActionResult> Delete(TKey id);
    }

}

