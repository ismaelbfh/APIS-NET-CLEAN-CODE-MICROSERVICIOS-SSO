using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.GestionEtiquetas.DTOs.Responses
{
    public class EtiquetaLabelsDTO
    {
        public int PK_IdEtiquetaLabel { get; set; }

        public int FK_IdEtiqueta { get; set; }

        public int FK_IdTipoLabel { get; set; }

        public int FK_IdIdioma { get; set; }

        public int FK_IdLabel { get; set; }
        public int? Orden { get; set; }
    }
}
