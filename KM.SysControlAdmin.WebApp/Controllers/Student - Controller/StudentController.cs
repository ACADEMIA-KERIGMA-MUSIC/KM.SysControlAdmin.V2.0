#region REFERENCIAS
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.BL.Student___BL;
using KM.SysControlAdmin.BL.User___BL;
using KM.SysControlAdmin.Core.Utils;
using KM.SysControlAdmin.EN.Student___EN;
using KM.SysControlAdmin.EN.User___EN;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


#endregion

namespace KM.SysControlAdmin.WebApp.Controllers.Student___Controller
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Desarrollador, Administrador, Secretario/a")]
    public class StudentController : Controller
    {
        // Creamos Una Instancia Para Acceder a Los Metodos
        StudentBL studentBL = new StudentBL();
        UserBL userBL = new UserBL();

        #region METODO PARA CREAR (ALUMNO BECADO)
        // Accion Para Mostrar La Vista De Crear
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public ActionResult CreateScholarshipForm()
        {
            ViewBag.Error = "";
            return View();
        }

        // Accion Que Recibe Los Datos Del Formulario Para Ser Enviados a La BD
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateScholarshipForm(Student student, IFormFile imagen)
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
                    student.ImageData = imagenData; // Asigna el array de bytes al campo de la imagen en tu modelo Membership
                }
                student.DateCreated = DateTime.Now.GetFechaZonaHoraria();
                student.DateModification = DateTime.Now.GetFechaZonaHoraria();
                if (student.ProjectCode != "" && student.ParticipantCode != "")
                {
                    int result = await studentBL.CreateAsync(student);
                }

                // Crear un nuevo objeto de tipo User y mapear las propiedades de Trainer con Server
                var user = new User
                {
                    IdRole = 4,
                    Name = student.Name,
                    LastName = student.LastName,
                    Email = student.Email,
                    Password = student.Password,
                    Status = student.Status,
                    DateCreated = student.DateCreated,
                    DateModification = student.DateModification,
                    ImageData = student.ImageData,
                    RecoveryEmail = student.PersonalEmail,
                };

                // Guardar en la tabla User
                if (student.ProjectCode != "" && student.ParticipantCode != "")
                {
                    int resultUser = await userBL.CreateAsync(user);
                }
                TempData["SuccessMessageCreate"] = "Alumno/a Agregado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(student);
            }
        }
        #endregion

        #region METODO PARA CREAR (ALUMNO EXTERNO)
        // Accion Para Mostrar La Vista De Crear
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public ActionResult CreateExternalForm()
        {
            ViewBag.Error = "";
            return View();
        }

        // Accion Que Recibe Los Datos Del Formulario Para Ser Enviados a La BD
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExternalForm(Student student, IFormFile imagen)
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
                    student.ImageData = imagenData; // Asigna el array de bytes al campo de la imagen en tu modelo Membership
                }
                student.DateCreated = DateTime.Now.GetFechaZonaHoraria();
                student.DateModification = DateTime.Now.GetFechaZonaHoraria();
                if (student.ChurchName != "")
                {
                    int result = await studentBL.CreateAsync(student);
                }

                // Crear un nuevo objeto de tipo User y mapear las propiedades de Trainer con Server
                var user = new User
                {
                    IdRole = 4,
                    Name = student.Name,
                    LastName = student.LastName,
                    Email = student.Email,
                    Password = student.Password,
                    Status = student.Status,
                    DateCreated = student.DateCreated,
                    DateModification = student.DateModification,
                    ImageData = student.ImageData,
                    RecoveryEmail = student.PersonalEmail,
                };

                if (student.ChurchName != "")
                {
                    // Guardar en la tabla User
                    int resultUser = await userBL.CreateAsync(user);
                }
                TempData["SuccessMessageCreate"] = "Alumno/a Agregado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(student);
            }
        }
        #endregion
    }
}
