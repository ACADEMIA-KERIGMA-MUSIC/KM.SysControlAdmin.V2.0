using System.Diagnostics;
using KM.SysControlAdmin.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace KM.SysControlAdmin.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
