using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.GestionEtiquetas.DTOs.Requests
{
    public class CampoDTORequest
    {
        public int PkIdCampo { get; set; }
        public bool? IsNavision { get; set; }

        public string NombreCampo { get; set; }

        public int? IdTipoCampo { get; set; }
        
        public int? IdCampoNavision { get; set; }
    }
}
