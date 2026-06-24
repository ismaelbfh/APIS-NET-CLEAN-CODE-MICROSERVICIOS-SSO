using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.GestionEtiquetas.DTOs.Responses
{
    public class BartenderMessage
    {
        public string ActionName { get; set; }
        public int Level { get; set; }  // Nivel de severidad
        public string Text { get; set; }
    }
}
