using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.GestionEtiquetas.DTOs.Responses
{
    public class EtiquetaCodigoBarraDTO
    {
        public int? PK_IdEtiquetaCodigoBarra { get; set; }
        public int FK_IdTipoCodigoBarra { get; set; }
        public int Orden { get; set; }
    }
}
