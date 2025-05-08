#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.EN.Schedule___EN;
using Microsoft.EntityFrameworkCore;

#endregion

namespace KM.SysControlAdmin.DAL.Schedule___DAL
{
    public class ScheduleDAL
    {
        #region METODO PARA VALIDAR UNICA EXISTENCIA DEL REGISTRO
        // Metodo Para Validar La Unica Existencia De Un Registro y No Haber Duplicidad
        private static async Task<bool> ExistSchedule(Schedule schedule, ContextDB dbContext)
        {
            bool result = false;
            var schedules = await dbContext.Schedule.FirstOrDefaultAsync(r => r.StartTime == schedule.StartTime && r.EndTime == schedule.EndTime && r.Id != schedule.Id);
            if (schedules != null && schedules.Id > 0 && schedules.StartTime == schedule.StartTime && schedules.EndTime == schedule.EndTime)
                result = true;
            return result;
        }
        #endregion

        #region METODO PARA GUARDAR
        // Metodo Para Guardar Un Nuevo Registro En La Base De Datos
        public static async Task<int> CreateAsync(Schedule schedule)
        {
            int result = 0;
            using (var dbContext = new ContextDB())
            {
                bool scheduleExists = await ExistSchedule(schedule, dbContext);
                if (scheduleExists == false)
                {
                    dbContext.Schedule.Add(schedule);
                    result = await dbContext.SaveChangesAsync();
                }
                else
                    throw new Exception("Horario Ya Existente, Vuelve a Intentarlo");
            }
            return result;
        }
        #endregion
    }
}
