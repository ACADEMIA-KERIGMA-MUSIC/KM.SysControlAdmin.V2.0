﻿#region REFERENCIAS
// Referencias Necesarias Para El Correcto Funcionamiento
using KM.SysControlAdmin.BL.Role___BL;
using KM.SysControlAdmin.BL.User___BL;
using KM.SysControlAdmin.Core.Utils;
using KM.SysControlAdmin.EN.Role___EN;
using KM.SysControlAdmin.EN.User___EN;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


#endregion

namespace KM.SysControlAdmin.WebApp.Controllers.User___Controller
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Desarrollador, Administrador, Instructor, Alumno/a, Secretario/a, Invitado")]
    public class UserController : Controller
    {
        // Creamos Las Instancias Para Acceder a Los Metodos
        UserBL userBL = new UserBL();
        RoleBL roleBL = new RoleBL();

        #region METODO PARA GUARDAR
        // Accion Que Muestra El Formulario
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public async Task<IActionResult> CreateUser()
        {
            var roles = await roleBL.GetAllAsync();
            ViewBag.Roles = roles;
            return View();
        }

        // Accion Que Recibe Los Datos y Los Envia a La Base De Datos
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(User user, IFormFile imagen)
        {
            try
            {
                const int maxFileSize = 1572864; // 1.5 MB
                if (imagen.Length > maxFileSize)
                {
                    throw new Exception("La imagen no debe pesar mas de 1.5MB.");
                }

                // Mapeo de img a ArrayByte
                if (imagen != null && imagen.Length > 0)
                {
                    byte[] imagenData = null!;
                    using (var memoryStream = new MemoryStream())
                    {
                        await imagen.CopyToAsync(memoryStream);
                        imagenData = memoryStream.ToArray();
                    }

                    user.ImageData = imagenData; // Asigna el array de bytes al campo de la imagen en tu modelo Membership
                }
                user.DateCreated = DateTime.Now.GetFechaZonaHoraria();
                user.DateModification = DateTime.Now.GetFechaZonaHoraria();
                int result = await userBL.CreateAsync(user);
                TempData["SuccessMessageCreate"] = "Usuario Agregado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                ViewBag.Roles = await roleBL.GetAllAsync();
                return View(user);
            }
        }
        #endregion

        #region METODO PARA INDEX
        // Metodo Para Mostrar La Vista Index
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public async Task<IActionResult> Index(User user = null!)
        {
            if (user == null)
                user = new User();

            var users = await userBL.SearchIncludeRoleAsync(user);
            var roles = await roleBL.GetAllAsync();

            ViewBag.Roles = roles;

            return View(users);
        }
        #endregion

        #region METODO PARA MODIFICAR
        // Acción que muestra el formulario
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await userBL.GetByIdAsync(new User { Id = id });
            user.Role = await roleBL.GetByIdAsync(new Role { Id = user.Id });

            // Convertir el array de bytes en imagen para mostrar en la vista (si la imagen existe)
            if (user.ImageData != null && user.ImageData.Length > 0)
            {
                ViewBag.ImageUrl = Convert.ToBase64String(user.ImageData);
            }
            ViewBag.Roles = await roleBL.GetAllAsync();
            return View(user);
        }

        // Acción que recibe los datos del formulario y los envía a la base de datos
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, User user, IFormFile imagen)
        {
            try
            {
                // Verificar que el id coincida con el usuario que se está modificando
                if (id != user.Id)
                {
                    return BadRequest();
                }

                // Si se ha subido una nueva imagen, actualizar el campo de imagen
                if (imagen != null && imagen.Length > 0)
                {
                    const int maxFileSize = 1572864; // 1.5 MB
                    if (imagen.Length > maxFileSize)
                    {
                        throw new Exception("La imagen no debe pesar mas de los 1.5MB.");

                    }

                    byte[] imagenData = null!;
                    using (var memoryStream = new MemoryStream())
                    {
                        await imagen.CopyToAsync(memoryStream);
                        imagenData = memoryStream.ToArray();
                    }
                    user.ImageData = imagenData; // Asignar el array de bytes de la nueva imagen al objeto User
                }
                else
                {
                    // Si no se proporciona una nueva imagen, mantener la imagen existente
                    User existingUser = await userBL.GetByIdAsync(new User { Id = id });
                    user.ImageData = existingUser.ImageData;
                }

                // Actualizar la fecha de modificación
                user.DateModification = DateTime.Now.GetFechaZonaHoraria();
                int result = await userBL.UpdateAsync(user);
                TempData["SuccessMessageUpdate"] = "Usuario Modificado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                ViewBag.Roles = await roleBL.GetAllAsync();
                return View(user);
            }
        }
        #endregion

        #region METODO PARA DETALLES
        // Acción que muestra los detalles de un registro
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public async Task<IActionResult> DetailsUser(int id)
        {
            try
            {
                // Obtener el usuario por ID
                var user = await userBL.GetByIdAsync(new User { Id = id });
                if (user == null)
                {
                    return NotFound();
                }
                // Verificar si el usuario tiene una imagen y convertirla para mostrar en la vista
                if (user.ImageData != null && user.ImageData.Length > 0)
                {
                    ViewBag.ImageUrl = Convert.ToBase64String(user.ImageData);
                }
                user.Role = await roleBL.GetByIdAsync(new Role { Id = user.IdRole });

                return View(user); // Retornar los detalles del usuario a la vista
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(); // Devolver la vista sin ningún objeto en caso de error
            }
        }
        #endregion

        #region METODO PARA ELIMINAR
        // Acción que muestra el formulario de eliminación
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                // Obtener el usuario por ID
                var user = await userBL.GetByIdAsync(new User { Id = id });
                if (user == null)
                {
                    return NotFound();
                }
                if (user.ImageData != null && user.ImageData.Length > 0)
                {
                    ViewBag.ImageUrl = Convert.ToBase64String(user.ImageData);
                }
                user.Role = await roleBL.GetByIdAsync(new Role { Id = user.IdRole });
                ViewBag.Roles = await roleBL.GetAllAsync();

                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(); // Devolver la vista sin ningún objeto en caso de error
            }
        }

        // Acción que recibe los datos del formulario para ser eliminados en la base de datos
        [Authorize(Roles = "Desarrollador, Administrador, Secretario/a")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id, User user)
        {
            try
            {
                // Obtener el usuario por ID para asegurarse de que existe antes de eliminar
                var userDb = await userBL.GetByIdAsync(new User { Id = id });
                if (userDb == null)
                {
                    return NotFound();
                }
                int result = await userBL.DeleteAsync(userDb);
                TempData["SuccessMessageDelete"] = "Usuario Eliminado Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                // Volver a cargar el usuario en caso de error
                var userDb = await userBL.GetByIdAsync(new User { Id = id });
                if (userDb == null)
                {
                    userDb = new User();
                }
                // Obtener el rol del usuario en caso de que exista
                if (userDb.Id > 0)
                {
                    userDb.Role = await roleBL.GetByIdAsync(new Role { Id = userDb.IdRole });
                }
                ViewBag.Roles = await roleBL.GetAllAsync();

                return View(userDb); // Devolver la vista con los datos del usuario para que pueda corregir o revisar
            }
        }
        #endregion

        #region METODO DE INICIO DE SESION Y CERRAR SESION (LOGIN, LOGOUT)
        // Accion Que Muestra El Formulario
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null!)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.Url = returnUrl;
            ViewBag.Error = "";
            return View();
        }

        // Accion Que Ejecuta La Autenticacion Del Usuario
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user, string returnUrl = null!)
        {
            try
            {
                var userDb = await userBL.LoginAsync(user);
                if (userDb != null && userDb.Id > 0 && userDb.Email == user.Email)
                {
                    userDb.Role = await roleBL.GetByIdAsync(new Role { Id = userDb.IdRole });
                    var claims = new[] { new Claim(ClaimTypes.Name, userDb.Email), new Claim(ClaimTypes.Role, userDb.Role.Name) };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                }
                else
                    throw new Exception("Credenciales Incorrectas, Vuelve a Intentarlo");

                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                ViewBag.Url = returnUrl;
                ViewBag.Error = e.Message;
                return View(new User { Email = user.Email });
            }
        }

        // Accion Que Permite Cerrar La Sesion
        [AllowAnonymous]
        public async Task<IActionResult> Logout(string returnUrl = null!)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }
        #endregion

        #region METODO PARA CAMBIAR LA CONTRASEÑA
        // Accion Que Muestra El Formulario
        [Authorize(Roles = "Desarrollador, Administrador, Instructor, Alumno/a, Secretario/a, Invitado")]
        public async Task<IActionResult> ChangePassword()
        {
            var users = await userBL.SearchAsync(new User { Email = User.Identity!.Name! });
            var actualUser = users.FirstOrDefault();
            return View(actualUser);
        }

        // Accion Que Recibe La Contraseña Actualizada y La Envia a La Base De Datos
        [Authorize(Roles = "Desarrollador, Administrador, Instructor, Alumno/a, Secretario/a, Invitado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(User user, string oldPassword)
        {
            try
            {
                int result = await userBL.ChangePasswordAsync(user, oldPassword);
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "User");
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                var users = await userBL.SearchAsync(new User { Email = User.Identity!.Name! });
                var actualUser = users.FirstOrDefault();
                return View(actualUser);
            }
        }
        #endregion

        #region METODO PARA DETALLES DEL PERFIL LOGIADO
        // Accion Que Muestra El Formulario
        [Authorize(Roles = "Desarrollador, Administrador, Instructor, Alumno/a, Secretario/a, Invitado")]
        public async Task<IActionResult> Information()
        {
            try
            {
                var users = await userBL.SearchIncludeRoleAsync(new User { Email = User.Identity!.Name! });
                var actualUser = users.FirstOrDefault();
                // Convertir el array de bytes en imagen para mostrar en la vista
                if (actualUser!.ImageData != null && actualUser.ImageData.Length > 0)
                {
                    ViewBag.ImageUrl = Convert.ToBase64String(actualUser.ImageData);
                }
                // Si hay mensaje en TempData, pásalo a ViewBag
                if (TempData["SuccessMessageUpdate"] != null)
                {
                    ViewBag.AlertMessage = TempData["SuccessMessageUpdate"];
                }
                return View(actualUser);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
        #endregion

        #region METODO PARA ACTUALIZAR UNICAMENTE FOTOGRAFIA DEL USUARIO
        // Muestra el perfil del usuario logueado con su imagen (si tiene)
        [Authorize(Roles = "Desarrollador, Administrador, Instructor, Alumno/a, Secretario/a, Invitado")]
        public async Task<IActionResult> UpdatePictureUser()
        {
            try
            {
                var users = await userBL.SearchIncludeRoleAsync(new User { Email = User.Identity!.Name! });
                var actualUser = users.FirstOrDefault();

                // Validar existencia
                if (actualUser == null)
                    throw new Exception("No se encontró información del usuario logueado.");

                // Convertir el array de bytes en imagen para mostrar en la vista
                if (actualUser.ImageData != null && actualUser.ImageData.Length > 0)
                {
                    ViewBag.ImageUrl = $"data:image/png;base64,{Convert.ToBase64String(actualUser.ImageData)}";
                }

                return View(actualUser);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // Método para actualizar únicamente la fotografía del usuario logueado
        [Authorize(Roles = "Desarrollador, Administrador, Instructor, Alumno/a, Secretario/a, Invitado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePictureUser(IFormFile imagen)
        {
            try
            {
                // Verificar si el archivo fue seleccionado
                if (imagen == null || imagen.Length == 0)
                {
                    throw new Exception("Debe seleccionar una imagen válida.");
                }

                // Obtener al usuario logueado desde su email
                var users = await userBL.SearchIncludeRoleAsync(new User { Email = User.Identity!.Name! });
                var actualUser = users.FirstOrDefault();

                if (actualUser == null)
                    throw new Exception("No se encontró información del usuario logueado.");

                // Verificar el tamaño del archivo (1.5MB = 1.5 * 1024 * 1024 bytes = 1,572,864 bytes)
                const int maxFileSize = 1572864; // 1.5 MB
                if (imagen.Length > maxFileSize)
                {
                    throw new Exception("La imagen no debe pesar mas de los 1.5MB.");

                }

                byte[] imagenData;
                using (var memoryStream = new MemoryStream())
                {
                    await imagen.CopyToAsync(memoryStream);
                    imagenData = memoryStream.ToArray();
                }

                actualUser.ImageData = imagenData;

                int result = await userBL.UpdatePhotoAsync(actualUser);

                if (result > 0)
                    TempData["SuccessMessageUpdate"] = "Fotografía actualizada exitosamente.";

                return RedirectToAction("UpdatePictureUser");
            }
            catch (Exception e)
            {
                // En caso de error, pasar el mensaje al TempData
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("UpdatePictureUser");
            }
        }
        #endregion

        #region METODO VISTA PREVIA A RECUPERAR CONTRASEÑA
        // Metodo que muestra la vista para buscar y validar si existe el usuario para recuperar la contraseña
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Método para mostrar la vista PasswordSentUser
        [AllowAnonymous]
        public IActionResult PasswordSentUser()
        {
            return View();
        }

        // Método POST para validar si el correo existe y manejar la respuesta
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            // Validamos si el correo existe en la base de datos
            bool emailExists = await UserBL.EmailExistsAsync(email);

            if (emailExists)
            {
                // Si el correo existe, generamos la contraseña temporal y la enviamos por correo
                bool passwordSent = await UserBL.SendTemporaryPasswordAsync(email);

                if (passwordSent)
                {
                    // Si la contraseña fue enviada correctamente, redirige a la vista PasswordSentUser
                    return RedirectToAction("PasswordSentUser");
                }
                else
                {
                    // Si hubo un error al enviar el correo, mostramos la alerta correspondiente
                    TempData["ToastMessage"] = "Error al enviar la contraseña. Intenta de nuevo.";
                    TempData["ToastType"] = "error"; // o "warning" si prefieres otro tipo de notificación
                    return View();
                }
            }
            else
            {
                // Si el correo no existe, mostramos la alerta correspondiente en la misma vista
                TempData["ToastMessage"] = "Usuario no encontrado";
                TempData["ToastType"] = "error";
                return RedirectToAction("ForgotPassword");
            }
        }
        #endregion

        #region METODO PARA MODIFICAR INFORMACION ESPECIFICA DEL USUARIO LOGIADO
        // Muestra La Vista De Informacion Especifica a Modificar
        [Authorize(Roles = "Instructor, Alumno/a, Invitado")]
        public async Task<IActionResult> UpdateInfoUser()
        {
            try
            {
                var users = await userBL.SearchIncludeRoleAsync(new User { Email = User.Identity!.Name! });
                var actualUser = users.FirstOrDefault();

                // Validar existencia
                if (actualUser == null)
                    throw new Exception("No se encontró información del usuario logueado.");

                return View(actualUser);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // Método para actualizar únicamente la fotografía del usuario logueado
        [Authorize(Roles = "Instructor, Alumno/a, Invitado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInfoUser(int id, User user, IFormFile imagen)
        {
            try
            {
                // Verificar que el id coincida con el usuario que se está modificando
                if (id != user.Id)
                {
                    return BadRequest();
                }

                int result = await userBL.UpdateInfoAsync(user);
                TempData["SuccessMessageUpdate"] = "Usuario Modificado Exitosamente";
                return RedirectToAction("Information");
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View(user);
            }
        }
        #endregion
    }
}
