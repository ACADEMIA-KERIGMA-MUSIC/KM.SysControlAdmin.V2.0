#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.EN.CourseAssignment___EN;
using Microsoft.EntityFrameworkCore;


#endregion

namespace KM.SysControlAdmin.DAL.CourseAssignment___DAL
{
    public class CourseAssignmentDAL
    {
        #region METODO PARA VALIDAR UNICA EXISTENCIA DEL REGISTRO Y OTRAS METODOS PARA VALIDACIONES EXTRAS
        // Metodo Para Validar La Unica Existencia De Un Registro y No Haber Duplicidad
        private static async Task<bool> ExistCourseAssignment(CourseAssignment courseAssignment, ContextDB contextDB)
        {
            var existingAssignment = await contextDB.CourseAssignment.FirstOrDefaultAsync(c =>
                c.IdStudent == courseAssignment.IdStudent &&
                c.IdCourse == courseAssignment.IdCourse &&
                c.Id != courseAssignment.Id);

            return existingAssignment != null;
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

        // Metodo Para Validar Si El Curso Llego
        private static async Task<bool> IsCourseFull(int courseId, ContextDB contextDB)
        {
            var course = await contextDB.Course.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null)
            {
                throw new Exception("El curso no existe.");
            }

            int currentAssignments = await contextDB.CourseAssignment.CountAsync(ca => ca.IdCourse == courseId);
            return currentAssignments >= course.MaxStudent; // Retorna true si el curso ya está lleno
        }
        #endregion

        #region METODO PARA CREAR
        public static async Task<int> CreateAsync(CourseAssignment courseAssignment)
        {
            if (courseAssignment == null)
            {
                throw new ArgumentNullException(nameof(courseAssignment), "La asignación no puede ser nula.");
            }

            int result = 0;

            using (var dbContext = new ContextDB())
            {
                // Validar si ya existe la asignación
                if (await ExistCourseAssignment(courseAssignment, dbContext))
                {
                    throw new Exception("Asignacion ya existente, vuelve a intentarlo nuevamente.");
                }

                // Validar si el estudiante está activo
                if (!await IsStudentActive(courseAssignment.IdStudent, dbContext))
                {
                    throw new Exception("No se puede agregar la asignacion ya que el Alumno/a no esta activo.");
                }

                // Validar si el curso está activo
                if (!await IsCourseActive(courseAssignment.IdCourse, dbContext))
                {
                    throw new Exception("No se puede agregar la asignacion ya que el Curso no esta activo.");
                }

                // Validar si el curso no ha alcanzado su límite
                if (await IsCourseFull(courseAssignment.IdCourse, dbContext))
                {
                    throw new Exception("No se puede agregar la asignacion porque el curso ha alcanzado su limite maximo de estudiantes.");
                }

                // Guardar la asignación en la base de datos
                dbContext.CourseAssignment.Add(courseAssignment);
                result = await dbContext.SaveChangesAsync();
            }

            return result;
        }
        #endregion
    }
}
