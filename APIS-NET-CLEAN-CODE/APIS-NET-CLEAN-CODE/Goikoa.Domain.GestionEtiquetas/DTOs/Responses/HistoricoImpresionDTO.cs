using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.GestionEtiquetas.DTOs.Responses
{
    public class HistoricoImpresionDTO
    {
        public int PK_IdHistoricoImpresion { get; set; }

        public int FK_IdEtiqueta { get; set; }

        public string CodigoOrden { get; set; }

        public string CodigoProducto { get; set; }

        public string NombreEtiqueta { get; set; }

        public string NombrePlantilla { get; set; }

        public string RutaArchivo { get; set; }

        public string TipoEtiqueta { get; set; }

        public string? TipoConservacion { get; set; }

        public string? Seccion { get; set; }

        public string? Cliente { get; set; }

        public string? Envasado { get; set; }

        public string? TipoCodigoBarras { get; set; }

        public string OvaloIdioma { get; set; }

        public string OvaloPlanta { get; set; }

        public string OvaloCE { get; set; }

        public DateTime FechaImpresion { get; set; }

        public string IpMaquina { get; set; }

        public string UsuarioPlanta { get; set; }

        public string UsuarioEtiqueta { get; set; }

        public string JsonPeticionBartender { get; set; }

        public string JsonRespuestaBartender { get; set; }
        
        public string NombreEquipo { get; set; }

        public string UsuarioWindows { get; set; }
    }
}
