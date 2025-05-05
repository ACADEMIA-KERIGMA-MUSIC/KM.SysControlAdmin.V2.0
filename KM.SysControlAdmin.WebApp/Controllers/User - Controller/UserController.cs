#region REFERENCIAS
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.BL.Role___BL;
using KM.SysControlAdmin.BL.User___BL;
using KM.SysControlAdmin.Core.Utils;
using KM.SysControlAdmin.EN.User___EN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


#endregion

namespace KM.SysControlAdmin.WebApp.Controllers.User___Controller
{
    public class UserController : Controller
    {
        // Creamos Las Instancias Para Acceder a Los Metodos
        UserBL userBL = new UserBL();
        RoleBL roleBL = new RoleBL();

        #region METODO PARA GUARDAR
        // Accion Que Muestra El Formulario
        public async Task<IActionResult> CreateUser()
        {
            var roles = await roleBL.GetAllAsync();
            ViewBag.Roles = roles;
            return View();
        }

        // Accion Que Recibe Los Datos y Los Envia a La Base De Datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(User user, IFormFile imagen)
        {
            try
            {
                const int maxFileSize = 1572864; // 1.5 MB
                if (imagen.Length > maxFileSize)
                {
                    throw new Exception("La imagen no debe pesar mas de 1.5MB.");
                }

                // Mapeo de img a ArrayByte
                if (imagen != null && imagen.Length > 0)
                {
                    byte[] imagenData = null!;
                    using (var memoryStream = new MemoryStream())
                    {
                        await imagen.CopyToAsync(memoryStream);
                        imagenData = memoryStream.ToArray();
                    }

                    user.ImageData = imagenData; // Asigna el array de bytes al campo de la imagen en tu modelo Membership
                }
                user.DateCreated = DateTime.Now.GetFechaZonaHoraria();
                user.DateModification = DateTime.Now.GetFechaZonaHoraria();
                int result = await userBL.CreateAsync(user);
                TempData["SuccessMessageCreate"] = "Usuario Agregado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                ViewBag.Roles = await roleBL.GetAllAsync();
                return View(user);
            }
        }
        #endregion
    }
}
