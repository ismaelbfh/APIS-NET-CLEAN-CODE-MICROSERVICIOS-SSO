using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.ApiAdmin.DTOs.Requests
{
    public class VisionIniciarOrdenRequest
    {
        public string OrdenFabricacion { get; set; }
        public string CodigoProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public DateTime? FechaProduccion { get; set; }
        public string? Linea { get; set; }
        public string TipoEtiqueta { get; set; }      // EAN / QR
        public string? PosicionQr { get; set; }       // Superior / Inferior / null
        public string CodigoEsperado { get; set; }
        public string? IpCamara { get; set; }
    }
}
