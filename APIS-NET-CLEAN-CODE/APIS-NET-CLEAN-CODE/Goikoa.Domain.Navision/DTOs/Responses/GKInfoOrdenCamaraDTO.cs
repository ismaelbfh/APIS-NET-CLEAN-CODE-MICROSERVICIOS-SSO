using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.Navision.Producccion.DTOs.Responses
{
    public class GKInfoOrdenCamaraDTO
    {
        public string OPFabricacion { get; set; }
        public string CodigoProducto { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public string Linea { get; set; }
        public string CodigoEmbalaje { get; set; }
        public decimal CantidadAProducirEmbalajes { get; set; }
    }
}
