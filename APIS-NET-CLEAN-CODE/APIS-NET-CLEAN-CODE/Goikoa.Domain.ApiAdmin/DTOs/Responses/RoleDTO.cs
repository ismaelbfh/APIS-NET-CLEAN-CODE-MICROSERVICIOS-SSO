using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.ApiAdmin.DTOs.Responses
{
    public class RoleDTO
    {
        public int PkIdRole { get; set; }

        public string NombreRol { get; set; }

        //public virtual ICollection<UserDTO> Usuarios { get; set; } = new List<UserDTO>();
    }
}
