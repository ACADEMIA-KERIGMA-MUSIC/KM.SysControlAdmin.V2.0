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

        #region METODO PARA MOSTRAR
        // Metodo Para Mostrar La Lista De Registros
        public static async Task<List<Course>> GetAllAsync()
        {
            var courses = new List<Course>();
            using (var dbContext = new ContextDB())
            {
                courses = await dbContext.Course.ToListAsync();
            }
            return courses;
        }
        #endregion

        #region METODO PARA OBTENER POR ID
        // Metodo Para Mostrar Un Registro En Base A Su Id
        public static async Task<Course> GetByIdAsync(Course course)
        {
            var courseDB = new Course();

            // Un bloque de conexión que mientras se permanezca en el bloque, la base de datos permanecerá abierta y al terminar se destruirá
            using (var dbContext = new ContextDB())
            {
                courseDB = await dbContext.Course
                    .Include(c => c.Schedule).Include(c => c.Trainer)
                    .FirstOrDefaultAsync(m => m.Id == course.Id);
            }

            return courseDB!; // Retornamos el registro encontrado
        }

        // Normalmente el metodo deberia ser asi como el siguiente pero, por modificaciones en
        // el proceso de servidores se a modificado pero se deja la forma normal para aprenderla.
        // *********** Metodo Para Mostrar Un Registro En Base A Su Id *************
        //public static async Task<Course> GetByIdAsync(Course course)
        //{
        //    var courseDB = new Course();
        //    using (var dbContext = new ContextDB())
        //    {
        //        courseDB = await dbContext.Course.FirstOrDefaultAsync(c => c.Id == course.Id);
        //    }
        //    return courseDB!;
        //}
        #endregion

        #region METODO PARA BUSCAR REGISTROS MEDIANTE EL USO DE FILTROS
        // Metodo Para Buscar Por Filtros
        // IQueryable es una interfaz que toma un coleccion a la cual se le pueden implementar multiples consultas (Filtros)
        internal static IQueryable<Course> QuerySelect(IQueryable<Course> query, Course course)
        {
            // Por ID
            if (course.Id > 0)
                query = query.Where(c => c.Id == course.Id);

            // Por Su Instructor
            if (course.IdTrainer > 0)
                query = query.Where(c => c.IdTrainer == course.IdTrainer);

            // Por Codigo, Si es verdadero lo vuelve falso y viceversa 
            if (!string.IsNullOrWhiteSpace(course.Code))
                query = query.Where(c => c.Code.Contains(course.Code));

            // Por Nomnbre, Si es verdadero lo vuelve falso y viceversa 
            if (!string.IsNullOrWhiteSpace(course.Name))
                query = query.Where(c => c.Name.Contains(course.Name));

            // Por Su Estatus
            if (course.Status > 0)
                query = query.Where(c => c.Status == course.Status);

            // Ordenamos de Manera Desendente
            query = query.OrderByDescending(c => c.Id).AsQueryable();

            return query;
        }
        #endregion

        #region METODO PARA BUSCAR
        // Metodo para Buscar Registros Existentes
        public static async Task<List<Course>> SearchAsync(Course course)
        {
            var courses = new List<Course>();
            using (var dbContext = new ContextDB())
            {
                var select = dbContext.Course.AsQueryable();
                select = QuerySelect(select, course);
                courses = await select.ToListAsync();
            }
            return courses;
        }
        #endregion

        #region METODO PARA INCLUIR HORARIO E INSTRUCTOR
        // Método que incluye el Horario y el Instructor para la búsqueda
        public static async Task<List<Course>> SearchIncludeScheduleAndTrainerAsync(Course course)
        {
            var courses = new List<Course>();
            using (var dbContext = new ContextDB())
            {
                var select = dbContext.Course.AsQueryable();
                select = QuerySelect(select, course).Include(c => c.Schedule).Include(c => c.Trainer).AsQueryable();
                courses = await select.ToListAsync();
            }
            return courses;
        }
        #endregion

        #region METODO PARA MODIFICAR
        public static async Task<int> UpdateAsync(Course course)
        {
            int result = 0;

            using (var dbContext = new ContextDB())
            {
                var courseDB = await dbContext.Course.FirstOrDefaultAsync(c => c.Id == course.Id);
                if (courseDB == null)
                {
                    throw new Exception("Curso no encontrado para actualizar.");
                }

                // Verificar si ya existe otro curso con el mismo código
                bool courseExists = await dbContext.Course.AnyAsync(c => c.Id != course.Id && c.Code == course.Code);
                if (courseExists)
                {
                    throw new Exception("Ya existe otro curso con el mismo código. Vuelve a intentarlo.");
                }

                // Validar que el horario esté activo
                if (!await StatusSchedule(course.IdSchedule, dbContext))
                {
                    throw new Exception("El curso no puede modificarse porque el horario no está disponible.");
                }

                // Validar que el horario esté activo
                if (!await IsTrainerActive(course.IdTrainer, dbContext))
                {
                    throw new Exception("El curso no puede modificarse porque el Instructor no está disponible.");
                }

                // Actualizar los datos del curso
                courseDB.Code = course.Code;
                courseDB.Name = course.Name;
                courseDB.ExternalFee = course.ExternalFee;
                courseDB.ScholarshipFee = course.ScholarshipFee;
                courseDB.StartTime = course.StartTime;
                courseDB.EndTime = course.EndTime;
                courseDB.MaxStudent = course.MaxStudent;
                courseDB.IdSchedule = course.IdSchedule;
                courseDB.IdTrainer = course.IdTrainer;
                courseDB.Status = course.Status;
                courseDB.DateModification = course.DateModification;

                dbContext.Update(courseDB);
                result = await dbContext.SaveChangesAsync();
            }

            return result;
        }
        #endregion

        #region METODO PARA ELIMINAR
        // Metodo Para Eliminar Un Registro Existente En La Base De Datos
        public static async Task<int> DeleteAsync(Course course)
        {
            int result = 0;
            // Un bloque de conexion que mientras se permanezca en el bloque la base de datos permanecera abierta y al terminar se destruira
            using (var dbContext = new ContextDB())
            {
                var courseDB = await dbContext.Course.FirstOrDefaultAsync(c => c.Id == course.Id);
                if (courseDB != null)
                {
                    dbContext.Course.Remove(courseDB);
                    result = await dbContext.SaveChangesAsync();
                }
            }
            return result;
        }
        #endregion
    }
}
