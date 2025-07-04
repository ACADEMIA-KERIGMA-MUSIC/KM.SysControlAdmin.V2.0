﻿#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.DAL.User___DAL;
using KM.SysControlAdmin.EN.User___EN;


#endregion

namespace KM.SysControlAdmin.BL.User___BL
{
    public class UserBL
    {
        #region METODO PARA CREAR
        // Metodo Para Guardar Un Nuevo Registro a La Base De Datos
        public async Task<int> CreateAsync(User user)
        {
            return await UserDAL.CreateAsync(user);
        }
        #endregion

        #region METODO PARA OBTENER TODOS
        // Metodo Para Listar y Mostrar Todos Los Resultados
        public async Task<List<User>> GetAllAsync()
        {
            return await UserDAL.GetAllAsync();
        }
        #endregion

        #region METODO PARA OBTENER POR ID
        // Metodo Para Obtener Un Registro En Base a Su Id
        public async Task<User> GetByIdAsync(User user)
        {
            return await UserDAL.GetByIdAsync(user);
        }
        #endregion

        #region METODO PARA BUSCAR
        // Metodo Para Buscar Registros En La Base De Datos
        public async Task<List<User>> SearchAsync(User user)
        {
            return await UserDAL.SearchAsync(user);
        }
        #endregion

        #region METODO PARA BUSCAR INCLUYENGO LA LLAVE FORANEA
        // Metodo Para Buscar Registros Incluyendo Las Llaves Foraneas
        public async Task<List<User>> SearchIncludeRoleAsync(User user)
        {
            return await UserDAL.SearchIncludeRoleAsync(user);
        }
        #endregion

        #region METODO PARA MODIFICAR
        // Metodo Para Modificar Un Registro Existente En La Base De Datos
        public async Task<int> UpdateAsync(User user)
        {
            return await UserDAL.UpdateAsync(user);
        }
        #endregion

        #region METODO PARA ELIMINAR
        // Metodo Para Eliminar Un Registro Existente En La Base De Datos
        public async Task<int> DeleteAsync(User user)
        {
            return await UserDAL.DeleteAsync(user);
        }
        #endregion

        #region METODO PARA INICIAR SESION (LOGIARSE)
        // Metodo Para Autenticar El Usuario
        public async Task<User> LoginAsync(User user)
        {
            return await UserDAL.LoginAsync(user);
        }
        #endregion

        #region METODO PARA CAMBIAR CONTRASEÑA
        // Metodo Para Cambiar La Contraseña Del Usuario
        public async Task<int> ChangePasswordAsync(User user, string oldPassword)
        {
            return await UserDAL.ChangePasswordAsync(user, oldPassword);
        }
        #endregion

        #region METODO PARA CAMBIAR LA CONTRASEÑA DESDE DESARROLLADOR Y ADMINISTRADOR
        // Metodo Para Cambiar La Contraseña Del Usuario
        public async Task<int> ChangePasswordRoleDesAsync(User user)
        {
            return await UserDAL.ChangePasswordRoleDesAsync(user);
        }
        #endregion

        #region METODO PARA ACTUALIZAR UNICAMENTE FOTOGRAFIA DEL USUARIO
        // Metodo Para Modificar Unicamente La Fotografia Del Usuario Logueado
        public async Task<int> UpdatePhotoAsync(User user)
        {
            return await UserDAL.UpdatePhotoAsync(user);
        }
        #endregion

        #region METODO BL PARA VALIDAR SI EXISTE UN CORREO DE USUARIO
        // Metodo BL Para Verificar Si Un Correo Existe
        public static async Task<bool> EmailExistsAsync(string email)
        {
            return await UserDAL.EmailExistsAsync(email);
        }
        #endregion

        #region METODO BL PARA ENVIAR CONTRASEÑA TEMPORAL
        // Metodo BL Para Enviar Contraseña Temporal
        public static async Task<bool> SendTemporaryPasswordAsync(string email)
        {
            return await UserDAL.SetTemporaryPasswordAsync(email);
        }
        #endregion

        #region METODO PARA MODIFICAR INFORMACION ESPECIFICA DEL USUARIO LOGIADO
        // Metodo Para Modificar Informacion Especifica Del Usuario Logiado
        public async Task<int> UpdateInfoAsync(User user)
        {
            return await UserDAL.UpdateInfoAsync(user);
        }
        #endregion
    }
}
