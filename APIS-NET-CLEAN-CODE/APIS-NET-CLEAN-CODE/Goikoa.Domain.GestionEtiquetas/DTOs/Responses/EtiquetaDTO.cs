using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goikoa.Domain.GestionEtiquetas.DAL.Models;

namespace Goikoa.Domain.GestionEtiquetas.DTOs.Responses
{
    public class EtiquetaDTO
    {
        public int PK_IdEtiqueta { get; set; }
        public int FK_IdPlantilla { get; set; }
        public int? FK_IdTipoEtiqueta { get; set; }
        public int? FK_IdConservacion { get; set; }
        public int? FK_IdSeccion { get; set; }
        public int? FK_IdCliente { get; set; }
        public int? FK_IdEnvasado { get; set; }
        public string? NombreEtiqueta { get; set; }
        public string? CodigoProducto { get; set; }
        public DateTime? FechaGeneracion { get; set; }
        public string? Comentario { get; set; }
        public bool? IsDeleted { get; set; }
        public int? Version { get; set; }
        public DateTime? FechaModificacion { get; set; }

        public string OvaloIdioma { get; set; }

        public string OvaloPlanta { get; set; }

        public string OvaloCE { get; set; }
        public List<EtiquetaCodigoBarraDTO> EtiquetaCodigosBarra { get; set; } = new();
        public int? FK_IdUbicacion { get; set; }
        public string? Usuario { get; set; }
        public List<EtiquetaCampoValorDTO> EtiquetaCamposValores { get; set; } = new List<EtiquetaCampoValorDTO>();
        public virtual List<EtiquetaLabelsDTO> EtiquetaLabels { get; set; } = new List<EtiquetaLabelsDTO>();
    }
}
