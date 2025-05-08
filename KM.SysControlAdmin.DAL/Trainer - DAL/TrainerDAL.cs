#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.EN.Trainer___EN;
using Microsoft.EntityFrameworkCore;

#endregion

namespace KM.SysControlAdmin.DAL.Trainer___DAL
{
    public class TrainerDAL
    {
        #region METODO PARA VALIDAR UNICA EXISTENCIA DEL REGISTRO
        // Metodo Para Validar La Unica Existencia De Un Registro y No Haber Duplicidad
        private static async Task<bool> ExistTrainer(Trainer trainer, ContextDB contextDB)
        {
            bool result = false;
            var trainers = await contextDB.Trainer.FirstOrDefaultAsync(m => m.Dui == trainer.Dui && m.Code == trainer.Code && m.Id != trainer.Id);
            if (trainers != null && trainers.Id > 0 && trainers.Dui == trainer.Dui && trainers.Code == trainer.Code)
                result = true;
            return result;
        }
        #endregion

        #region METODO PARA CREAR
        // Metodo Para Guardar Un Nuevo Registro En La Base De Datos
        public static async Task<int> CreateAsync(Trainer trainer)
        {
            int result = 0;
            using (var dbContext = new ContextDB())
            {
                bool trainerExists = await ExistTrainer(trainer, dbContext);
                if (trainerExists == false)
                {
                    dbContext.Trainer.Add(trainer);
                    result = await dbContext.SaveChangesAsync();
                }
                else
                    throw new Exception("Instructor/Docente Ya Existente, Vuelve a Intentarlo Nuevamente.");
            }
            return result;  // Si se realizo con exito devuelve 1 sino devuelve 0
        }
        #endregion
    }
}
