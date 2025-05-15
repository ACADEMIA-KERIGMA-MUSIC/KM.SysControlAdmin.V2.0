#region REFERENCIAS
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.BL.Course___BL;
using KM.SysControlAdmin.BL.Schedule___BL;
using KM.SysControlAdmin.BL.Trainer___BL;
using KM.SysControlAdmin.Core.Utils;
using KM.SysControlAdmin.EN.Course___EN;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


#endregion

namespace KM.SysControlAdmin.WebApp.Controllers.Course___Controller
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Desarrollador, Administrador, Secretario/a")]
    public class CourseController : Controller
    {
        // Creamos las instancias para acceder a los metodos
        CourseBL courseBL = new CourseBL();
        TrainerBL trainerBL = new TrainerBL();
        ScheduleBL scheduleBL = new ScheduleBL();

        #region METODO PARA CREAR
        // Accion Para Mostrar La Vista De Crear
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public async Task<IActionResult> CreateCourse()
        {
            ViewBag.Trainers = await trainerBL.GetAllAsync();
            ViewBag.Schedule = await scheduleBL.GetAllAsync();
            ViewBag.Error = "";
            return View();
        }

        // Accion Que Recibe Los Datos Del Formulario Para Ser Enviados a La BD
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(Course course)
        {
            try
            {
                course.DateCreated = DateTime.Now.GetFechaZonaHoraria();
                course.DateModification = DateTime.Now.GetFechaZonaHoraria();
                int result = await courseBL.CreateAsync(course);
                TempData["SuccessMessageCreate"] = "Curso Agregado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Trainers = await trainerBL.GetAllAsync();
                ViewBag.Schedule = await scheduleBL.GetAllAsync();
                return View(course);
            }
        }
        #endregion
    }
}
