#region REFERENCIAS
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.BL.Role___BL;
using KM.SysControlAdmin.Core.Utils;
using KM.SysControlAdmin.EN.Role___EN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


#endregion

namespace KM.SysControlAdmin.WebApp.Controllers.Role___Controller
{
    public class RoleController : Controller
    {
        // Creamos Las Instancias Para Acceder a Los Metodos
        RoleBL roleBL = new RoleBL();

        #region METODO PARA GUARDAR
        // Metodo Para Mostrar La Vista Guardar
        public IActionResult CreateRole()
        {
            ViewBag.Error = "";
            return View();
        }

        // Metodo Que Recibe y Envia a La Base De Datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(Role role)
        {
            try
            {
                role.Status = 1;
                role.DateCreated = DateTime.Now.GetFechaZonaHoraria();
                role.DateModification = DateTime.Now.GetFechaZonaHoraria();
                int result = await roleBL.CreateAsync(role);
                TempData["SuccessMessageCreate"] = "Rol Agregado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(role);
            }
        }
        #endregion

        #region METODO PARA INDEX
        // Metodo Para Mostrar La Vista Index
        public async Task<IActionResult> Index(Role role = null!)
        {
            if (role == null)
                role = new Role();

            var roles = await roleBL.SearchAsync(role);
            return View(roles);
        }
        #endregion

        #region METODO PARA MODIFICAR
        // Metodo Para Mostrar La Vista De Modificar
        public async Task<IActionResult> EditRole(int id)
        {
            var role = await roleBL.GetByIdAsync(new Role { Id = id });
            ViewBag.Error = "";
            return View(role);
        }

        // Metodo Que Recibe y Envia a La Base De Datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(Role role)
        {
            try
            {
                role.DateModification = DateTime.Now.GetFechaZonaHoraria();
                int result = await roleBL.UpdateAsync(role);
                TempData["SuccessMessageUpdate"] = "Rol Modificado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(role);
            }
        }
        #endregion

        #region METODO PARA ELIMINAR
        // Metodo Para Mostrar La Vista De Eliminar
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await roleBL.GetByIdAsync(new Role { Id = id });
            ViewBag.Error = "";
            return View(role);
        }

        // Metodo Que Recibe y Envia a La Base De Datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(int id, Role role)
        {
            try
            {
                int result = await roleBL.DeleteAsync(role);
                TempData["SuccessMessageDelete"] = "Rol Eliminado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(role);
            }
        }
        #endregion
    }
}
