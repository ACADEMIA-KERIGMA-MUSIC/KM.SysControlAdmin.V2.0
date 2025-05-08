#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.DAL.Schedule___DAL;
using KM.SysControlAdmin.EN.Schedule___EN;


#endregion

namespace KM.SysControlAdmin.BL.Schedule___BL
{
    public class ScheduleBL
    {
        #region METODO PARA GUARDAR
        // Metodo Para Guardar Un Nuevo Registro a La Base De Datos
        public async Task<int> CreateAsync(Schedule schedule)
        {
            return await ScheduleDAL.CreateAsync(schedule);
        }
        #endregion
    }
}
