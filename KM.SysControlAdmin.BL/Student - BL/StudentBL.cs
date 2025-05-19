#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.DAL.Student___DAL;
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
    }
}
