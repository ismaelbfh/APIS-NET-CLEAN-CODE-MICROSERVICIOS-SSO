using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.Navision.Producccion.Entities
{
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalRows { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
