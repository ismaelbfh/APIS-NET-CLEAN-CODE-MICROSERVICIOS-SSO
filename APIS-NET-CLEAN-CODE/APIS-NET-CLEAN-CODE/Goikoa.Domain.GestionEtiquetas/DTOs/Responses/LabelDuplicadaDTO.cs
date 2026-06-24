using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.GestionEtiquetas.DTOs.Responses
{
    public class LabelDuplicadaDTO
    {
        public bool Existe { get; set; }
        public string Tipo { get; set; }
        public string Idioma { get; set; }
    }
}
