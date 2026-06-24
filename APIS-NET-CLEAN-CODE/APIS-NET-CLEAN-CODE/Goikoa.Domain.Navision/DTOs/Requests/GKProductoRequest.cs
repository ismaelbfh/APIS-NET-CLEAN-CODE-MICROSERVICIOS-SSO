using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.Navision.Producccion.DTOs.Requests
{
    public class GKProductoRequest
    {
        public string pNumProduct { get; set; }
        public string pTipoCodigoBarras { get; set; }
    }
}
