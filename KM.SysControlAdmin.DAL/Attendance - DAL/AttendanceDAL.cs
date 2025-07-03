#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.EN.Attendance___EN;
using Microsoft.EntityFrameworkCore;


#endregion

namespace KM.SysControlAdmin.DAL.Attendance___DAL
{
    public class AttendanceDAL
    {
        #region METODO PARA VALIDAR UNICA EXISTENCIA DEL REGISTRO Y OTRAS METODOS PARA VALIDACIONES EXTRAS
        // Metodo Para Validar La Unica Existencia De Un Registro y No Haber Duplicidad
        private static async Task<bool> ExistAttendance(Attendance attendance, ContextDB contextDB)
        {
            var existingAttendance = await contextDB.Attendance.FirstOrDefaultAsync(a =>
                a.IdStudent == attendance.IdStudent &&
                a.IdCourse == attendance.IdCourse &&
                a.Id != attendance.Id);

            return existingAttendance != null;
        }

        // Metodo Para Validara Si El Estudiante Esta Activo
        private static async Task<bool> IsStudentActive(int studentId, ContextDB contextDB)
        {
            var student = await contextDB.Student.FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null)
            {
                throw new Exception("El estudiante no existe.");
            }

            return student.Status == 1; // Retorna true si el estudiante está activo
        }

        // Metodo Para Validar Si El Curso Esta Activo
        private static async Task<bool> IsCourseActive(int courseId, ContextDB contextDB)
        {
            var course = await contextDB.Course.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null)
            {
                throw new Exception("El curso no existe.");
            }

            return course.Status == 1; // Retorna true si el curso está activo
        }


        #endregion

        #region METODO PARA CREAR
        // Metodo Para Crear
        public static async Task<int> CreateAsync(Attendance attendance)
        {
            if (attendance == null)
            {
                throw new ArgumentNullException(nameof(attendance), "La asistencia no puede ser nula.");
            }

            int result = 0;

            using (var dbContext = new ContextDB())
            {
                // Validar si ya existe la asignación
                if (await ExistAttendance(attendance, dbContext))
                {
                    throw new Exception("Asistencia ya existente, vuelve a intentarlo nuevamente.");
                }

                // Validar si el estudiante está activo
                if (!await IsStudentActive(attendance.IdStudent, dbContext))
                {
                    throw new Exception("No se puede agregar la asistencia ya que el Alumno/a no esta activo.");
                }

                // Validar si el curso está activo
                if (!await IsCourseActive(attendance.IdCourse, dbContext))
                {
                    throw new Exception("No se puede agregar la asistencia ya que el Curso no esta activo.");
                }

                // Guardar la asignación en la base de datos
                dbContext.Attendance.Add(attendance);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }
        #endregion

        #region METODO PARA MOSTRAR
        // Metodo Para Mostrar La Lista De Registros
        public static async Task<List<Attendance>> GetAllAsync()
        {
            var attendance = new List<Attendance>();
            using (var dbContext = new ContextDB())
            {
                attendance = await dbContext.Attendance.ToListAsync();
            }
            return attendance;
        }
        #endregion

        #region METODO PARA OBTENER POR ID
        // Metodo Para Mostrar Un Registro En Base A Su Id
        public static async Task<Attendance> GetByIdAsync(Attendance attendance)
        {
            var attendanceDB = new Attendance();
            using (var dbContext = new ContextDB())
            {
                attendanceDB = await dbContext.Attendance.FirstOrDefaultAsync(a  => a.Id == attendance.Id);
            }
            return attendanceDB!;
        }
        #endregion

        #region METODO PARA BUSCAR REGISTROS MEDIANTE EL USO DE FILTROS
        // Metodo Para Buscar Por Filtros
        // IQueryable es una interfaz que toma un coleccion a la cual se le pueden implementar multiples consultas (Filtros)
        internal static IQueryable<Attendance> QuerySelect(IQueryable<Attendance> query, Attendance attendance)
        {
            // Por ID
            if (attendance.Id > 0)
                query = query.Where(c => c.Id == attendance.Id);

            // Por Estudiante
            if (attendance.IdStudent > 0)
                query = query.Where(c => c.IdStudent == attendance.IdStudent);

            // Por Curso
            if (attendance.IdCourse > 0)
                query = query.Where(c => c.IdCourse == attendance.IdCourse);

            // Ordenamos de Manera Desendente
            query = query.OrderByDescending(c => c.Id).AsQueryable();

            return query;
        }
        #endregion

        #region METODO PARA BUSCAR
        // Metodo para Buscar Registros Existentes
        public static async Task<List<Attendance>> SearchAsync(Attendance attendance)
        {
            var attendances = new List<Attendance>();
            using (var dbContext = new ContextDB())
            {
                var select = dbContext.Attendance.AsQueryable();
                select = QuerySelect(select, attendance);
                attendances = await select.ToListAsync();
            }
            return attendances;
        }
        #endregion

        #region METODO PARA INCLUIR ESTUDIANTE Y CURSO
        // Método que incluye el estudiante y el curso para la búsqueda
        public static async Task<List<Attendance>> SearchIncludeAsync(Attendance attendance)
        {
            var attendances = new List<Attendance>();
            using (var dbContext = new ContextDB())
            {
                var select = dbContext.Attendance.AsQueryable();
                select = QuerySelect(select, attendance).Include(a => a.Student).Include(a => a.Course).AsQueryable();
                attendances = await select.ToListAsync();
            }
            return attendances;
        }
        #endregion

        #region METODO PARA MODIFICAR
        // Metodo para modificar un registro
        public static async Task<int> UpdateAsync(Attendance attendance)
        {
            if (attendance == null)
            {
                throw new ArgumentNullException(nameof(attendance), "La asignación no puede ser nula.");
            }

            int result = 0;

            using (var dbContext = new ContextDB())
            {
                var attendanceDB = await dbContext.Attendance.FirstOrDefaultAsync(a => a.Id == attendance.Id);
                if (attendanceDB == null)
                {
                    throw new Exception("Asistencia no encontrada para actualizar.");
                }

                // Validar si ya existe la asignación
                if (await ExistAttendance(attendance, dbContext))
                {
                    throw new Exception("Asistencia ya existente, vuelve a intentarlo nuevamente.");
                }

                // Validar si el estudiante está activo
                if (!await IsStudentActive(attendance.IdStudent, dbContext))
                {
                    throw new Exception("No se puede modificar la asistencia ya que el Alumno/a no está activo.");
                }

                // Validar si el curso está activo
                if (!await IsCourseActive(attendance.IdCourse, dbContext))
                {
                    throw new Exception("No se puede modificar la asistencia ya que el Curso no está activo.");
                }

                // Actualizar la asistencia en la base de datos
                attendanceDB.IdStudent = attendance.IdStudent;
                attendanceDB.IdCourse = attendance.IdCourse;
                attendanceDB.AttendedCount = attendance.AttendedCount;
                attendanceDB.AbsentCount = attendance.AbsentCount;
                attendanceDB.LeaveCount = attendance.LeaveCount;
                attendanceDB.DateModification = attendance.DateModification;

                dbContext.Update(attendanceDB);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }
        #endregion

        #region METODO PARA ELIMINAR
        // Metodo Para Eliminar Un Registro Existente En La Base De Datos
        public static async Task<int> DeleteAsync(Attendance attendance)
        {
            int result = 0;

            using (var dbContext = new ContextDB())
            {
                var attendanceDB = await dbContext.Attendance.FirstOrDefaultAsync(a => a.Id == attendance.Id);
                if (attendanceDB != null)
                {
                    dbContext.Attendance.Remove(attendanceDB);
                    result = await dbContext.SaveChangesAsync();
                }
            }
            return result;
        }
        #endregion

        #region METODO PARA OBTENER POR ID DE ESTUDIANTE Y ID DE CURSO
        // Metodo Para Obtener Un Registro En Base A Su IdStudiante Y IdCurso
        public static async Task<Attendance> GetByIdStudentAndCourseAsync(int idStudent, int idCourse)
        {
            using (var dbContext = new ContextDB())
            {
                var attendance = await dbContext.Attendance.Include(a => a.Student)
    .Include(a => a.Course).FirstOrDefaultAsync(a => a.IdStudent == idStudent && a.IdCourse == idCourse);
                if (attendance == null)
                    throw new Exception("No se encontró registro para el estudiante seleccionado");
                return attendance;
            }
        }
        #endregion
    }
}
