using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Goikoa.CommonServices.Patterns;
using Goikoa.Domain.GestionEtiquetas.DAL.Context;
using Goikoa.Domain.GestionEtiquetas.DAL.Models;
using Goikoa.Domain.GestionEtiquetas.DTOs.Responses;
using Goikoa.Domain.GestionEtiquetas.Entities;
using Goikoa.Domain.GestionEtiquetas.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Goikoa.Domain.GestionEtiquetas.Services
{
    public class ServicePlantilla : Repository<Plantilla>, IServicePlantilla
    {
        private readonly BDGestionEtiquetasContext _dbContext;
        private readonly IMapper _mapper;

        public ServicePlantilla(BDGestionEtiquetasContext dbContext, IMapper mapper) : base(dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        
        public async Task<PaginatedResult<PlantillaDTO>> GetPlantillasList(int pageNumber, int pageSize, string filtro)
        {
            var query = _dbContext.Plantilla
                .Where(p => !p.IsDeleted.HasValue || p.IsDeleted == false)
                .Include(p => p.PlantillaCampos)
                .OrderByDescending(p => p.FechaModificacion)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                string texto = filtro.ToLower().Trim();

                // Intenta parsear como fecha
                DateTime? fechaBuscada = null;
                if (DateTime.TryParseExact(filtro,
                    new[] { "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy" },
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var dt))
                {
                    fechaBuscada = dt.Date;
                }

                query = query.Where(p =>
                    // Nombre de plantilla y ruta por contains
                    (p.NombrePlantilla != null && p.NombrePlantilla.ToLower() == texto) ||
                    (p.RutaArchivo != null && p.RutaArchivo.ToLower() == texto) ||

                    // Fechas: busca por fecha exacta (solo parte de la fecha)
                    (fechaBuscada.HasValue && (
                        (p.FechaCreacion.HasValue && p.FechaCreacion.Value.Date == fechaBuscada.Value) ||
                        (p.FechaModificacion.HasValue && p.FechaModificacion.Value.Date == fechaBuscada.Value)
                    ))
                );
            }

            var total = await query.CountAsync();

            var entidades = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<List<PlantillaDTO>>(entidades);

            return new PaginatedResult<PlantillaDTO>
            {
                Items = dtos,
                TotalRows = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task AddPlantillaAsync(PlantillaDTO plantillaDto)
        {
            var plantillaEntity = _mapper.Map<Plantilla>(plantillaDto);
            plantillaEntity.IsDeleted = false;
            plantillaEntity.FechaCreacion = DateTime.Now;
            plantillaEntity.FechaModificacion = DateTime.Now;
            await _dbContext.Plantilla.AddAsync(plantillaEntity);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                var inner = ex.InnerException?.Message;
                Log.Error("Error al guardar los cambios en la base de datos para añadir una plantilla: {inner} , excepcion completa: {err}", inner, ex);
                throw new Exception($"Error al guardar los cambios en la base de datos para añadir una plantilla: {inner}", ex);
            }
            Log.Information("Se ha creado una plantilla nueva con éxito.");
        }

        /// <summary>
        /// Actualiza una plantilla existente junto con sus campos (PlantillaCampos) y sincroniza las etiquetas activas relacionadas.
        /// Elimina valores antiguos que ya no están en la plantilla y añade valores nuevos si faltan.
        /// </summary>
        /// <param name="plantillaDto">DTO recibido desde el frontend con los datos actualizados de la plantilla</param>
        public async Task UpdatePlantillaAsync(PlantillaDTO plantillaDto)
        {
            // Buscar la plantilla con su lista de campos (PlantillaCampos)
            // Ejemplo: Si plantillaDto.PK_IdPlantilla = 3, se cargará la plantilla con ID 3 incluyendo sus campos actuales
            var plantillaEntity = await _dbContext.Plantilla
                .Include(p => p.PlantillaCampos)
                .FirstOrDefaultAsync(p => p.PK_IdPlantilla == plantillaDto.PK_IdPlantilla);

            if (plantillaEntity == null)
            {
                // Si no se encuentra, se lanza una excepción y se loguea
                Log.Error("Error al editar una plantilla porque el id {IdPlantillaReal} no existe.", plantillaDto.PK_IdPlantilla);
                throw new KeyNotFoundException($"No se encontró la plantilla con id {plantillaDto.PK_IdPlantilla}.");
            }

            // Actualizamos la fecha de modificación
            plantillaEntity.FechaModificacion = DateTime.Now;

            // Obtenemos la lista de IDs de campos que tenía la plantilla antes de la edición
            var idsCamposAnteriores = plantillaEntity.PlantillaCampos.Select(pc => (int)pc.FK_IdCampo).ToList();

            // Obtenemos la lista de IDs de campos que el usuario ha dejado tras editar (campos nuevos actuales)
            var idsCamposNuevos = plantillaDto.PlantillaCampos?.Select(pc => pc.FK_IdCampo).ToList() ?? new List<int>();

            // Calculamos los campos eliminados haciendo diferencia entre los anteriores y los nuevos
            // Ejemplo: Si antes tenía [1,2,3] y ahora tiene [2,3], el eliminado es 1
            var idsCamposEliminados = idsCamposAnteriores.Except(idsCamposNuevos).ToList();

            // Guardamos los campos nuevos tal cual para usarlos en otro paso
            var idsCamposActuales = idsCamposNuevos;

            // ----------------------------------------------------------------
            // Paso 1: Eliminar valores (EtiquetaCamposValores) que correspondan a campos eliminados en etiquetas activas
            // ----------------------------------------------------------------
            if (idsCamposEliminados.Any())
            {
                // Traemos las etiquetas activas asociadas a la plantilla (IsDeleted = false o null)
                // Ejemplo: Si plantilla P1 tiene 4 etiquetas, y 2 están activas, traerá esas 2
                var etiquetasActivas = await _dbContext.Etiqueta
                    .Where(e => e.FK_IdPlantilla == plantillaDto.PK_IdPlantilla && (!e.IsDeleted.HasValue || e.IsDeleted == false))
                    .Include(e => e.EtiquetaCamposValores) // Incluir también los valores actuales para poder eliminarlos
                    .ToListAsync();

                foreach (var etiqueta in etiquetasActivas)
                {
                    // Buscamos qué valores corresponden a campos eliminados (por ID), en este caso era el 1 por lo que borrará el 1 de esas etiquetas que vamos recorriendo
                    var valoresAEliminar = etiqueta.EtiquetaCamposValores
                        .Where(ecv => ecv.FK_IdCampo.HasValue && idsCamposEliminados.Contains(ecv.FK_IdCampo.Value))
                        .ToList();

                    // Eliminamos esos valores de la base de datos, por cada etiqueta ya que ese campo no existirá ya al borrarlo de la plantilla
                    _dbContext.EtiquetaCamposValores.RemoveRange(valoresAEliminar);
                }
            }

            // ----------------------------------------------------------------
            // Paso 2: Sustituir completamente los PlantillaCampos actuales por los nuevos del DTO
            // ----------------------------------------------------------------
            plantillaEntity.PlantillaCampos.Clear(); // Limpiamos los anteriores

            if (plantillaDto.PlantillaCampos != null)
            {
                foreach (var campoDto in plantillaDto.PlantillaCampos)
                {
                    // Convertimos el DTO a entidad (usando AutoMapper)
                    var nuevoCampo = _mapper.Map<PlantillaCampos>(campoDto);
                    nuevoCampo.FK_IdPlantilla = plantillaEntity.PK_IdPlantilla; // Aseguramos que esté bien enlazado
                    plantillaEntity.PlantillaCampos.Add(nuevoCampo); // Añadimos a la colección
                }
            }

            // ----------------------------------------------------------------
            // Paso 3: Asegurar que todas las etiquetas activas tengan todos los campos actuales
            // (si falta algún campo, se añade un EtiquetaCamposValores vacío)
            // ----------------------------------------------------------------
            if (idsCamposActuales.Any())
            {
                // Traemos nuevamente las etiquetas activas de la plantilla
                var etiquetasActivas = await _dbContext.Etiqueta
                    .Where(e => e.FK_IdPlantilla == plantillaDto.PK_IdPlantilla && (!e.IsDeleted.HasValue || e.IsDeleted == false))
                    .Include(e => e.EtiquetaCamposValores)
                    .ToListAsync();

                foreach (var etiqueta in etiquetasActivas)
                {
                    foreach (var idCampoActual in idsCamposActuales)
                    {
                        // Verificamos si ese campo ya existe en la etiqueta
                        bool yaExiste = etiqueta.EtiquetaCamposValores.Any(ecv => ecv.FK_IdCampo == idCampoActual);
                        if (!yaExiste)
                        {
                            // Si no existe, lo creamos con valor null (para ser rellenado luego si es necesario)
                            etiqueta.EtiquetaCamposValores.Add(new EtiquetaCamposValores
                            {
                                FK_IdCampo = idCampoActual,
                                ValorCampo = null,
                                FK_IdEtiqueta = etiqueta.PK_IdEtiqueta
                            });
                        }
                    }
                }
            }

            // ----------------------------------------------------------------
            // Paso 4: Actualizar propiedades simples como el nombre y ruta
            // ----------------------------------------------------------------
            plantillaEntity.NombrePlantilla = plantillaDto.NombrePlantilla?.Trim();
            plantillaEntity.RutaArchivo = plantillaDto.RutaArchivo?.Trim();
            plantillaEntity.UsuarioModificacion = plantillaDto.UsuarioModificacion?.Trim();

            // ----------------------------------------------------------------
            // Paso 5: Guardar todos los cambios en base de datos, con gestión de errores
            // ----------------------------------------------------------------
            try
            {
                await _dbContext.SaveChangesAsync();
                Log.Information("Actualización realizada con éxito para la plantilla con id {IdPlantillaReal}", plantillaEntity.PK_IdPlantilla);
            }
            catch (DbException ex)
            {
                var inner = ex.InnerException?.Message;
                Log.Error("Error al guardar los cambios en la base de datos para editar una plantilla: {inner} , excepcion completa: {err}", inner, ex);
                throw new Exception($"Error al guardar los cambios en la base de datos para editar una plantilla: {inner}", ex);
            }
        }


        public async Task<Plantilla> DeletePlantillaAsync(int id)
        {
            var plantillaEntity = await _dbContext.Plantilla
                .Include(p => p.PlantillaCampos)
                .FirstOrDefaultAsync(p => p.PK_IdPlantilla == id);

            if (plantillaEntity == null)
            {
                Log.Error("Error al borrar una plantilla porque el id {IdPlantillaReal} no existe.", id);
                throw new KeyNotFoundException($"No se encontró la etiqueta con id {id}.");
            }

            // Borrado lógico de la plantilla
            plantillaEntity.IsDeleted = true;

            // Borrado lógico de sus etiquetas asociadas
            var etiquetas = await _dbContext.Etiqueta
                .Where(e => e.FK_IdPlantilla == id && (!e.IsDeleted.HasValue || e.IsDeleted == false))
                .ToListAsync();

            foreach (var etiqueta in etiquetas)
            {
                etiqueta.IsDeleted = true;
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                var inner = ex.InnerException?.Message;
                Log.Error("Error al guardar los cambios en la base de datos para borrar una plantilla: {inner} , excepcion completa: {err}", inner, ex);
                throw new Exception($"Error al guardar los cambios en la base de datos para borrar una plantilla: {inner}", ex);
            }
            Log.Information("Se ha eliminado la plantilla con id {IdPlantillaReal} con éxito.", id);
            return plantillaEntity;
        }
    }
}
