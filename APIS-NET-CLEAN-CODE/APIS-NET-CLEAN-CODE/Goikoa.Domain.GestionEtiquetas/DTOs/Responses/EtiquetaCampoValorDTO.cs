using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.GestionEtiquetas.DTOs.Responses
{
    public class EtiquetaCampoValorDTO
    {
        public int? PK_IdEtiquetaCampoValor { get; set; }
        public int? FK_IdEtiqueta { get; set; }
        public int? FK_IdCampo { get; set; }
        public string? ValorCampo { get; set; }
        public string? NombreCampoDespuesDeBorrado { get; set; }
    }
}
