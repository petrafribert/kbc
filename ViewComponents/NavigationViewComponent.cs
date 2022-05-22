using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KBC.ViewComponents {
    public class NavigationViewComponent : ViewComponent {

            public IViewComponentResult Invoke(){
                ViewBag.Controller = RouteData?.Values["Controller"];
                return View();
            }

    }
}
