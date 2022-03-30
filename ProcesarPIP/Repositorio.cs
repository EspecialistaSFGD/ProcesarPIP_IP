using Dapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using NextSIT.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProcesarPIP
{
    public class Repositorio
    {
        private readonly string Conexion = "";
        private readonly ProxyManager proxyManager;
        private readonly TypeConvertionManager typeConvertionsManager;
        private readonly int TiempoEsperaCargadoMasivo;
        private readonly int BatchSize;

        public Repositorio(string conexion)
        {
            Conexion = conexion;
            proxyManager = ProxyManager.GetNewProxyManager();
            typeConvertionsManager = TypeConvertionManager.GetNewTypeConvertionManager();
            TiempoEsperaCargadoMasivo = 1000;
            BatchSize = 50000;
        }

        //Paso 1.- Obtener el listado de Ejecutoras
        public async Task<List<WebServiceEjecutar>> ObtenerListadoInvocaciones()
        {
            using var conexionSql = new SqlConnection(Conexion);
            conexionSql.Open();
            try
            {
                var respuesta = await conexionSql.QueryAsync<WebServiceEjecutar>("dbo.03A_GenerarListadoPip", commandType: CommandType.StoredProcedure, commandTimeout: 1200);
                return respuesta.ToList();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
            finally
            {
                conexionSql.Close();
            }
        }

        //Paso 2.- Recuperar los datos del proyecto
        public async Task<ProyectoPip> ObtenerPip(string urlServicio, WebServiceEjecutar invocacion, int numeroReintentosMaximo)
        {
            ProyectoPip proyecto = null;
            try
            {
                var numeroReintento = 0;
                var request = new ProxyManager.Request();
                Console.WriteLine($"Cuerpo de la consulta (proyecto) : { invocacion.UrlWebService }.");
                request.HttpMethod = ProxyManager.HttpMethod.Post;
                request.Uri = urlServicio;
                request.Body = invocacion.UrlWebService;
                request.MediaType = ProxyManager.MediaType.Xml;
                var respuesta = new ProxyManager.Response { Ok = false };
                while (!respuesta.Ok && (numeroReintento <= numeroReintentosMaximo))
                {
                    if (numeroReintento > 0)
                    {
                        Console.WriteLine($"Se procede con el reintento numero : {numeroReintento} de consulta al servicio");
                    }
                    try
                    {
                        respuesta = await proxyManager.CallServiceAsync(request);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"Error al intentar comunicarse con el servicio del MEF. Detalle del error => {exception.Message}");
                        Thread.Sleep(1000);
                        respuesta.Ok = false;
                    }
                    numeroReintento++;
                }

                if (respuesta.Ok)
                {
                    proyecto = typeConvertionsManager.XmlStringToObject<ProyectoPip>(respuesta.ResponseBody, "soap:Envelope.soap:Body.GetPipResponse.GetPipResult.PIP");
                }

                if (proyecto != null)
                {
                    proyecto.Codigo = invocacion.IdProyecto;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Ocurrio un problema al intentar recuperar la informacion del servicio.\nError Asociado: {exception.Message}");
                proyecto = null;
            }
            return proyecto;
        }
        
        //Paso 3.- Eliminar PIP antiguo 
        public async Task<bool> EliminarPip(WebServiceEjecutar proyecto)
        {
            using var conexionSql = new SqlConnection(Conexion);
            try
            {
                conexionSql.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@IdProyecto", proyecto.IdProyecto, DbType.String, ParameterDirection.Input, 10);

                var respuesta = await conexionSql.QueryAsync("dbo.03B_EliminarPip", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 1200);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
            finally
            {
                conexionSql.Close();
            }

        }

        //Paso 4.- Eliminar Localizacion de PIP antiguo 
        public async Task<bool> EliminarPipLocalizacion(WebServiceEjecutar proyecto)
        {
            using var conexionSql = new SqlConnection(Conexion);
            try
            {
                conexionSql.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@IdProyecto", proyecto.IdProyecto, DbType.String, ParameterDirection.Input, 10);

                var respuesta = await conexionSql.QueryAsync("dbo.03C_EliminarPipLocalizacion", commandType: CommandType.StoredProcedure, commandTimeout: 1200);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
            finally
            {
                conexionSql.Close();
            }

        }

        //Paso 5.- Registrar el PIP
        public bool RegistrarPip(DataTable valores)
        {
            using var conexionSql = new SqlConnection(Conexion);
            conexionSql.Open();

            using SqlBulkCopy bulkCopy = new(conexionSql);
            bulkCopy.BulkCopyTimeout = TiempoEsperaCargadoMasivo;
            bulkCopy.BatchSize = BatchSize;
            bulkCopy.DestinationTableName = "dbo.PIP";

            try
            {
                foreach (DataColumn column in valores.Columns)
                {
                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
                }

                bulkCopy.WriteToServer(valores);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                # region Para revisar al detalle las columnas que presentan problemas
                //string pattern = @"\d+";
                //Match match = Regex.Match(exception.Message.ToString(), pattern);
                //var index = Convert.ToInt32(match.Value) - 1;
                //
                //FieldInfo fi = typeof(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic | BindingFlags.Instance);
                //var sortedColumns = fi.GetValue(bulkCopy);
                //var items = (Object[])sortedColumns.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sortedColumns);
                //
                //FieldInfo itemdata = items[index].GetType().GetField("_metadata", BindingFlags.NonPublic | BindingFlags.Instance);
                //var metadata = itemdata.GetValue(items[index]);
                //
                //var column = metadata.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                //var length = metadata.GetType().GetField("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                #endregion
                throw;

            }
            finally
            {
                conexionSql.Close();
            }
        }

        //Paso 6.- Registrar la localizacion del PIP
        public bool RegistrarPipLocalizacion(DataTable valores)
        {
            using var conexionSql = new SqlConnection(Conexion);
            conexionSql.Open();

            using SqlBulkCopy bulkCopy = new(conexionSql);
            bulkCopy.BulkCopyTimeout = TiempoEsperaCargadoMasivo;
            bulkCopy.BatchSize = BatchSize;
            bulkCopy.DestinationTableName = "dbo.PIP_Localizacion";

            try
            {
                foreach (DataColumn column in valores.Columns)
                {
                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
                }

                bulkCopy.WriteToServer(valores);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                # region Para revisar al detalle las columnas que presentan problemas
                //string pattern = @"\d+";
                //Match match = Regex.Match(exception.Message.ToString(), pattern);
                //var index = Convert.ToInt32(match.Value) - 1;
                //
                //FieldInfo fi = typeof(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic | BindingFlags.Instance);
                //var sortedColumns = fi.GetValue(bulkCopy);
                //var items = (Object[])sortedColumns.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sortedColumns);
                //
                //FieldInfo itemdata = items[index].GetType().GetField("_metadata", BindingFlags.NonPublic | BindingFlags.Instance);
                //var metadata = itemdata.GetValue(items[index]);
                //
                //var column = metadata.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                //var length = metadata.GetType().GetField("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                #endregion
                throw;

            }
            finally
            {
                conexionSql.Close();
            }
        }

        //Paso 7.- Actualizar el proyecto a estado procesado
        public async Task<bool> ActualizarPip(WebServiceEjecutar proyecto)
        {
            using var conexionSql = new SqlConnection(Conexion);
            try
            {
                conexionSql.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@IdProyecto", proyecto.IdProyecto, DbType.String, ParameterDirection.Input, 10);

                var respuesta = await conexionSql.QueryAsync<WebServiceEjecutar>("dbo.03D_ActualizarProyecto", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 1200);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
            finally
            {
                conexionSql.Close();
            }

        }

        //Paso 8.- Enviar mail por concepto de error o éxito
        public void SendMail(Mail configuracion, string asunto, string mensaje)
        {
            try
            {
                // create message
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(configuracion.De);
                string[] destinatarios = configuracion.Para.Split(";");

                foreach (string destinatario in destinatarios) email.To.Add(MailboxAddress.Parse(destinatario));
                email.Subject = asunto;//"Notificaciones Mapa Inversiones - Sincronizacion de Datos del MEF";
                email.Body = new TextPart(TextFormat.Html) { Text = mensaje };

                // send email
                using var smtp = new SmtpClient();
                smtp.Connect(configuracion.Servidor, configuracion.Puerto, SecureSocketOptions.StartTls);
                smtp.Authenticate(configuracion.De, configuracion.Clave);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Ocurrio un problema al enviar la notificacion de la carga fallida. Detalle del error => { exception.Message }");
            }
        }

    }
}
