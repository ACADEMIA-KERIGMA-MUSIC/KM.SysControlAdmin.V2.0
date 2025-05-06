#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using System.Net.Mail;
using System.Net;

#endregion

namespace KM.SysControlAdmin.Core.Utils
{
    public class UtilsEmailService
    {
        public static async Task<bool> SendTemporaryPassword(string toEmail, string temporaryPassword)
        {
            try
            {
                var fromEmail = "soporteplataforma2025@gmail.com";
                var fromPassword = "tghv zaje nhrb aopa"; // Usa credenciales seguras en producción

                var smtpClient = new SmtpClient("smtp.gmail.com") // Ej: smtp.gmail.com
                {
                    Port = 587, // 465 para SSL o 587 para TLS
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    EnableSsl = true
                };

                var currentYear = DateTime.Now.GetFechaZonaHoraria().Year;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "Soporte Técnico Kerigma Music"),
                    Subject = "Contraseña Temporal",
                    IsBodyHtml = true,
                    Body = $@"
                    <!DOCTYPE html>
                    <html lang='es'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta http-equiv='X-UA-Compatible' content='IE=edge'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Contraseña Temporal</title>
                    </head>
                    <body style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                        <div style='max-width: 600px; margin: auto; background-color: #fff; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                            <div style='text-align: center;'>
                                <h1>Academia Kerigma Music</h1>
                            </div>
                            <h2 style='color: #333;'>Hola,</h2>
                            <p style='font-size: 16px; color: #555;'>
                                Has solicitado restablecer tu contraseña en <strong>Academia Kerigma Music</strong>.
                            </p>
                            <p style='font-size: 16px; color: #555;'>
                                Usa la siguiente contraseña temporal para acceder a tu cuenta:
                            </p>
                            <div style='font-size: 28px; font-weight: bold; color: #2c3e50; margin: 20px 0; text-align: center;'>
                                {temporaryPassword}
                            </div>
                            <p style='font-size: 16px; color: #555;'>
                                Te recomendamos cambiarla inmediatamente después de iniciar sesión en la sección de 'Seguridad'.
                            </p>
                            <hr style='margin: 30px 0;'/>
                            <p style='font-size: 12px; color: #999; text-align: center;'>
                                Si no solicitaste este cambio, informa inmediatamente al proveedor del sistema o al Soporte Técnico.<br/>
                                © Kerigma Music {currentYear}. Todos los derechos reservados.
                            </p>
                        </div>
                    </body>
                    </html>"
                };


                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
