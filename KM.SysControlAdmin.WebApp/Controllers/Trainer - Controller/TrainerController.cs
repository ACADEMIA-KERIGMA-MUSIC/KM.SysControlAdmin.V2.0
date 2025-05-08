#region REFERENCIAS
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.BL.Trainer___BL;
using KM.SysControlAdmin.BL.User___BL;
using KM.SysControlAdmin.Core.Utils;
using KM.SysControlAdmin.EN.Trainer___EN;
using KM.SysControlAdmin.EN.User___EN;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


#endregion

namespace KM.SysControlAdmin.WebApp.Controllers.Trainer___Controller
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Desarrollador, Administrador, Secretario/a")]
    public class TrainerController : Controller
    {
        // Creamos Una Instancia Para Acceder a Los Metodos
        TrainerBL trainerBL = new TrainerBL();
        UserBL userBL = new UserBL();

        #region METODO PARA CREAR
        // Accion Para Mostrar La Vista De Crear
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public ActionResult CreateTrainer()
        {
            ViewBag.Error = "";
            return View();
        }

        // Accion Que Recibe Los Datos Del Formulario Para Ser Enviados a La BD
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrainer(Trainer trainer, IFormFile imagen)
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

                    trainer.ImageData = imagenData; // Asigna el array de bytes al campo de la imagen en tu modelo Membership
                }
                trainer.DateCreated = DateTime.Now.GetFechaZonaHoraria();
                trainer.DateModification = DateTime.Now.GetFechaZonaHoraria();
                int result = await trainerBL.CreateAsync(trainer);

                // Crear un nuevo objeto de tipo User y mapear las propiedades de Trainer con Server
                var user = new User
                {
                    IdRole = 3,
                    Name = trainer.Name,
                    LastName = trainer.LastName,
                    Email = trainer.Email,
                    Password = trainer.Password,
                    Status = trainer.Status,
                    DateCreated = trainer.DateCreated,
                    DateModification = trainer.DateModification,
                    ImageData = trainer.ImageData,
                    RecoveryEmail = trainer.PersonalEmail,
                };

                // Guardar en la tabla User
                int resultUser = await userBL.CreateAsync(user);
                TempData["SuccessMessageCreate"] = "Instructor/Docente Agregado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(trainer);
            }
        }
        #endregion
    }
}
