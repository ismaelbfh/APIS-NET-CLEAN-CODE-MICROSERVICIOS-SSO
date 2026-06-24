using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Goikoa.CommonServices.Services
{
    public interface IServiceDB
    {
        void ResearchNewPK<T>(DbContext context, T entity) where T : class;
        bool TestConnection(DbContext context);
    }
}
