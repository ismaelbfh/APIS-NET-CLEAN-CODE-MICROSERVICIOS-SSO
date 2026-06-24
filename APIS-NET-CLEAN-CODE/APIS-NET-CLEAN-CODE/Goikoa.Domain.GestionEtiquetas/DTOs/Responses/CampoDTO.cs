using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.GestionEtiquetas.DTOs.Responses
{
    public class CampoDTO
    {
        public int PkIdCampo { get; set; }
        public bool? IsNavision { get; set; }
        public string NombreCampo { get; set; }

        public int? IdTipoCampo { get; set; }
        public string DescripcionTipoCampo { get; set; }
        
        public int? IdCampoNavision { get; set; } // FK
        public string? DescripcionCampoNavision { get; set; } // Descripción del campo de Navision
    }
}
