#region REFERENCIAS
// Referencias Necesarias Para El Correcto Funcionamiento
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


#endregion

namespace KM.SysControlAdmin.WebApp.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Desarrollador, Administrador, Instructor, Alumno/a, Secretario/a, Invitado")]
    public class HomeController : Controller
    {
        [Authorize(Roles = "Desarrollador, Administrador, Instructor, Alumno/a, Secretario/a, Invitado")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
