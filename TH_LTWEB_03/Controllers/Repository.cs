using Microsoft.AspNetCore.Mvc;
using TH_LTWEB_03.Models;
using TH_LTWEB_03.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace TH_LTWEB_03.Controllers
{
    public class Repository : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
