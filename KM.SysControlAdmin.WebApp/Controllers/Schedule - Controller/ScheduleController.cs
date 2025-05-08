#region REFERENCIAS
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.BL.Schedule___BL;
using KM.SysControlAdmin.Core.Utils;
using KM.SysControlAdmin.EN.Schedule___EN;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


#endregion

namespace KM.SysControlAdmin.WebApp.Controllers.Schedule___Controller
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Desarrollador, Administrador, Secretario/a")]
    public class ScheduleController : Controller
    {
        // Instancia De La Clase Logica De Negocio
        ScheduleBL scheduleBL = new ScheduleBL();

        #region METODO PARA GUARDAR
        // Metodo Para Mostrar La Vista Guardar
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public IActionResult CreateSchedule()
        {
            ViewBag.Error = "";
            return View();
        }

        // Metodo Que Recibe y Envia a La Base De Datos
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSchedule(Schedule schedule)
        {
            try
            {
                schedule.Status = 1;
                schedule.DateCreated = DateTime.Now.GetFechaZonaHoraria();
                schedule.DateModification = DateTime.Now.GetFechaZonaHoraria();
                int result = await scheduleBL.CreateAsync(schedule);
                TempData["SuccessMessageCreate"] = "Horario Agregado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(schedule);
            }
        }
        #endregion
    }
}
