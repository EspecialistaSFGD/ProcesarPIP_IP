using System;
using Newtonsoft.Json;

namespace ProcesarPIP
{
    [JsonObject(Title = "PIP")]
    public class Pip
    {
        public string Codigo { get; set; }
        public string Estado { get; set; }
        public string Nombre { get; set; }
        public string FechaRegistro { get; set; }
        public string TipoCadenaFunc { get; set; }
        public string FuncionCodigo { get; set; }
        public string ProgramaCodigo { get; set; }
        public string SubprogramaCodigo { get; set; }
        public string Funcion { get; set; }
        public string Programa { get; set; }
        public string Subprograma { get; set; }
        public string UnidadFormuladora { get; set; }
        public string UnidadFormuladoraCodigo { get; set; }
        public string NivelGobierno { get; set; }
        public string Sector { get; set; }
        public string SectorCodigo { get; set; }
        public string Pliego { get; set; }
        public string PliegoCodigo { get; set; }
        public string Evaluadora { get; set; }
        public string EvaluadoraCodigo { get; set; }
        public string Ejecutora { get; set; }
        public string EjecutoraCodigo { get; set; }
        public string Situacion { get; set; }
        public string UltimoEstudio { get; set; }
        public string EstadoUltimoEstudio { get; set; }
        public string Beneficiario { get; set; }
        public string ConglomeradoCodigo { get; set; }
        public string FuenteFinanciamiento { get; set; }
        public string MontoAlternativa { get; set; }
        public string MontoReformulado { get; set; }
        public string MontoF15 { get; set; }
        public string MontoF16 { get; set; }
        public decimal CostoActualizado { get; set; }
        public string DesTipoFormato { get; set; }
        public string FlagExpedienteTecnico { get; set; }
        public string AnioViabilidad { get; set; }
        public string FechaViabilidad { get; set; }
        public int NumeroConvenio { get; set; }
        public string EncarganteConvenio { get; set; }
        public string NombreConglomerado { get; set; }
        public string NombreProgramaInversion { get; set; }
        public string ModalidadEjecucion { get; set; }
        public string Marco { get; set; }
        public string ConInformeCierre { get; set; }
        public string IncluidoPMI { get; set; }
        public string IncluidoPMIEjecucion { get; set; }
        public string Cod_Proyecto { get; set; }
        public int Anio { get; set; } 
        public DateTime FechaInsercion { get; set; } = DateTime.Now;

        public static Func<ProyectoPip, Pip> ProyeccionPip(int anio)
        {
            return pip => new Pip
            {
                Codigo = pip.CodigoUnico,
                Estado = pip.Estado ?? "",
                Nombre = pip.Nombre ?? "",
                FechaRegistro = pip.FechaRegistro ?? "",
                TipoCadenaFunc = "0",
                FuncionCodigo = "00",
                ProgramaCodigo = "000",
                SubprogramaCodigo = "0000",
                Funcion = pip.Funcion ?? "",
                Programa = pip.Programa ?? "",
                Subprograma = pip.Subprograma ?? "",
                UnidadFormuladora = pip.UnidadFormuladora ?? "",
                UnidadFormuladoraCodigo = pip.UnidadFormuladoraCodigo ?? "",
                NivelGobierno = pip.NivelGobierno ?? "",
                Sector = pip.Sector ?? "",
                SectorCodigo = "00",
                Pliego = pip.Pliego ?? "",
                PliegoCodigo = "",
                Evaluadora = pip.Evaluadora ?? "",
                EvaluadoraCodigo = pip.EvaluadoraCodigo ?? "",
                Ejecutora = pip.Ejecutora ?? "",
                EjecutoraCodigo = pip.EjecutoraCodigo ?? "",
                Situacion = pip.Situacion ?? "",
                UltimoEstudio = pip.UltimoEstudio ?? "",
                EstadoUltimoEstudio = pip.EstadoUltimoEstudio ?? "",
                Beneficiario = string.IsNullOrEmpty(pip.Beneficiario.Trim()) ? "0" : pip.Beneficiario.Trim(),
                ConglomeradoCodigo = "",
                FuenteFinanciamiento  = pip.FuenteFinanciamiento ?? "",
                MontoAlternativa = string.IsNullOrEmpty(pip.MontoAlternativa.Trim()) ? "0" : pip.MontoAlternativa.Trim(),
                MontoReformulado = string.IsNullOrEmpty(pip.MontoReformulado.Trim()) ? "0" : pip.MontoReformulado.Trim(),
                MontoF15 = string.IsNullOrEmpty(pip.MontoF15.Trim()) ? "0" : pip.MontoF15.Trim(),
                MontoF16 = string.IsNullOrEmpty(pip.MontoF16.Trim()) ? "0" : pip.MontoF16.Trim(),
                CostoActualizado = pip.CostoActualizado,
                DesTipoFormato = string.IsNullOrEmpty(pip.DesTipoFormato.Trim()) ? "" : pip.DesTipoFormato.Trim(),
                FlagExpedienteTecnico = string.IsNullOrEmpty(pip.FlagExpedienteTecnico.Trim()) ? "0" : pip.FlagExpedienteTecnico.Trim(),
                AnioViabilidad = pip.AnioViabilidad ?? "",
                FechaViabilidad = pip.FechaViabilidad ?? "",
                NumeroConvenio = pip.NumeroConvenio,
                EncarganteConvenio = "",
                NombreConglomerado = "",
                NombreProgramaInversion = pip.NombreProgramaInversion ?? "",
                ModalidadEjecucion = "",
                Marco = "",
                ConInformeCierre = string.IsNullOrEmpty(pip.ConInformeCierre.Trim()) ? "0" : pip.ConInformeCierre.Trim(),
                IncluidoPMI = string.IsNullOrEmpty(pip.IncluidoPMI.Trim()) ? "0" : pip.IncluidoPMI.Trim(),
                IncluidoPMIEjecucion = string.IsNullOrEmpty(pip.IncluidoPMIEjecucion.Trim()) ? "0" : pip.IncluidoPMIEjecucion.Trim(),
                Cod_Proyecto = pip.Codigo,
                Anio = anio,
                FechaInsercion = DateTime.Now
            };
        }


        public static Func<ProyectoPip, int, Pip> PipMap = (pip, anio) => new Pip
            {
                Codigo = pip.CodigoUnico,
                Estado = pip.Estado ?? "",
                Nombre = pip.Nombre ?? "",
                FechaRegistro = pip.FechaRegistro ?? "",
                TipoCadenaFunc = "0",
                FuncionCodigo = "00",
                ProgramaCodigo = "000",
                SubprogramaCodigo = "0000",
                Funcion = pip.Funcion ?? "",
                Programa = pip.Programa ?? "",
                Subprograma = pip.Subprograma ?? "",
                UnidadFormuladora = pip.UnidadFormuladora ?? "",
                UnidadFormuladoraCodigo = pip.UnidadFormuladoraCodigo ?? "",
                NivelGobierno = pip.NivelGobierno ?? "",
                Sector = pip.Sector ?? "",
                SectorCodigo = "00",
                Pliego = pip.Pliego ?? "",
                PliegoCodigo = "",
                Evaluadora = pip.Evaluadora ?? "",
                EvaluadoraCodigo = pip.EvaluadoraCodigo ?? "",
                Ejecutora = pip.Ejecutora ?? "",
                EjecutoraCodigo = pip.EjecutoraCodigo ?? "",
                Situacion = pip.Situacion ?? "",
                UltimoEstudio = pip.UltimoEstudio ?? "",
                EstadoUltimoEstudio = pip.EstadoUltimoEstudio ?? "",
                Beneficiario = string.IsNullOrEmpty(pip.Beneficiario.Trim()) ? "0" : pip.Beneficiario.Trim(),
                ConglomeradoCodigo = "",
                FuenteFinanciamiento = pip.FuenteFinanciamiento ?? "",
                MontoAlternativa = string.IsNullOrEmpty(pip.MontoAlternativa.Trim()) ? "0" : pip.MontoAlternativa.Trim(),
                MontoReformulado = string.IsNullOrEmpty(pip.MontoReformulado.Trim()) ? "0" : pip.MontoReformulado.Trim(),
                MontoF15 = string.IsNullOrEmpty(pip.MontoF15.Trim()) ? "0" : pip.MontoF15.Trim(),
                MontoF16 = string.IsNullOrEmpty(pip.MontoF16.Trim()) ? "0" : pip.MontoF16.Trim(),
                CostoActualizado = pip.CostoActualizado,
                DesTipoFormato = string.IsNullOrEmpty(pip.DesTipoFormato.Trim()) ? "" : pip.DesTipoFormato.Trim(),
                FlagExpedienteTecnico = string.IsNullOrEmpty(pip.FlagExpedienteTecnico.Trim()) ? "0" : pip.FlagExpedienteTecnico.Trim(),
                AnioViabilidad = pip.AnioViabilidad ?? "",
                FechaViabilidad = pip.FechaViabilidad ?? "",
                NumeroConvenio = pip.NumeroConvenio,
                EncarganteConvenio = "",
                NombreConglomerado = "",
                NombreProgramaInversion = pip.NombreProgramaInversion ?? "",
                ModalidadEjecucion = "",
                Marco = "",
                ConInformeCierre = string.IsNullOrEmpty(pip.ConInformeCierre.Trim()) ? "0" : pip.ConInformeCierre.Trim(),
                IncluidoPMI = string.IsNullOrEmpty(pip.IncluidoPMI.Trim()) ? "0" : pip.IncluidoPMI.Trim(),
                IncluidoPMIEjecucion = string.IsNullOrEmpty(pip.IncluidoPMIEjecucion.Trim()) ? "0" : pip.IncluidoPMIEjecucion.Trim(),
                Cod_Proyecto = pip.Codigo,
                Anio = anio,
                FechaInsercion = DateTime.Now
            };
    }
}
