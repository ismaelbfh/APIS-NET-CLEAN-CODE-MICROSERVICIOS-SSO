using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.ApiAdmin.DTOs.Requests
{
    public class VisionActualizarOrdenRequest
    {
        public Guid VisionOrdenResumenId { get; set; }
        public string CodigoEsperado { get; set; }
        public string TipoEtiqueta { get; set; }
        public string? PosicionQr { get; set; }
    }
}
