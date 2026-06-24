using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.ApiAdmin.DTOs.Requests
{
    public class UserRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public int? FkRoleId { get; set; }
    }
}
