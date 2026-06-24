using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Goikoa.CommonServices.Services
{
    public class ServiceDB : IServiceDB
    {
        private readonly ILogger<ServiceDB> _logger;

        public ServiceDB(ILogger<ServiceDB> logger)
        {
            _logger = logger;
        }

        public void ResearchNewPK<T>(DbContext context, T entity) where T : class
        {
            try
            {
                var tableName = context.Model.FindEntityType(typeof(T))?.GetTableName();
                var pkProperty = typeof(T).GetProperties().FirstOrDefault(p => p.Name.StartsWith("Pk"));

                if (tableName == null || pkProperty == null)
                    throw new InvalidOperationException("No se puede determinar la PK o tabla.");

                // Suponiendo que el PK es int
                var maxId = context.Set<T>()
                                   .AsNoTracking()
                                   .Select(e => EF.Property<int>(e, pkProperty.Name))
                                   .DefaultIfEmpty()
                                   .Max();

                pkProperty.SetValue(entity, (short)(maxId + 1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular nuevo PK");
                throw;
            }
        }

        public bool TestConnection(DbContext context)
        {
            try
            {
                return context.Database.CanConnect();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al probar conexión con la base de datos.");
                return false;
            }
        }
    }
}
