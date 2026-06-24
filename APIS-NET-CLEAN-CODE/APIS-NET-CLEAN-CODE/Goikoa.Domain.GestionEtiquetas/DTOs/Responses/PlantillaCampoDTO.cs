using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.GestionEtiquetas.DTOs.Responses
{
    public class PlantillaCampoDTO
    {
        public int? PK_IdPlantillaCampo { get; set; }  // Puede ser nulo en alta
        public int FK_IdPlantilla { get; set; }
        public int FK_IdCampo { get; set; }
        public string NombreCampoBartender { get; set; }
        public int Orden { get; set; }
        public bool EsObligatorio { get; set; }
    }
}
