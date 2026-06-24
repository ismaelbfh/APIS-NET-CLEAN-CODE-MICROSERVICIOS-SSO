using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.Navision.Producccion.DTOs.Responses
{
    public class GKProductoDTO
    {
        public string CodigoProducto { get; set; }
        public string TipoCodigoBarras { get; set; }
        public string CodificacionCodigoBarras { get; set; }
    }
}
