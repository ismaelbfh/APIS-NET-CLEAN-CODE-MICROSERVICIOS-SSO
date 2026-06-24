using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.GestionEtiquetas.DTOs.Responses
{
    public class PlantillaDTO
    {
        public int PK_IdPlantilla { get; set; }
        public string NombrePlantilla { get; set; }
        public string RutaArchivo { get; set; }
        public string? UsuarioCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string? UsuarioModificacion { get; set; }
        public string? RutaOvalo { get; set; }
        public bool? IsDeleted { get; set; }

        // Lista de campos asociados a la plantilla
        public List<PlantillaCampoDTO> PlantillaCampos { get; set; }
    }
}
