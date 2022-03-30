using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcesarPIP
{
    [JsonObject(Title = "PIP")]
    public class ProyectoPip
    {
        public string Codigo { get; set; }
        public string CodigoUnico { get; set; }
        public string Estado { get; set; }
        public string Nombre { get; set; }
        public string FechaRegistro { get; set; }
        public string Funcion { get; set; }
        public string Programa { get; set; }
        public string Subprograma { get; set; }
        public string UnidadFormuladora { get; set; }
        public string UnidadFormuladoraCodigo { get; set; }
        public string UnidadUEI { get; set; }
        public string UnidadUEICodigo { get; set; }
        public string UnidadOPMI { get; set; }
        public string UnidadOPMICodigo { get; set; }
        public string NivelGobierno { get; set; }
        public string Sector { get; set; }
        public string Pliego { get; set; }
        public string Evaluadora { get; set; }
        public string EvaluadoraCodigo { get; set; }
        public string Ejecutora { get; set; }
        public string EjecutoraCodigo { get; set; }
        public string Situacion { get; set; }
        public string UltimoEstudio { get; set; }
        public string EstadoUltimoEstudio { get; set; }
        public string NivelEstudio { get; set; }
        public string Beneficiario { get; set; }
        public string FuenteFinanciamiento { get; set; }
        public string MontoAlternativa { get; set; }
        public string MontoReformulado { get; set; }
        public string MontoF15 { get; set; }
        public string MontoF16 { get; set; }
        public string MontoLaudo { get; set; }
        public string MontoCartaFianza { get; set; }
        public decimal CostoActualizado { get; set; }
        public decimal PIM { get; set; }
        //public decimal PIA { get; set; }
        public decimal DevengadoAcumulado { get; set; }
        public decimal DevengadoAnioActual { get; set; }
        public string DesTipoFormato { get; set; }
        public string FlagExpedienteTecnico { get; set; }
        public string AnioViabilidad { get; set; }
        public string FechaViabilidad { get; set; }
        public string Actualizacion { get; set; }
        public int NumeroConvenio { get; set; }
        public string NombreProgramaInversion { get; set; }
        public string Marco { get; set; }
        public string ConInformeCierre { get; set; }
        public string IncluidoPMI { get; set; }
        public string IncluidoPMIEjecucion { get; set; }
        public string FlagEtapas { get; set; }

        [JsonProperty("listalocalizacion")]
        public ListadoLocalizacion Detalle { get; set; }
    }

    public class ListadoLocalizacion
    {

        [JsonConverter(typeof(SingleObjectOrArrayJsonConverter<ProyectoPipLocalizacion>))]
        [JsonProperty("ProyectoLocalizacion")]
        public List<ProyectoPipLocalizacion> Localizaciones { get; set; } = new List<ProyectoPipLocalizacion>();
    }

    public class ProyectoPipLocalizacion
    {
        public string Codigo { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }
        public string CentroPoblado { get; set; }
        public string Ubigeo { get; set; }
        [JsonProperty("latitud")]
        public string Latitud { get; set; }
        [JsonProperty("longitud")]
        public string Longitud { get; set; }
    }

    public class SingleObjectOrArrayJsonConverter<T> : JsonConverter<List<T>> where T : class, new()
    {
        public override void WriteJson(JsonWriter writer, List<T> value, JsonSerializer serializer) =>
            // avoid possibility of infinite recursion by wrapping the List<T> with AsReadOnly()
            serializer.Serialize(writer, value.Count == 1 ? (object)value.Single() : value.AsReadOnly());

        public override List<T> ReadJson(JsonReader reader, Type objectType, List<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            existingValue ??= new List<T>();
            switch (reader.TokenType)
            {
                case JsonToken.StartObject: existingValue.Add(serializer.Deserialize<T>(reader)); break;
                case JsonToken.StartArray: serializer.Populate(reader, existingValue); break;
                default: throw new ArgumentOutOfRangeException($"Converter does not support JSON token type {reader.TokenType}.");
            };
            return existingValue;
        }
    }
}
