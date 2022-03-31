using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NextSIT.Utility;

namespace ProcesarPIP
{
    class Program
    {

        private static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
            var conexionBd = "";
            var numeroReintentos = "";
            var servidorSmtp = "";
            var puertoSmtp = "";
            var usuarioSmtp = "";
            var claveSmtp = "";
            var deSmtp = "";
            var paraSmtp = "";
            var servicioWeb = "";
            try
            {
                conexionBd = string.IsNullOrEmpty(args[0]) ? "" : args[0];
                numeroReintentos = string.IsNullOrEmpty(args[1]) ? "" : args[1];
                servidorSmtp = string.IsNullOrEmpty(args[2]) ? "" : args[2];
                puertoSmtp = string.IsNullOrEmpty(args[3]) ? "" : args[3];
                usuarioSmtp = string.IsNullOrEmpty(args[4]) ? "" : args[4];
                claveSmtp = string.IsNullOrEmpty(args[5]) ? "" : args[5];
                deSmtp = string.IsNullOrEmpty(args[6]) ? "" : args[6];
                paraSmtp = string.IsNullOrEmpty(args[7]) ? "" : args[7];
                servicioWeb = string.IsNullOrEmpty(args[8]) ? "" : args[8];
            }
            catch (Exception)
            {
                Console.WriteLine($"Algunos parametros no han sido transferidos a la consola, se utilizaran los valores por defecto");
            }

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            var mailConfiguration = new Mail
            {
                Servidor = !string.IsNullOrEmpty(servidorSmtp) ? servidorSmtp : Configuration.GetSection("Servidor").Value.ToString(),
                Puerto = int.Parse(!string.IsNullOrEmpty(puertoSmtp) ? puertoSmtp : Configuration.GetSection("Puerto").Value.ToString()),
                Usuario = !string.IsNullOrEmpty(usuarioSmtp) ? usuarioSmtp : Configuration.GetSection("Usuario").Value.ToString(),
                Clave = !string.IsNullOrEmpty(claveSmtp) ? claveSmtp : Configuration.GetSection("Clave").Value.ToString(),
                De = !string.IsNullOrEmpty(deSmtp) ? deSmtp : Configuration.GetSection("De").Value.ToString(),
                Para = !string.IsNullOrEmpty(paraSmtp) ? paraSmtp : Configuration.GetSection("Para").Value.ToString()
            };

            EjecutarProceso(
                !string.IsNullOrEmpty(conexionBd) ? conexionBd : Configuration.GetConnectionString("ConexionPcm"),
                int.Parse(!string.IsNullOrEmpty(numeroReintentos) ? numeroReintentos : Configuration.GetSection("NumeroReintentosMaximo").Value.ToString()),
                mailConfiguration,
                !string.IsNullOrEmpty(servicioWeb) ? servicioWeb : Configuration.GetSection("ServicioBancoInversiones").Value.ToString())
                .GetAwaiter()
                .GetResult();
        }

        static async Task EjecutarProceso(string conexion, int numeroReintentosMaximo, Mail mail, string servicioBancoInversiones)
        {
            try
            {
                Console.WriteLine($"-----------------------------------------------------------------------------");
                Console.WriteLine($"    Proceso de carga de Proyectos de Inversion para el anio configurado");
                Console.WriteLine($"-----------------------------------------------------------------------------");
                var mensajeRespuesta = @"<h3>Proceso de carga de Proyectos de Inversion</h3><p>mensaje_respuesta</p>";
                var proxyManager = ProxyManager.GetNewProxyManager();
                var typeConvertionsManager = TypeConvertionManager.GetNewTypeConvertionManager();
                var fileManager = FileManager.GetNewFileManager();
                var request = new ProxyManager.Request();
                var repositorio = new Repositorio(conexion);

                var listaWebService = await repositorio.ObtenerListadoInvocaciones();
                var listaErrados = new List<string>();
                Console.WriteLine($"Numero de invocaciones que tendra el servicio : {listaWebService.Count}");
                Console.WriteLine($"Se inicia el proceso de carga a la base de datos desde el servicio");
                Console.WriteLine($"Numero de reintentos maximos para consulta de servicio : {numeroReintentosMaximo}");

                foreach (var invocacion in listaWebService)
                {
                    var pipTable = new List<Pip>();
                    var pipLocalizacionTable = new List<PipLocalizacion>();

                    var pip = await repositorio.ObtenerPip(servicioBancoInversiones, invocacion, numeroReintentosMaximo);
                    Console.WriteLine($"{(pip == null ? "No se ha recuperado informacion del PIP" : "Informacion del PIP recuperada correctamente")}");
                    //Elimina las existencias anteriores si hay registros nuevos
                    if (pip == null)
                    {
                        listaErrados.Add($"<tr><td>{invocacion.IdProyecto}</td><td>No se recuperó información del proyecto para el año configurado</td></tr>");
                        continue;
                    }

                    var haSidoEliminadoPip = await repositorio.EliminarPip(invocacion);

                    if (!haSidoEliminadoPip)
                    {
                        listaErrados.Add($"<tr><td>{invocacion.IdProyecto}</td><td>No se ha podido eliminar informacion previa del proyecto</td></tr>");
                        continue;
                    }

                    var haSidoEliminadoPipLocalizacion = await repositorio.EliminarPipLocalizacion(invocacion);

                    if (!haSidoEliminadoPipLocalizacion)
                    {
                        listaErrados.Add($"<tr><td>{invocacion.IdProyecto}</td><td>No se ha podido eliminar informacion previa de la localizacion del proyecto</td></tr>");
                        continue;
                    }

                    Console.WriteLine($"Existencias previas eliminadas para el proyecto {invocacion.IdProyecto} del anio {invocacion.Anio}");
                    pipTable.Add(Pip.PipMap(pip, int.Parse(invocacion.Anio)));

                    var pipDataTable = typeConvertionsManager.ArrayListToDataTable(new ArrayList(pipTable));
                    var haSidoRegistradoPip = repositorio.RegistrarPip(pipDataTable);

                    if (!haSidoRegistradoPip)
                    {
                        listaErrados.Add($"<tr><td>{invocacion.IdProyecto}</td><td>No se han podido registrar el proyecto</td></tr>");
                        continue;
                    }

                    pipLocalizacionTable.Add(PipLocalizacion.PipLocalizacionMap(pip, int.Parse(invocacion.Anio)));

                    var pipLocalizacionDataTable = typeConvertionsManager.ArrayListToDataTable(new ArrayList(pipLocalizacionTable));
                    var haSidoRegistradoPipLocalizacion = repositorio.RegistrarPipLocalizacion(pipLocalizacionDataTable);

                    if (!haSidoRegistradoPipLocalizacion)
                    {
                        listaErrados.Add($"<tr><td>{invocacion.IdProyecto}</td><td>No se han podido registrar la localizacion del proyecto</td></tr>");
                        continue;
                    }
                    Console.WriteLine($"Proyecto {invocacion.IdProyecto} registrado correctamente para el anio {invocacion.Anio}");
                    var haSidoActualizado = await repositorio.ActualizarPip(invocacion);
                    if (!haSidoActualizado)
                    {
                        listaErrados.Add($"<tr><td>{invocacion.IdProyecto}</td><td>No se ha podido actualizar el estado del proyecto, se debe volver a procesar</td></tr>");
                        continue;
                    }

                }

                var detalle = listaErrados.Count == 0 ?
                   "Los proyectos se han registrado correctamente" :
                   listaErrados.Count == listaWebService.Count ?
                   "No se han procesado los proyectos, por favor revisar el proceso ETL configurado" :
                   "Los proyectos se han procesado parcialmente, sin embargo existen algunas observaciones:";

                var listadoDetalle = $"<table><thead><tr><th>Proyecto</th><th>Mensaje de Error</th></tr></thead><tbody>{string.Join(' ', listaErrados)}</tbody></table>";

                mensajeRespuesta = mensajeRespuesta.Replace("mensaje_respuesta", detalle);
                mensajeRespuesta += listadoDetalle;

                repositorio.SendMail(mail, "Proceso de Carga Masiva de Datos de Proyectos", mensajeRespuesta);

                return;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }

        }

    }
}
