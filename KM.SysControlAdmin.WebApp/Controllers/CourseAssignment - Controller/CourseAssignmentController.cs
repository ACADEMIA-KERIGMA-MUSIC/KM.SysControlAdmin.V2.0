#region REFERENCIAS
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.BL.Course___BL;
using KM.SysControlAdmin.BL.CourseAssignment___BL;
using KM.SysControlAdmin.BL.Student___BL;
using KM.SysControlAdmin.Core.Utils;
using KM.SysControlAdmin.EN.CourseAssignment___EN;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


#endregion

namespace KM.SysControlAdmin.WebApp.Controllers.CourseAssignment___Controller
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Desarrollador, Administrador, Secretario/a")]
    public class CourseAssignmentController : Controller
    {
        // Creamos las instancias para acceder a los metodos
        CourseAssignmentBL courseAssignmentBL = new CourseAssignmentBL();
        StudentBL studentBL = new StudentBL();
        CourseBL courseBL = new CourseBL();

        #region METODOS PARA AUTOCOMPLETADO
        // Metod que extrae por Id y devolver a la vista en foramto Json
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        [HttpGet]
        public async Task<IActionResult> GetStudentDetails(int id)
        {
            var student = await studentBL.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var studentDetails = new
            {
                StudentCode = student.StudentCode,
                ProjectCode = string.IsNullOrWhiteSpace(student.ProjectCode) ? "NO APLICA" : student.ProjectCode,
                ParticipantCode = string.IsNullOrWhiteSpace(student.ParticipantCode) ? "NO APLICA" : student.ParticipantCode,
                DateOfBirth = student.DateOfBirth.ToString("dd/MM/yyyy"),
                Age = student.Age + " Años",
                ChurchName = string.IsNullOrWhiteSpace(student.ChurchName) ? "NO APLICA" : student.ChurchName,
                ImageData = student.ImageData
            };
            return Json(studentDetails);
        }

        // Metod que extrae por Id y devolver a la vista en formato Json
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        [HttpGet]
        public async Task<IActionResult> GetCourseDetails(int id)
        {
            var course = await courseBL.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var courseDetails = new
            {
                Code = course.Code,
                Name = course.Name,
                StartTime = course.StartTime.ToString("dd/MM/yyyy"),
                EndTime = course.EndTime.ToString("dd/MM/yyyy"),
                MaxStudent = course.MaxStudent,
                Schedule = course.IdSchedule != null && course.Schedule?.StartTime != null && course.Schedule?.EndTime != null
                    ? $"{((TimeSpan)course.Schedule.StartTime):hh\\:mm} - {((TimeSpan)course.Schedule.EndTime):hh\\:mm}"
                    : null,
                Trainer = course.IdTrainer != null
                    ? $"{course.Trainer.Name} {course.Trainer.LastName} - {course.Trainer.Code}"
                    : null,
                Status = course.Status == 1 ? "Activo" : course.Status == 2 ? "Inactivo" : "Desconocido" // Lógica de validación
            };
            return Json(courseDetails);
        }
        #endregion

        #region METODO PARA CREAR
        // Accion Para Mostrar La Vista De Crear
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public async Task<IActionResult> CreateCourseAssignment()
        {
            ViewBag.Student = await studentBL.GetAllAsync();
            ViewBag.Course = await courseBL.GetAllAsync();
            ViewBag.Error = "";
            return View();
        }

        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourseAssignment(CourseAssignment courseAssignment)
        {
            try
            {
                courseAssignment.DateCreated = DateTime.Now.GetFechaZonaHoraria();
                courseAssignment.DateModification = DateTime.Now.GetFechaZonaHoraria();
                int result = await courseAssignmentBL.CreateAsync(courseAssignment);
                TempData["SuccessMessageCreate"] = "Asignacion Agregada Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Student = await studentBL.GetAllAsync();
                ViewBag.Course = await courseBL.GetAllAsync();
                return View(courseAssignment);
            }
        }
        #endregion

        #region METODO PARA MOSTRAR INDEX
        // Accion Para Mostrar La Vista Index
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public async Task<IActionResult> Index(CourseAssignment courseAssignment = null!)
        {
            if (courseAssignment == null)
                courseAssignment = new CourseAssignment();

            var courseAssignments = await courseAssignmentBL.SearchIncludeAsync(courseAssignment);
            var student = await studentBL.GetAllAsync();
            var course = await courseBL.GetAllAsync();

            ViewBag.Students = student;
            ViewBag.Courses = course;
            return View(courseAssignments);
        }
        #endregion

        #region METODO PARA MODIFICAR
        // Acción que muestra la vista de modificar
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public async Task<IActionResult> EditCourseAssignment(int id)
        {
            try
            {
                CourseAssignment courseAssignment = await courseAssignmentBL.GetByIdAsync(new CourseAssignment { Id = id });
                if (courseAssignment == null)
                {
                    return NotFound();
                }
                ViewBag.Student = await studentBL.GetAllAsync();
                ViewBag.Course = await courseBL.GetAllAsync();
                return View(courseAssignment);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // Acción que recibe los datos del formulario para ser enviados a la base de datos
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourseAssignment(int id, CourseAssignment courseAssignment)
        {
            try
            {
                if (id != courseAssignment.Id)
                {
                    return BadRequest();
                }
                courseAssignment.DateModification = DateTime.Now.GetFechaZonaHoraria();
                int result = await courseAssignmentBL.UpdateAsync(courseAssignment);
                TempData["SuccessMessageUpdate"] = "Asingacion Modificada Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Student = await studentBL.GetAllAsync();
                ViewBag.Course = await courseBL.GetAllAsync();
                return View(courseAssignment);
            }
        }
        #endregion
    }
}
