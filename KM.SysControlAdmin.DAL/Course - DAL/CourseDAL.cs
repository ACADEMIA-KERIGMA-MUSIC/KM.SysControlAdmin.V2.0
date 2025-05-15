#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.EN.Course___EN;
using Microsoft.EntityFrameworkCore;

#endregion

namespace KM.SysControlAdmin.DAL.Course___DAL
{
    public class CourseDAL
    {
        #region METODO PARA VALIDAR UNICA EXISTENCIA DEL REGISTRO Y METODO PARA VALIDAR STATUS DE SCHEDULE
        // Metodo Para Validar La Unica Existencia De Un Registro y No Haber Duplicidad
        private static async Task<bool> ExistCourse(Course course, ContextDB contextDB)
        {
            bool result = false;
            var courses = await contextDB.Course.FirstOrDefaultAsync(c => c.Code == course.Code && c.Id != course.Id);
            if (courses != null && courses.Id > 0 && courses.Code == course.Code)
                result = true;
            return result;
        }

        // Metodo Para Validar El Status Del Horario Seleccionado
        private static async Task<bool> StatusSchedule(int scheduleId, ContextDB contextDB)
        {
            var schedule = await contextDB.Schedule.FirstOrDefaultAsync(s => s.Id == scheduleId);

            if (schedule == null)
            {
                throw new Exception("El horario especificado no existe.");
            }

            return schedule.Status == 1; // Retorna true si el Schedule está activo (Status == 1)
        }

        private static async Task<bool> IsTrainerActive(int trainerId, ContextDB contextDB)
        {
            var trainer = await contextDB.Trainer.FirstOrDefaultAsync(t => t.Id == trainerId);

            if (trainer == null)
            {
                throw new Exception("El Instructor especificado no existe");
            }

            return trainer.Status == 1; // Retorna true si el Trainer está activo (Status == 1)
        }

        #endregion

        #region METODO PARA CREAR
        public static async Task<int> CreateAsync(Course course)
        {
            if (course == null)
            {
                throw new ArgumentNullException(nameof(course), "El curso no puede ser nulo.");
            }

            int result = 0;

            using (var dbContext = new ContextDB())
            {
                // Verificar si el curso ya existe
                bool courseExists = await ExistCourse(course, dbContext);
                if (courseExists)
                {
                    throw new Exception("Curso ya existente, vuelve a intentarlo nuevamente.");
                }

                // Validar que el horario esté activo
                if (!await StatusSchedule(course.IdSchedule, dbContext))
                {
                    throw new Exception("Horario No Disponible o Inactivo, Intenta Con Otro Horario.");
                }

                // Validar que el Instructor este activo
                if (!await IsTrainerActive(course.IdTrainer, dbContext))
                {
                    throw new Exception("Instructor No Disponible o Inactivo, Intenta Con Otro Instructor.");
                }

                // Guardar el curso en la base de datos
                dbContext.Course.Add(course);
                result = await dbContext.SaveChangesAsync();
            }

            return result;
        }
        #endregion
    }
}
