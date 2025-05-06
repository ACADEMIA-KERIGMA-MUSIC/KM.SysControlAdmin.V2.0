#region REFERENCIAS
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.BL.Role___BL;
using KM.SysControlAdmin.BL.User___BL;
using KM.SysControlAdmin.EN.User___EN;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


#endregion

namespace KM.SysControlAdmin.WebApp.Controllers.Support___Controller
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Desarrollador, Administrador")]
    public class SupportController : Controller
    {
        // Creamos Las Instancias Para Acceder a Los Metodos
        UserBL userBL = new UserBL();
        RoleBL roleBL = new RoleBL();

        #region METODO PARA INDEX
        // Metodo Para Mostrar La Vista Index
        [Authorize(Roles = "Desarrollador, Administrador")]
        public async Task<IActionResult> Index(User user = null!)
        {
            if (user == null)
                user = new User();

            var users = await userBL.SearchIncludeRoleAsync(user);
            var roles = await roleBL.GetAllAsync();

            ViewBag.Roles = roles;

            return View(users);
        }
        #endregion

        #region METODO PARA CAMBIAR CONTRASEÑA COMO ADMIN
        // Acción que muestra el formulario de cambio de contraseña para un usuario específico
        [Authorize(Roles = "Desarrollador, Administrador")]
        public async Task<IActionResult> ChangePasswordForUser(int id)
        {
            var user = await userBL.GetByIdAsync(new User { Id = id });
            return View(user);
        }

        // Acción que procesa el cambio de contraseña para un usuario específico
        [Authorize(Roles = "Desarrollador, Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePasswordForUser(User user)
        {
            try
            {
                int result = await userBL.ChangePasswordRoleDesAsync(user);
                TempData["SuccessMessageUpdate"] = "Credencial Modificada Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                var existingUser = await userBL.GetByIdAsync(new User { Id = user.Id });
                return View(existingUser);
            }
        }
        #endregion
    }
}
