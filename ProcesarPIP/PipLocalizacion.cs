using System;

namespace ProcesarPIP
{
    public class PipLocalizacion
    {
        public string Codigo { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }
        public string CentroPoblado { get; set; }
        public string Ubigeo { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Cod_Proyecto { get; set; }
        public int Anio { get; set; }

        public static Func<ProyectoPip, PipLocalizacion> ProyeccionPipLocalizacion(int anio)
        {
            return pip => {
                if(pip.Detalle == null)
                {
                    return null;
                }

                if (pip.Detalle.Localizaciones == null)
                {
                    return null;
                }

                if (pip.Detalle.Localizaciones.Count == 0)
                {
                    return null;
                }

                var pipLocalizacion = pip.Detalle.Localizaciones[0];

                return new PipLocalizacion
                {
                    Codigo = pip.CodigoUnico,
                    Departamento = pipLocalizacion.Departamento,
                    Provincia = pipLocalizacion.Provincia,
                    Distrito = pipLocalizacion.Distrito,
                    CentroPoblado = pipLocalizacion.CentroPoblado,
                    Ubigeo = pipLocalizacion.Ubigeo,
                    Latitud = pipLocalizacion.Latitud,
                    Longitud = pipLocalizacion.Longitud,
                    Cod_Proyecto = pipLocalizacion.Codigo,
                    Anio = anio
                };

            };
        }

        public static Func<ProyectoPip, int, PipLocalizacion> PipLocalizacionMap = (pip, anio) =>
        {
            if (pip.Detalle == null)
            {
                return null;
            }

            if (pip.Detalle.Localizaciones == null)
            {
                return null;
            }

            if (pip.Detalle.Localizaciones.Count == 0)
            {
                return null;
            }

            var pipLocalizacion = pip.Detalle.Localizaciones[0];

            return new PipLocalizacion
            {
                Codigo = pip.CodigoUnico,
                Departamento = pipLocalizacion.Departamento,
                Provincia = pipLocalizacion.Provincia,
                Distrito = pipLocalizacion.Distrito,
                CentroPoblado = pipLocalizacion.CentroPoblado,
                Ubigeo = pipLocalizacion.Ubigeo,
                Latitud = pipLocalizacion.Latitud,
                Longitud = pipLocalizacion.Longitud,
                Cod_Proyecto = pipLocalizacion.Codigo,
                Anio = anio
            };
        };
    }
}
