#region REFERENCIAS
using KM.SysControlAdmin.EN.User___EN;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento


#endregion

namespace KM.SysControlAdmin.DAL.User___DAL
{
    public class UserDAL
    {
        #region METODO PARA ENCRIPTAR
        // Metodo Para Encriptar Via MD5 El Password
        private static void EncryptMD5(User user)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(user.Password));
                var encryptedStr = "";
                for (int i = 0; i < result.Length; i++)
                {
                    encryptedStr += result[i].ToString("x2").ToLower();
                }
                user.Password = encryptedStr;
            }
        }
        #endregion

        #region METODO PARA VALIDAR EXISTENCIA DEL USUARIO Y OTRAS VALIDACIONES
        // Metodo Para Validar La Existencia o No De Un Usuario
        private static async Task<bool> ExistsUser(User user, ContextDB dbContext)
        {
            bool result = false;
            var userLoginExists = await dbContext.User.FirstOrDefaultAsync(u => u.Email == user.Email && u.Id != user.Id);
            if (userLoginExists != null && userLoginExists.Id > 0 && userLoginExists.Email == user.Email)
                result = true;

            return result;
        }

        // Metodo para validar el estado del rol antes de agregar o modificar
        private static async Task<bool> StatusRole(int roleId, ContextDB contextDB)
        {
            var role = await contextDB.Role.FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
            {
                throw new Exception("El Rol especificado no existe.");
            }

            return role.Status == 1; // Retorna true si el rol esta activo (Status == 1)
        }
        #endregion

        #region METODO PARA GUARDAR
        // Metodo Para Guardar Un Nuevo Registro En La Base De Datos
        public static async Task<int> CreateAsync(User user)
        {
            int result = 0;
            using (var dbContext = new ContextDB())
            {
                // Verificar si el curso ya existe
                bool userExists = await ExistsUser(user, dbContext);
                if (userExists)
                {
                    throw new Exception("Usuario ya existente, vuelve a intentarlo nuevamente.");
                }

                // Validar que el horario esté activo
                if (!await StatusRole(user.IdRole, dbContext))
                {
                    throw new Exception("Rol No Disponible o Inactivo, Intenta Con Otro Rol.");
                }

                // Guardar el usuario en la base de datos
                EncryptMD5(user);
                dbContext.User.Add(user);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }
        #endregion
    }
}
