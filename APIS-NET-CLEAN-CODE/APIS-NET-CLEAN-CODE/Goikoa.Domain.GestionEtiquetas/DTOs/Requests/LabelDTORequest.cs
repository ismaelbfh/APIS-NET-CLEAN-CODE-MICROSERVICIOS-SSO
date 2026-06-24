using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.GestionEtiquetas.DTOs.Requests
{
    public class LabelDTORequest
    {
        public int PKIdLabel { get; set; }

        public int FKIdTipoLabel { get; set; }

        public int FKIdIdioma { get; set; }

        public string DescripcionLabel { get; set; }
    }
}
