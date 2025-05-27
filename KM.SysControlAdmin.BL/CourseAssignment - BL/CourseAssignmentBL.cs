#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.DAL.CourseAssignment___DAL;
using KM.SysControlAdmin.EN.CourseAssignment___EN;


#endregion

namespace KM.SysControlAdmin.BL.CourseAssignment___BL
{
    public class CourseAssignmentBL
    {
        #region METODO PARA CREAR
        // Metodo Para Guardar Un Nuevo Registro
        public async Task<int> CreateAsync(CourseAssignment courseAssignment)
        {
            return await CourseAssignmentDAL.CreateAsync(courseAssignment);
        }
        #endregion
    }
}
