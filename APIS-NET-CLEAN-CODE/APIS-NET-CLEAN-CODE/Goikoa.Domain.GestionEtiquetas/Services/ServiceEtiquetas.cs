using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
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
    public class ServiceEtiquetas : Repository<Etiqueta>, IServiceEtiqueta
    {
        private readonly BDGestionEtiquetasContext _dbContext;
        private readonly IMapper _mapper;
        public ServiceEtiquetas(BDGestionEtiquetasContext dbContext, IMapper mapper) : base(dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<EtiquetaDTO>> GetEtiquetasList(int pageNumber, int pageSize, string filtro, int? plantillaId)
        {
            IQueryable<Etiqueta> query = _dbContext.Etiqueta
                .Where(e => (bool)!e.IsDeleted)
                .Include(e => e.EtiquetaCamposValores)
                .Include(e => e.EtiquetaLabels)
                .Include(x => x.EtiquetaCodigosBarra)
                    .ThenInclude(x => x.FK_IdTipoCodigoBarraNavigation)
                .OrderByDescending(e => e.FechaModificacion)
                // INCLUYE los FK para poder filtrar por nombres (opcional, puedes incluir si lo necesitas)
                .Include(e => e.FK_IdPlantillaNavigation)
                .Include(e => e.FK_IdTipoEtiquetaNavigation)
                .Include(e => e.FK_IdConservacionNavigation)
                .Include(e => e.FK_IdSeccionNavigation)
                .Include(e => e.FK_IdClienteNavigation)
                .Include(e => e.FK_IdEnvasadoNavigation)
                .Include(e => e.FK_IdUbicacionNavigation);

            if (plantillaId != null && plantillaId != 0)
                query = query.Where(e => e.FK_IdPlantilla == plantillaId.Value);

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                string texto = filtro.ToLower().Trim();

                // Manejo de fechas
                DateTime? fechaBuscada = null;
                if (DateTime.TryParseExact(filtro, new[] { "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy" },
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var dt))
                {
                    fechaBuscada = dt.Date;
                }

                query = query.Where(e =>
                    (e.NombreEtiqueta != null && e.NombreEtiqueta.ToLower() == (texto)) ||
                    (e.CodigoProducto != null && e.CodigoProducto.ToLower() == (texto)) ||
                    (e.Comentario != null && e.Comentario.ToLower().Contains(texto)) ||
                    (e.OvaloIdioma != null && e.OvaloIdioma.ToLower() == (texto)) ||
                    (e.OvaloPlanta != null && e.OvaloPlanta.ToLower() == (texto)) ||
                    (e.OvaloCE != null && e.OvaloCE.ToLower() == (texto)) ||

                    // Filtros por nombre de FK (solo si incluyes los .Include arriba)
                    (e.FK_IdPlantillaNavigation != null && e.FK_IdPlantillaNavigation.NombrePlantilla != null && e.FK_IdPlantillaNavigation.NombrePlantilla.ToLower() == (texto)) ||
                    (e.FK_IdTipoEtiquetaNavigation != null && e.FK_IdTipoEtiquetaNavigation.Descripcion != null && e.FK_IdTipoEtiquetaNavigation.Descripcion.ToLower() == (texto)) ||
                    (e.FK_IdConservacionNavigation != null && e.FK_IdConservacionNavigation.Descripcion != null && e.FK_IdConservacionNavigation.Descripcion.ToLower() == (texto)) ||
                    (e.FK_IdSeccionNavigation != null && e.FK_IdSeccionNavigation.Descripcion != null && e.FK_IdSeccionNavigation.Descripcion.ToLower() == (texto)) ||
                    (e.FK_IdClienteNavigation != null && e.FK_IdClienteNavigation.Descripcion != null && e.FK_IdClienteNavigation.Descripcion.ToLower() == (texto)) ||
                    (e.FK_IdEnvasadoNavigation != null && e.FK_IdEnvasadoNavigation.Descripcion != null && e.FK_IdEnvasadoNavigation.Descripcion.ToLower() == (texto)) ||
                    (e.FK_IdUbicacionNavigation != null && e.FK_IdUbicacionNavigation.Descripcion != null && e.FK_IdUbicacionNavigation.Descripcion.ToLower() == (texto)) ||


                    // Fechas, compara solo el día
                    (fechaBuscada.HasValue && e.FechaGeneracion.HasValue && e.FechaGeneracion.Value.Date == fechaBuscada.Value) ||
                    (fechaBuscada.HasValue && e.FechaModificacion.HasValue && e.FechaModificacion.Value.Date == fechaBuscada.Value)
                );
            }

            var total = await query.CountAsync();

            var entidades = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<List<EtiquetaDTO>>(entidades);

            return new PaginatedResult<EtiquetaDTO>
            {
                Items = dtos,
                TotalRows = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Endpoint para uso en APP WPF DE PLANTA PARA TRAER TODOS LOS DATOS DE LA ETIQUETA EN BASE A UN CODIGO DE PRODUCTO Y PODER IMPRIMIR ETIQUETAS
        /// </summary>
        /// <param name="pCodigoProducto"></param>
        /// <returns>Etiqueta entera con todas sus posibles relaciones para hacer funcionar la app wpf de planta de impresion</returns>
        public async Task<Etiqueta> GetEtiquetaPorCodigoProductoAsync(string pCodigoProducto)
        {
            var etiqueta = await _dbContext.Etiqueta
                .Include(e => e.EtiquetaCamposValores)
                .Include(e => e.FK_IdPlantillaNavigation)
                    .ThenInclude(p => p.PlantillaCampos)
                        .ThenInclude(o => o.FK_IdCampoNavigation)
                            .ThenInclude(l => l.FK_IdTipoCampoNavigation)
                .Include(x => x.EtiquetaCodigosBarra)
                    .ThenInclude(x => x.FK_IdTipoCodigoBarraNavigation)
                .Include(e => e.FK_IdPlantillaNavigation)
                                .ThenInclude(p => p.PlantillaCampos)
                                    .ThenInclude(o => o.FK_IdCampoNavigation)
                                        .ThenInclude(o => o.FK_IdCampoNavisionNavigation)
                .Include(e => e.FK_IdConservacionNavigation)
                .Include(e => e.EtiquetaLabels)
                .ThenInclude(p => p.FK_IdLabelNavigation)
                .ThenInclude(p => p.FK_IdTipoLabelNavigation)
                .Include(e => e.FK_IdClienteNavigation)
                .Include(e => e.FK_IdEnvasadoNavigation)
                .Include(e => e.FK_IdSeccionNavigation)
                .Include(e => e.FK_IdTipoEtiquetaNavigation)
                .Include(e => e.FK_IdUbicacionNavigation)
                .FirstOrDefaultAsync(e => e.CodigoProducto == pCodigoProducto && (!e.IsDeleted.HasValue || e.IsDeleted == false));

            if (etiqueta == null)
                throw new KeyNotFoundException("No se encontró ninguna etiqueta activa para ese código de producto.");

            return etiqueta;
        }

        public async Task AddEtiquetaAsync(EtiquetaDTO etiquetaDto)
        {
            PrepareAndReOrderEtiquetasCodigoBarras(etiquetaDto);

            var etiquetaEntity = _mapper.Map<Etiqueta>(etiquetaDto);
            etiquetaEntity.IsDeleted = false;
            etiquetaEntity.Version = 1;
            etiquetaEntity.FechaGeneracion = DateTime.Now;
            etiquetaEntity.FechaModificacion = DateTime.Now;


            if (etiquetaEntity.EtiquetaCamposValores != null)
            {
                foreach (var campo in etiquetaEntity.EtiquetaCamposValores)
                {
                    campo.PK_IdEtiquetaCampoValor = 0;
                    // Se puede asignar la FK más adelante o dejar que EF la establezca
                }
            }

            if (etiquetaEntity.EtiquetaCodigosBarra != null)
            {
                foreach (var codigoBarra in etiquetaEntity.EtiquetaCodigosBarra)
                {
                    codigoBarra.PK_IdEtiquetaCodigoBarra = 0;
                    codigoBarra.FK_IdEtiqueta = 0;
                }
            }

            await _dbContext.Etiqueta.AddAsync(etiquetaEntity);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                var inner = ex.InnerException?.Message;
                Log.Error("Error al guardar los cambios en la base de datos para añadir una etiqueta: {inner} , excepcion completa: {err}", inner, ex);
                throw new Exception($"Error al guardar los cambios en la base de datos para añadir una etiqueta: {inner}", ex);
            }
            Log.Information("Se ha creado una etiqueta nueva con éxito.");
        }

        public async Task UpdateEtiquetaAsync(EtiquetaDTO etiquetaDto)
        {
            // Buscamos la etiqueta actual activa (IsDeleted = false)
            var etiquetaEntity = await _dbContext.Etiqueta
                .Include(e => e.EtiquetaCamposValores)
                .Include(e => e.EtiquetaCodigosBarra)
                .FirstOrDefaultAsync(e => e.PK_IdEtiqueta == etiquetaDto.PK_IdEtiqueta && (bool)!e.IsDeleted);

            if (etiquetaEntity == null)
            {
                Log.Error("Error al editar una etiqueta porque el id {IdEtiquetaReal} no existe.", etiquetaDto.PK_IdEtiqueta);
                throw new KeyNotFoundException($"No se encontró la etiqueta con id {etiquetaDto.PK_IdEtiqueta}.");
            }

            // Marcar la etiqueta actual como borrada lógicamente.
            etiquetaEntity.IsDeleted = true;

            await DeleteEtiquetasLabelsAndRReOrder(etiquetaEntity, etiquetaDto);
            PrepareAndReOrderEtiquetasCodigoBarras(etiquetaDto);

            // Crear una nueva entidad mapeada desde el DTO.
            var nuevaEtiqueta = _mapper.Map<Etiqueta>(etiquetaDto);
            nuevaEtiqueta.PK_IdEtiqueta = 0; //nuevo
            nuevaEtiqueta.Version = etiquetaEntity.Version + 1;
            nuevaEtiqueta.IsDeleted = false;
            nuevaEtiqueta.FechaModificacion = DateTime.Now;


            // Para la colección de EtiquetaCamposValores, se debe resetear la clave primaria de cada hijo
            // para evitar conflictos de rastreo (tracking)
            if (nuevaEtiqueta.EtiquetaCamposValores != null)
            {
                foreach (var campo in nuevaEtiqueta.EtiquetaCamposValores)
                {
                    campo.PK_IdEtiquetaCampoValor = 0;
                    // Se puede asignar la FK más adelante o dejar que EF la establezca

                }
            }

            if (nuevaEtiqueta.EtiquetaCodigosBarra != null)
            {
                foreach (var codigoBarra in nuevaEtiqueta.EtiquetaCodigosBarra)
                {
                    codigoBarra.PK_IdEtiquetaCodigoBarra = 0;
                    codigoBarra.FK_IdEtiqueta = 0;
                }
            }

            await _dbContext.Etiqueta.AddAsync(nuevaEtiqueta);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                var inner = ex.InnerException?.Message;
                Log.Error("Error al guardar los cambios en la base de datos para editar una etiqueta: {inner} , excepcion completa: {err}", inner, ex);
                throw new Exception($"Error al guardar los cambios en la base de datos para editar una etiqueta: {inner}", ex);
            }
            Log.Information("Actualizacion realizada con éxito para la etiqueta con id {IdEtiquetaReal}", etiquetaEntity.PK_IdEtiqueta);
        }

        public async Task<Etiqueta> DeleteEtiquetaAsync(int id)
        {
            // Borrado lógico: buscamos la etiqueta activa y marcamos IsDeleted=true
            var etiquetaEntity = await _dbContext.Etiqueta
                .FirstOrDefaultAsync(e => e.PK_IdEtiqueta == id && (bool)!e.IsDeleted);
            if (etiquetaEntity == null)
            {
                Log.Error("Error al borrar una etiqueta porque el id {IdEtiquetaReal} no existe.", id);
                throw new KeyNotFoundException($"No se encontró la etiqueta con id {id}.");
            }

            // Eliminar relaciones con EtiquetaLabels (si las hay)
            var etiquetasLabelsRelacionadas = await _dbContext.EtiquetaLabels
                .Where(el => el.FK_IdEtiqueta == id)
                .ToListAsync();

            _dbContext.EtiquetaLabels.RemoveRange(etiquetasLabelsRelacionadas);

            etiquetaEntity.IsDeleted = true;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                var inner = ex.InnerException?.Message;
                Log.Error("Error al guardar los cambios en la base de datos para borrar una etiqueta: {inner} , excepcion completa: {err}", inner, ex);
                throw new Exception($"Error al guardar los cambios en la base de datos para borrar una etiqueta: {inner}", ex);
            }
            Log.Information("Se ha eliminado la etiqueta con id {IdEtiquetaReal} con éxito.", id);
            return etiquetaEntity;
        }

        /// <summary>
        /// - No toca nada si el usuario no ha cambiado nada.
        /// - Respeta el orden que venga desde el DTO.
        /// - Solo elimina relaciones que realmente se quitaron.
        /// - Recalcula Orden solo si es necesario.
        /// Este método se ejecuta solo en el update de etiquetas.
        /// </summary>
        /// <param name="etiquetaEntity">Entidad de la etiqueta original (la que va a marcarse como IsDeleted = true)</param>
        /// <param name="etiquetaDto">DTO recibido del frontend con la nueva versión de la etiqueta</param>
        private async Task DeleteEtiquetasLabelsAndRReOrder(Etiqueta etiquetaEntity, EtiquetaDTO etiquetaDto)
        {
            // Paso 1: Cargar todas las relaciones anteriores (labels asociados a la etiqueta antigua)
            // Ejemplo: si la etiqueta 15 tenía 3 labels asociadas, aquí cargamos esas 3
            var etiquetasLabelsAnteriores = await _dbContext.EtiquetaLabels
                .Where(el => el.FK_IdEtiqueta == etiquetaEntity.PK_IdEtiqueta)
                .ToListAsync();

            // Paso 2: Obtener desde el DTO las nuevas relaciones que el usuario ha dejado tras editar
            // Ejemplo: si el usuario eliminó una label, aquí solo habrá 2 (las actuales)
            var nuevosEtiquetaLabels = etiquetaDto.EtiquetaLabels ?? new List<EtiquetaLabelsDTO>();

            // Paso 3: Detectar las relaciones que hay que eliminar
            // Aquí se eliminan aquellas combinaciones tipo+idioma+label que ya no existen en el DTO
            // Es decir, las que estaban en la versión anterior y no están en la nueva
            // Ejemplo:
            //  - Antes: [Tipo=1, Idioma=1, Label=5], [Tipo=1, Idioma=1, Label=6]
            //  - Ahora: [Tipo=1, Idioma=1, Label=6] → se eliminará la relación con Label=5
            var aEliminar = etiquetasLabelsAnteriores
                .Where(elViejo =>
                    !nuevosEtiquetaLabels.Any(nuevo =>
                        nuevo.FK_IdTipoLabel == elViejo.FK_IdTipoLabel &&
                        nuevo.FK_IdIdioma == elViejo.FK_IdIdioma &&
                        nuevo.FK_IdLabel == elViejo.FK_IdLabel))
                .ToList();

            // Paso 4: Eliminamos solo las relaciones detectadas como removidas
            _dbContext.EtiquetaLabels.RemoveRange(aEliminar);

            // Paso 5: Actualizar orden solo en los casos donde no se ha eliminado pero sí se ha reordenado
            // Ejemplo:
            //  - Antes: Label1 (Orden 1), Label2 (Orden 2), Label3 (Orden 3)
            //  - Ahora: Label1 (Orden 1), Label3 (Orden 2) → se eliminó Label2 y Label3 debe actualizar su orden
            // Este bucle actualiza solo los elementos que aún existen pero con diferente orden
            foreach (var elViejo in etiquetasLabelsAnteriores)
            {
                // Buscamos si esta relación antigua aún existe en el nuevo DTO
                var correspondienteEnNuevo = nuevosEtiquetaLabels.FirstOrDefault(nuevo =>
                    nuevo.FK_IdTipoLabel == elViejo.FK_IdTipoLabel &&
                    nuevo.FK_IdIdioma == elViejo.FK_IdIdioma &&
                    nuevo.FK_IdLabel == elViejo.FK_IdLabel);

                // Si sigue existiendo pero su orden ha cambiado, lo actualizamos
                // Ejemplo: antes Label3 era Orden 3, ahora en el nuevo DTO viene como Orden 2 → lo actualizamos
                // También cubre casos como: antes había 3 labels y se eliminó la del medio → los siguientes deben reordenarse
                if (correspondienteEnNuevo != null && elViejo.Orden != correspondienteEnNuevo.Orden)
                {
                    elViejo.Orden = correspondienteEnNuevo.Orden;
                }
            }
        }

        // -----------------------------------------------
        // ¿Por qué se llama este método justo después de marcar IsDeleted = true?
        // -----------------------------------------------
        // En UpdateEtiquetaAsync, primero se marca la entidad anterior como borrada (IsDeleted = true),
        // y solo entonces se llama a DeleteEtiquetasLabelsAndRReOrder(). Esto es correcto porque:
        // - Este método opera sobre la versión anterior de la etiqueta (la que se está marcando como borrada)
        // - Necesitamos su ID (PK_IdEtiqueta) para saber qué EtiquetaLabels eliminar
        // - Si lo llamáramos antes, no sabríamos todavía que esa versión está quedando obsoleta
        //
        // Luego se crea una nueva entidad `nuevaEtiqueta` con los datos actualizados y se guarda como nueva versión.
        //
        // Resultado:
        // - La etiqueta anterior queda marcada como eliminada
        // - Sus relaciones con labels se limpian solo si realmente cambiaron
        // - La nueva versión se guarda con nuevos EtiquetaLabels (si es que los hay en el DTO)
        //
        // Esto garantiza que el histórico queda intacto y solo se modifican relaciones relevantes.

        /// <summary>
        /// Limpia y reordena la colección de códigos de barras recibida desde frontend.
        /// - Elimina filas inválidas o vacías.
        /// - Respeta el orden visual que venga desde el DTO.
        /// - Reasigna el orden secuencialmente desde 1.
        /// Este método se ejecuta tanto en alta como en edición antes de crear la entidad final.
        /// </summary>
        /// <param name="etiquetaDto">DTO recibido del frontend con la etiqueta y sus códigos de barras.</param>
        private void PrepareAndReOrderEtiquetasCodigoBarras(EtiquetaDTO etiquetaDto)
        {
            if (etiquetaDto == null)
                return;

            var codigosActuales = etiquetaDto.EtiquetaCodigosBarra ?? new List<EtiquetaCodigoBarraDTO>();

            // Paso 1: eliminar filas inválidas o vacías
            var codigosValidos = codigosActuales
                .Where(x => x != null && x.FK_IdTipoCodigoBarra > 0)
                .OrderBy(x => x.Orden)
                .ToList();

            // Paso 2: reordenar secuencialmente
            for (int i = 0; i < codigosValidos.Count; i++)
            {
                codigosValidos[i].Orden = i + 1;
            }

            // Paso 3: sustituir la colección original por la colección limpia y reordenada
            etiquetaDto.EtiquetaCodigosBarra = codigosValidos;
        }
    }
}