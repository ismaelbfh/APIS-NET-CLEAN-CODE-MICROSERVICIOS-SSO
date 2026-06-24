using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.ApiAdmin.DTOs.Responses
{
    public class VisionLecturaDTO
    {
        public Guid Id { get; set; }
        public Guid VisionOrdenResumenId { get; set; }
        public DateTime FechaHoraLectura { get; set; }
        public string CodigoEsperado { get; set; }
        public string CodigoLeido { get; set; }
        public string Resultado { get; set; }
        public string? RawMensaje { get; set; }
        public string? Observaciones { get; set; }
        public string? TipoEtiqueta { get; set; }      // EAN / QR
    }
}
