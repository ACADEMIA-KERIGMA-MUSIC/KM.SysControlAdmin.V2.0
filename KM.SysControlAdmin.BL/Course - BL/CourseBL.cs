#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.DAL.Course___DAL;
using KM.SysControlAdmin.EN.Course___EN;

#endregion

namespace KM.SysControlAdmin.BL.Course___BL
{
    public class CourseBL
    {
        #region METODO PARA CREAR
        // Metodo Para Guardar Un Nuevo Registro
        public async Task<int> CreateAsync(Course course)
        {
            return await CourseDAL.CreateAsync(course);
        }
        #endregion
    }
}
