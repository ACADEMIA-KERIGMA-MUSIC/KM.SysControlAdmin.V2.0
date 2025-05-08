#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.DAL.Trainer___DAL;
using KM.SysControlAdmin.EN.Trainer___EN;

#endregion

namespace KM.SysControlAdmin.BL.Trainer___BL
{
    public class TrainerBL
    {
        #region METODO PARA CREAR
        // Metodo Para Guardar Un Nuevo Registro
        public async Task<int> CreateAsync(Trainer trainer)
        {
            return await TrainerDAL.CreateAsync(trainer);
        }
        #endregion
    }
}
