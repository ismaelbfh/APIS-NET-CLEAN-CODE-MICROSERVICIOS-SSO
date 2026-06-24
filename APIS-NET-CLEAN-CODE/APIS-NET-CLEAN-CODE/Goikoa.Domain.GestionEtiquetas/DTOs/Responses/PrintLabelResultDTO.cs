using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.GestionEtiquetas.DTOs.Responses
{
    public class PrintLabelResultDTO
    {
        public string message { get; set; }
        public BartenderResponse bartenderResponse { get; set; }
    }
}
