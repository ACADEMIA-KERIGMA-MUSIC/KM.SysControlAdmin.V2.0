﻿#region REFERENCIAS
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.BL.Trainer___BL;
using KM.SysControlAdmin.BL.User___BL;
using KM.SysControlAdmin.Core.Utils;
using KM.SysControlAdmin.EN.Course___EN;
using KM.SysControlAdmin.EN.Trainer___EN;
using KM.SysControlAdmin.EN.User___EN;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;


#endregion

namespace KM.SysControlAdmin.WebApp.Controllers.Trainer___Controller
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Desarrollador, Administrador, Secretario/a, Instructor")]
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
                    CodeUser = trainer.Code,
                };

                // Guardar en la tabla User
                int resultUser = await userBL.CreateAsync(user);
                TempData["SuccessMessageCreate"] = "Instructor Agregado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(trainer);
            }
        }
        #endregion

        #region METODO PARA MOSTRAR INDEX
        // Accion Para Mostrar La Vista Index
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public async Task<IActionResult> Index(Trainer trainer = null!)
        {
            if (trainer == null)
                trainer = new Trainer();

            var trainers = await trainerBL.SearchAsync(trainer);
            return View(trainers);
        }
        #endregion

        #region METODO PARA MODIFICAR
        // Acción que muestra la vista de modificar
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public async Task<IActionResult> EditTrainer(int id)
        {
            try
            {
                Trainer trainer = await trainerBL.GetByIdAsync(new Trainer { Id = id });
                if (trainer == null)
                {
                    return NotFound();
                }
                // Convertir el array de bytes en imagen para mostrar en la vista
                if (trainer.ImageData != null && trainer.ImageData.Length > 0)
                {
                    ViewBag.ImageUrl = Convert.ToBase64String(trainer.ImageData);
                }
                return View(trainer);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(); // Devolver la vista sin ningún objeto Membership
            }
        }

        // Acción que recibe los datos del formulario para ser enviados a la base de datos
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainer(int id, Trainer trainer, IFormFile imagen)
        {
            try
            {
                if (id != trainer.Id)
                {
                    return BadRequest();
                }
                if (imagen != null && imagen.Length > 0) // Verificar si se ha subido una nueva imagen
                {
                    const int maxFileSize = 1572864; // 1.5 MB
                    if (imagen.Length > maxFileSize)
                    {
                        throw new Exception("La imagen no debe pesar mas de los 1.5MB.");

                    }

                    byte[] imagenData = null!;
                    using (var memoryStream = new MemoryStream())
                    {
                        await imagen.CopyToAsync(memoryStream);
                        imagenData = memoryStream.ToArray();
                    }
                    trainer.ImageData = imagenData; // Asignar el array de bytes de la nueva imagen a la entidad Membership
                }
                else
                {
                    // Si no se proporciona una nueva imagen, se conserva la imagen existente
                    Trainer existingTrainer = await trainerBL.GetByIdAsync(new Trainer { Id = id });
                    trainer.ImageData = existingTrainer.ImageData;
                }
                trainer.DateModification = DateTime.Now.GetFechaZonaHoraria();
                int result = await trainerBL.UpdateAsync(trainer);
                TempData["SuccessMessageUpdate"] = "Instructor Modificado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(trainer); // Devolver la vista con el objeto Membership para que el usuario pueda corregir los datos
            }
        }
        #endregion

        #region METODO PARA MOSTRAR DETALLES
        // Accion Que Muestra El Detalle De Un Registro
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public async Task<IActionResult> DetailsTrainer(int id)
        {
            try
            {
                Trainer trainer = await trainerBL.GetByIdAsync(new Trainer { Id = id });
                if (trainer == null)
                {
                    return NotFound();
                }
                // Convertir el array de bytes en imagen para mostrar en la vista
                if (trainer.ImageData != null && trainer.ImageData.Length > 0)
                {
                    ViewBag.ImageUrl = Convert.ToBase64String(trainer.ImageData);
                }
                return View(trainer); // Retornamos los Detalles a La Vista
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(); // Devolver la vista sin ningún objeto Membership
            }
        }
        #endregion

        #region METODO PARA ELIMINAR
        // Accion Que Muestra La Vista De Eliminar
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public async Task<IActionResult> DeleteTrainer(int id)
        {
            try
            {
                Trainer trainer = await trainerBL.GetByIdAsync(new Trainer { Id = id });

                if (trainer == null)
                {
                    return NotFound();
                }
                // Convertir el array de bytes en imagen para mostrar en la vista
                if (trainer.ImageData != null && trainer.ImageData.Length > 0)
                {
                    ViewBag.ImageUrl = Convert.ToBase64String(trainer.ImageData);
                }
                return View(trainer);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(); // Devolver la vista sin ningún objeto Membership
            }
        }

        // Accion Que Recibe Los Datos Del Formulario Para Ser Enviados a La BD
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTrainer(int id, Trainer trainer)
        {
            try
            {
                Trainer trainerDB = await trainerBL.GetByIdAsync(trainer);
                int result = await trainerBL.DeleteAsync(trainerDB);
                TempData["SuccessMessageDelete"] = "Instructor/Docente Eliminado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                var trainerDB = await trainerBL.GetByIdAsync(trainer);
                if (trainerDB == null)
                    trainerDB = new Trainer();
                return View(trainerDB);
            }
        }
        #endregion

        #region METODO PARA REPORTE
        // Metodo Para Generar Ficha o Reporte En PDF
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public async Task<ActionResult> GeneratePDFfileTrainer(int id)
        {
            var generatePDF = await trainerBL.GetByIdAsync(new Trainer { Id = id });
            string fileName = $"FichaDocente_{generatePDF.Name}_{generatePDF.LastName}_{generatePDF.Dui}_KM.pdf";
            return new ViewAsPdf("GeneratePDFfileTrainer", generatePDF)
            {
                FileName = fileName
            };
        }
        #endregion

        #region METODO PARA OBTENER CURSOS ASIGNADOS SEGUN INSTRUCTOR
        // Metodo Para Mostrar Vista De Cursos Asignados Al Instructor Logiado
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> MyAssignedCourses()
        {
            try
            {
                // Buscar usuario logueado por email
                var users = await userBL.SearchIncludeRoleAsync(new User { Email = User.Identity!.Name! });
                var actualUser = users.FirstOrDefault();

                if (actualUser == null)
                {
                    ViewBag.Error = "No se encontró el usuario logueado.";
                    return View(new List<Course>());
                }

                // Obtener el código del trainer desde el usuario logueado
                string trainerCode = actualUser.CodeUser;

                // Obtener los cursos desde la BL
                var cursos = await trainerBL.GetCoursesByTrainerCodeAsync(trainerCode);

                return View(cursos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar los cursos: " + ex.Message;
                return View(new List<Course>());
            }
        }
        #endregion
    }
}
