﻿#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.DAL.Student___DAL;
using KM.SysControlAdmin.EN.Course___EN;
using KM.SysControlAdmin.EN.Student___EN;


#endregion

namespace KM.SysControlAdmin.BL.Student___BL
{
    public class StudentBL
    {
        #region METODO PARA CREAR
        // Metodo Para Guardar Un Nuevo Registro
        public async Task<int> CreateAsync(Student student)
        {
            return await StudentDAL.CreateAsync(student);
        }
        #endregion

        #region METODO PARA MOSTRAR
        // Metodo Para Mostrar Una Lista De Registros
        public async Task<List<Student>> GetAllAsync()
        {
            return await StudentDAL.GetAllAsync();
        }
        #endregion

        #region METODO PARA MOSTRAR POR ID
        // Metodo Para Mostrar Un Registro Especifico Bajo Un Id
        public async Task<Student> GetByIdAsync(Student student)
        {
            return await StudentDAL.GetByIdAsync(student);
        }

        // Metodo para que admita int al hacer uso del metodo antecesor para automatizacion
        public async Task<Student> GetByIdAsync(int id)
        {
            // Crear una instancia de Membership y asignarle el ID
            var student = new Student { Id = id };

            // Llamar al método existente con el objeto Membership
            return await StudentDAL.GetByIdAsync(student);
        }
        #endregion

        #region METODO PARA BUSCAR
        // Metodo Para Buscar Registros Existentes
        public async Task<List<Student>> SearchAsync(Student student)
        {
            return await StudentDAL.SearchAsync(student);
        }
        #endregion

        #region METODO PARA MODIFICAR ALUMNO BECADO
        // Metodo Para Guardar Un Nuevo Registro
        public async Task<int> UpdateAsyncBecado(Student student)
        {
            return await StudentDAL.UpdateAsyncBecado(student);
        }
        #endregion

        #region METODO PARA MODIFICAR ALUMNO EXTERNO
        // Metodo Para Guardar Un Nuevo Registro
        public async Task<int> UpdateAsyncExterno(Student student)
        {
            return await StudentDAL.UpdateAsyncExterno(student);
        }
        #endregion

        #region METODO PARA ELIMINAR
        // Metodo Para Eliminar Un Registro Existente En La Base De Datos
        public async Task<int> DeleteAsync(Student student)
        {
            return await StudentDAL.DeleteAsync(student);
        }
        #endregion

        #region METODO PARA OBTENER CURSOS ASIGNADOS SEGUN ESTUDIANTE
        // Metodo para obtener los cursos a los que está inscrito un estudiante
        public async Task<List<Course>> GetCoursesByStudentCodeAsync(string studentCode)
        {
            return await StudentDAL.GetCoursesByStudentCodeAsync(studentCode);
        }
        #endregion
    }
}
