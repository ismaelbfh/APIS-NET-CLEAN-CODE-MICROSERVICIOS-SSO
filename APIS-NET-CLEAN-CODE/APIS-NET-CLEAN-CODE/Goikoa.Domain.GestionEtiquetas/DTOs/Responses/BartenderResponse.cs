using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.GestionEtiquetas.DTOs.Responses
{
    public class BartenderResponse
    {
        public string Version { get; set; }
        public string Status { get; set; }
        public string WaitStatus { get; set; }
        public bool Validated { get; set; }
        public List<BartenderMessage> Messages { get; set; }
    }
}
