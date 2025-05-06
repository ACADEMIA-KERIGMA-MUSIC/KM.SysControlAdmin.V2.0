using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using static KM.SysControlAdmin.Core.Utils.UtilsRegion;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuración de autenticación con cookies basada en roles
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(static options =>
    {
        options.LoginPath = new PathString("/User/Login");
        options.AccessDeniedPath = new PathString("/Home/Index");
        options.SlidingExpiration = true;

        options.Events = new CookieAuthenticationEvents
        {
            OnSigningIn = async context =>
            {
                var principal = context.Principal;
                if (principal != null)
                {
                    var identity = (ClaimsIdentity)principal.Identity!;
                    if (identity != null)
                    {
                        var roleClaim = identity.FindFirst(ClaimTypes.Role)?.Value;

                        // Definir tiempos de sesión según el rol
                        TimeSpan sessionDuration = roleClaim switch
                        {
                            "Desarrollador" => TimeSpan.FromHours(8),
                            "Administrador" => TimeSpan.FromHours(8),
                            "Instructor" => TimeSpan.FromHours(5),
                            "Alumno/a" => TimeSpan.FromHours(3),
                            "Secretario/a" => TimeSpan.FromHours(5),
                            "Invitado" => TimeSpan.FromHours(1),
                            _ => TimeSpan.FromMinutes(30),            // Otros roles tienen 30 minutos
                        };

                        // Usar la zona horaria personalizada definida en UtilsRegion
                        var fechaZona = DateTime.Now.GetFechaZonaHoraria();

                        // Establecer la expiración en UTC a partir de esa fecha
                        context.Properties.ExpiresUtc = fechaZona.Add(sessionDuration).ToUniversalTime();
                        context.Properties.IsPersistent = true; // Para asegurar que la cookie persista el tiempo asignado
                    }
                }
                await Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Primero autenticación
app.UseAuthorization();  // Luego autorización

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
