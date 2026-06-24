using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.GestionEtiquetas.DTOs.Responses
{
    public class LabelDTO
    {
        public int PkIdLabel { get; set; }
        public string DescripcionLabel { get; set; }
        public int IdTipoLabel { get; set; }
        public string DescripcionTipoLabel { get; set; }
        public int IdIdiomaLabel { get; set; }
        public string DescripcionIdiomaLabel { get; set; }
    }
}
