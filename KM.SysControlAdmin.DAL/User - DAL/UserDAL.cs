﻿#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.EN.User___EN;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using KM.SysControlAdmin.Core.Utils;

#endregion

namespace KM.SysControlAdmin.DAL.User___DAL
{
    public class UserDAL
    {
        #region METODO PARA ENCRIPTAR
        // Metodo Para Encriptar Via MD5 El Password
        private static string EncryptMD5(string plainText)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(plainText));
                var encryptedStr = "";
                for (int i = 0; i < result.Length; i++)
                {
                    encryptedStr += result[i].ToString("x2").ToLower();
                }
                return encryptedStr;
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
                user.Password = EncryptMD5(user.Password);
                dbContext.User.Add(user);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }
        #endregion

        #region METODO PARA MOSTRAR TODOS
        // Metodo Para Listar y Mostrar Todos Los Resultados
        public static async Task<List<User>> GetAllAsync()
        {
            var users = new List<User>();
            using (var dbContext = new ContextDB())
            {
                users = await dbContext.User.ToListAsync();
            }
            return users;
        }
        #endregion

        #region METODO PARA OBTENER POR ID
        // Metodo Para Obtener Un Registro Por Su Id
        public static async Task<User> GetByIdAsync(User user)
        {
            var userDb = new User();
            using (var dbContext = new ContextDB())
            {
                userDb = await dbContext.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == user.Id);
            }
            return userDb!;
        }

        // EL METODO DE COMENTADO ES LA FORMA BASICA Y FACIL DEL METODO, EL DE ARRIBA ESTA MEJORADO SEGUN LAS NECESIDADES DE ALGUN PROCESO CON ESE METODO
        //public static async Task<User> GetByIdAsync(User user)
        //{
        //    var userDb = new User();
        //    using (var dbContext = new ContextDB())
        //    {
        //        userDb = await dbContext.User.FirstOrDefaultAsync(u => u.Id == user.Id);
        //    }
        //    return userDb!;
        //}
        #endregion

        #region METODO PARA FILTRAR EN BASE A PARAMETROS
        // Metodo Para Filtrar Resultados De La Busqueda En Base a Parametros 
        internal static IQueryable<User> QuerySelect(IQueryable<User> query, User user)
        {
            if (user.Id > 0)
                query = query.Where(u => u.Id == user.Id);

            if (user.IdRole > 0)
                query = query.Where(u => u.IdRole == user.IdRole);

            if (!string.IsNullOrWhiteSpace(user.Name))
                query = query.Where(u => u.Name.Contains(user.Name));

            if (!string.IsNullOrWhiteSpace(user.LastName))
                query = query.Where(u => u.LastName.Contains(user.LastName));

            if (!string.IsNullOrWhiteSpace(user.Email))
                query = query.Where(u => u.Email.Contains(user.Email));

            if (user.Status > 0)
                query = query.Where(u => u.Status == user.Status);

            if (user.DateCreated.Year > 1000)
            {
                DateTime inicialDate = new DateTime(user.DateCreated.Year, user.DateCreated.Month, user.DateCreated.Day, 0, 0, 0);
                DateTime finalDate = inicialDate.AddDays(1).AddMilliseconds(-1);
                query = query.Where(u => u.DateCreated >= inicialDate && u.DateCreated <= finalDate);
            }

            query = query.OrderByDescending(u => u.Id).AsQueryable();

            return query;
        }
        #endregion

        #region METODO PARA BUSCAR
        // Metodo Para Buscar Registros Existentes En La Base De Datos
        public static async Task<List<User>> SearchAsync(User user)
        {
            var users = new List<User>();
            using (var dbContext = new ContextDB())
            {
                var select = dbContext.User.AsQueryable();
                select = QuerySelect(select, user);
                users = await select.ToListAsync();
            }
            return users;
        }
        #endregion

        #region METODO PARA INCLUIR LLAVES FORANEAS A LA BUSQUEDA
        // Metodo Para Incluir El Rol En La Busqueda
        public static async Task<List<User>> SearchIncludeRoleAsync(User user)
        {
            var users = new List<User>();
            using (var dbContext = new ContextDB())
            {
                var select = dbContext.User.AsQueryable();
                select = QuerySelect(select, user).Include(u => u.Role).AsQueryable();
                users = await select.ToListAsync();
            }
            return users;
        }
        #endregion

        #region METODO PARA MODIFICAR
        // Metodo Para Modificar Un Registro Existente De La Base De Datos
        public static async Task<int> UpdateAsync(User user)
        {
            int result = 0;
            using (var dbContext = new ContextDB())
            {
                // Verificar si el curso ya existe
                bool userExists = await ExistsUser(user, dbContext);
                if (userExists)
                {
                    throw new Exception("Usuario Ya Existente, Vuelve a Intentarlo Nuevamente.");
                }

                // Validar que el horario esté activo
                if (!await StatusRole(user.IdRole, dbContext))
                {
                    throw new Exception("Rol No Disponible o Inactivo, Intenta Con Otro Rol.");
                }

                var userDb = await dbContext.User.FirstOrDefaultAsync(u => u.Id == user.Id);
                if (userDb == null)
                {
                    throw new Exception("Usuario no encontrado.");
                }
                userDb.Name = user.Name;
                userDb.LastName = user.LastName;
                userDb.Email = user.Email;
                userDb.Status = user.Status;
                userDb.DateModification = user.DateModification;
                userDb.ImageData = user.ImageData;
                userDb.IdRole = user.IdRole;
                userDb.RecoveryEmail = user.RecoveryEmail;
                userDb.CodeUser = user.CodeUser;

                dbContext.User.Update(userDb);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }
        #endregion

        #region METODO PARA ELIMINAR
        // Metodo Para Eliminar Un Registro Existente En La Base De Datos
        public static async Task<int> DeleteAsync(User user)
        {
            int result = 0;
            using (var dbContext = new ContextDB())
            {
                var userDb = await dbContext.User.FirstOrDefaultAsync(u => u.Id == user.Id);
                dbContext.User.Remove(userDb!);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }
        #endregion

        #region METODO PARA INICIAR SESION (LOGIARSE)
        // Metodo Para Autenticar El Usuario
        public static async Task<User> LoginAsync(User user)
        {
            var userDb = new User();
            using (var dbContext = new ContextDB())
            {
                user.Password = EncryptMD5(user.Password);
                userDb = await dbContext.User.FirstOrDefaultAsync(
                    u => u.Email == user.Email && u.Password == user.Password
                    && u.Status == 1);
            }
            return userDb!;
        }
        #endregion

        #region METODO PARA CAMBIAR LA CONTRASEÑA
        // Metodo Para Cambiar o Actualizar La Contraseña Del Usuario
        public static async Task<int> ChangePasswordAsync(User user, string oldPassword)
        {
            int result = 0;

            // Encriptar la contraseña antigua recibida
            string oldPasswordEncrypted = EncryptMD5(oldPassword);

            using (var dbContext = new ContextDB())
            {
                var userDb = await dbContext.User.FirstOrDefaultAsync(u => u.Id == user.Id);
                if (userDb == null)
                    throw new Exception("Usuario no encontrado");

                // Comparar contraseñas encriptadas
                if (oldPasswordEncrypted == userDb.Password)
                {
                    // Encriptar la nueva contraseña del usuario
                    userDb.Password = EncryptMD5(user.Password);

                    dbContext.User.Update(userDb);
                    result = await dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Credencial Incorrecta");
                }
            }
            return result;
        }
        #endregion

        #region METODO PARA CAMBIAR LA CONTRASEÑA DESDE DESARROLLADOR Y ADMINISTRADOR
        // Metodo Para Cambiar o Actualizar La Contraseña Del Usuario Sin Validar La Anterior
        public static async Task<int> ChangePasswordRoleDesAsync(User user)
        {
            int result = 0;
            string encryptedPassword = EncryptMD5(user.Password); // Encriptamos la nueva contraseña
            using (var dbContext = new ContextDB())
            {
                var userDb = await dbContext.User.FirstOrDefaultAsync(u => u.Id == user.Id);
                if (userDb != null)
                {
                    userDb.Password = user.Password;
                    dbContext.User.Update(userDb);
                    result = await dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Usuario no encontrado");
                }
            }
            return result;
        }
        #endregion

        #region METODO PARA ACTUALIZAR UNICAMENTE FOTOGRAFIA DEL USUARIO
        // Metodo Para Modificar Unicamente La Fotografia Del Usuario Logueado
        public static async Task<int> UpdatePhotoAsync(User user)
        {
            int result = 0;
            using (var dbContext = new ContextDB())
            {
                var userDb = await dbContext.User.FirstOrDefaultAsync(u => u.Id == user.Id);
                if (userDb == null)
                {
                    throw new Exception("Usuario no encontrado.");
                }

                userDb.ImageData = user.ImageData;

                dbContext.User.Update(userDb);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }
        #endregion

        #region METODO PARA VALIDAR SI EXISTE UN CORREO DE USUARIO
        // Metodo Para Verificar Si Un Correo Existe En La Base De Datos
        public static async Task<bool> EmailExistsAsync(string email)
        {
            using (var dbContext = new ContextDB())
            {
                return await dbContext.User.AnyAsync(u => u.Email == email);
            }
        }
        #endregion

        #region METODO PARA ENVIAR CONTRASEÑA TEMPORAL
        // Metodo encargado de mandar la contraseña aleatoria al correo
        public static async Task<bool> SetTemporaryPasswordAsync(string email)
        {
            using (var dbContext = new ContextDB())
            {
                var user = await dbContext.User.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null || string.IsNullOrEmpty(user.RecoveryEmail))
                    return false;

                // Generar contraseña temporal
                string tempPassword = UtilsPasswordGenerator.GenerateTemporaryPassword();

                // Encriptar la contraseña temporal directamente
                string encryptedTempPassword = EncryptMD5(tempPassword);

                // Asignar la nueva contraseña encriptada al usuario
                user.Password = encryptedTempPassword;

                // Guardar cambios en DB
                await dbContext.SaveChangesAsync();

                // Enviar contraseña temporal al correo de recuperación (se envía la no encriptada para que el usuario la use)
                return await UtilsEmailService.SendTemporaryPassword(user.RecoveryEmail, tempPassword);
            }
        }
        #endregion

        #region METODO PARA MODIFICAR INFORMACION ESPECIFICA DEL USUARIO LOGIADO
        // Metodo Para Modificar Informacion Especifica Del Usuario Logiado
        public static async Task<int> UpdateInfoAsync(User user)
        {
            int result = 0;
            using (var dbContext = new ContextDB())
            {
                var userDb = await dbContext.User.FirstOrDefaultAsync(u =>u.Id == user.Id);
                if (userDb == null)
                {
                    throw new Exception("Usuario no encontrado");
                }

                userDb.Name = user.Name;
                userDb.LastName = user.LastName;
                userDb.RecoveryEmail = user.RecoveryEmail;

                dbContext.User.Update(userDb);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }
        #endregion
    }
}
