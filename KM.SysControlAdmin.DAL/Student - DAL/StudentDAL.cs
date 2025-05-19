#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.EN.Student___EN;
using Microsoft.EntityFrameworkCore;


#endregion

namespace KM.SysControlAdmin.DAL.Student___DAL
{
    public class StudentDAL
    {
        #region METODO PARA VALIDAR UNICA EXISTENCIA DEL REGISTRO
        // Metodo Para Validar La Unica Existencia De Un Registro y No Haber Duplicidad
        private static async Task<bool> ExistStudent(Student student, ContextDB contextDB)
        {
            bool result = false;
            var students = await contextDB.Student.FirstOrDefaultAsync(m => m.StudentCode == student.StudentCode && m.Name == student.Name &&
                                                                       m.LastName == student.LastName && m.DateOfBirth == student.DateOfBirth &&
                                                                       m.Id != student.Id);
            if (students != null && students.Id > 0 && students.StudentCode == student.StudentCode &&
                                                       students.Name == student.Name &&
                                                       students.LastName == student.LastName &&
                                                       students.DateOfBirth == student.DateOfBirth)
                result = true;
            return result;
        }
        #endregion

        #region METODO PARA CREAR
        // Metodo Para Guardar Un Nuevo Registro En La Base De Datos
        public static async Task<int> CreateAsync(Student student)
        {
            int result = 0;
            // Un bloque de conexion que mientras se permanezca en el bloque la base de datos permanecera abierta y al terminar se destruira
            using (var dbContext = new ContextDB())
            {
                bool studentExists = await ExistStudent(student, dbContext);
                if (studentExists == false)
                {
                    dbContext.Student.Add(student);
                    result = await dbContext.SaveChangesAsync(); // Await sirve para esperar a terminar todos los procesos para devolverlos todos juntos
                }
                else
                    throw new Exception("Alumno/a Ya Existente, Vuelve a Intentarlo Nuevamente.");
            }
            return result;  // Si se realizo con exito devuelve 1 sino devuelve 0
        }
        #endregion
    }
}
