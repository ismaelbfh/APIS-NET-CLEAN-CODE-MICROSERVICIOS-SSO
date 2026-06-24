using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.Navision.Producccion.DTOs.Requests
{
    public class GKOrdenProdRequest
    {
        public string pLine { get; set; }
        public int pTipoDestino { get; set; }
        public DateTime pStartingDate { get; set; }

        // Estos campos son opcionales y se les asigna un valor por defecto
        public int pPageNumber { get; set; } = 1;
        public int pPageSize { get; set; } = 20;
    }
}
