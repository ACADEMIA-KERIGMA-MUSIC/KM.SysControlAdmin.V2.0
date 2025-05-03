#region REFERENCIAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Referencias Necesarias Para El Correcto Funcionamiento
using System.Globalization;

#endregion

namespace KM.SysControlAdmin.Core.Utils
{
    public static class UtilsRegion
    {
        public static DateTime GetFechaZonaHoraria(this DateTime fecha)
        {
            TimeZoneInfo zonaHoraria = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

            if (zonaHoraria.SupportsDaylightSavingTime)
            {
                TimeZoneInfo.AdjustmentRule[] reglasHorarioVerano = zonaHoraria.GetAdjustmentRules();

                foreach (TimeZoneInfo.AdjustmentRule regla in reglasHorarioVerano)
                {
                    TimeZoneInfo.AdjustmentRule nuevaRegla = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(
                        regla.DateStart, regla.DateEnd, TimeSpan.Zero,
                        regla.DaylightTransitionStart, regla.DaylightTransitionEnd);

                    zonaHoraria = TimeZoneInfo.CreateCustomTimeZone(
                        zonaHoraria.Id, zonaHoraria.BaseUtcOffset, zonaHoraria.DisplayName,
                        zonaHoraria.StandardName, zonaHoraria.DaylightName,
                        new TimeZoneInfo.AdjustmentRule[] { nuevaRegla });
                }
            }

            DateTime fechaConvertida = TimeZoneInfo.ConvertTime(fecha, TimeZoneInfo.Local, zonaHoraria);
            return fechaConvertida;
        }

        public static string GetFechaEsp(this DateTime fecha, string formato)
        {
            CultureInfo culture = new CultureInfo("es-ES");
            return fecha.ToString(formato, culture);
        }
    }
}
