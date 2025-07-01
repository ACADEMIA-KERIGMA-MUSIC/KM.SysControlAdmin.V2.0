#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.EN.Attendance___EN;
using KM.SysControlAdmin.DAL.Attendance___DAL;

#endregion

namespace KM.SysControlAdmin.BL.Attendance___BL
{
    public class AttendanceBL
    {
        #region METODO PARA CREAR
        // Metodo Para Guardar Un Nuevo Registro
        public async Task<int> CreateAsync(Attendance attendance)
        {
            return await AttendanceDAL.CreateAsync(attendance);
        }
        #endregion

        #region METODO PARA MOSTRAR
        // Metodo Para Mostrar Una Lista De Registros
        public async Task<List<Attendance>> GetAllAsync()
        {
            return await AttendanceDAL.GetAllAsync();
        }
        #endregion

        #region METODO PARA MOSTRAR POR ID
        // Metodo Para Mostrar Un Registro Especifico Bajo Un Id
        public async Task<Attendance> GetByIdAsync(Attendance attendance)
        {
            return await AttendanceDAL.GetByIdAsync(attendance);
        }
        #endregion

        #region METODO PARA BUSCAR
        // Metodo Para Buscar Registros Existentes
        public async Task<List<Attendance>> SearchAsync(Attendance attendance)
        {
            return await AttendanceDAL.SearchAsync(attendance);
        }
        #endregion

        #region METODO PARA INCLUIR ESTUDIANTE Y CURSO
        // Metodo Para Incluir Estudiante y Curso En La Busqueda
        public async Task<List<Attendance>> SearchIncludeAsync(Attendance attendance)
        {
            return await AttendanceDAL.SearchIncludeAsync(attendance);
        }
        #endregion

        #region METODO PARA MODIFICAR
        // Metodo Para Modificar Un Registro En Base a Un Id
        public async Task<int> UpdateAsync(Attendance attendance)
        {
            return await AttendanceDAL.UpdateAsync(attendance);
        }
        #endregion

        #region METODO PARA ELIMINAR
        // Metodo Para Elimnar Un Registro Existente En La Base De Datos
        public async Task<int> DeleteAsync(Attendance attendance)
        {
            return await AttendanceDAL.DeleteAsync(attendance);
        }
        #endregion
    }
}
