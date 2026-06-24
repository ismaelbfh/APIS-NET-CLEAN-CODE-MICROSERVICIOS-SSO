using AutoMapper;
using Goikoa.CommonServices.Patterns;
using Goikoa.Domain.GestionEtiquetas.DAL.Context;
using Goikoa.Domain.GestionEtiquetas.DAL.Models;
using Goikoa.Domain.GestionEtiquetas.Entities;
using Goikoa.Domain.GestionEtiquetas.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

public class ServiceCamposWSBartender : Repository<CamposWSBartender>, IServiceCamposWSBartender
{
    private readonly BDGestionEtiquetasContext _dbContext;
    private readonly IMapper _mapper;

    public ServiceCamposWSBartender(BDGestionEtiquetasContext dbContext, IMapper mapper) : base(dbContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<string>> GetTodosCamposPaginadoAsync(int page, int pageSize)
    {
        var total = await _dbContext.CamposWSBartender.CountAsync();
        var items = await _dbContext.CamposWSBartender
            .OrderBy(c => c.NombreCampoBartender)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => c.NombreCampoBartender)
            .ToListAsync();

        return new PaginatedResult<string>
        {
            Items = items,
            TotalRows = total,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Obtiene todos los campos usados en plantillas y labels, pero que aún no han sido registrados en la tabla CamposWSBartender
    /// Se usa para saber qué campos faltan por registrar en el WS de Bartender
    /// </summary>
    /// <returns>Lista de nombres de campos que están en uso pero no registrados</returns>
    public async Task<List<string>> GetCamposFaltantesAsync()
    {
        // --------------------------
        // 1. Obtener los campos usados en las plantillas activas
        // --------------------------
        // Se hace join entre PlantillaCampos, Campo y Plantilla para asegurar que:
        // - El campo pertenece a una plantilla activa (no borrada)
        // - Se selecciona el NombreCampoBartender asociado al campo
        // Ejemplo: si la plantilla P1 usa el campo "CodigoProducto" y P1 no está eliminada → se incluye "CodigoProducto"

        var camposEnUso = await (
            from pc in _dbContext.PlantillaCampos
            join c in _dbContext.Campo on pc.FK_IdCampo equals c.PK_IdCampo
            join p in _dbContext.Plantilla on pc.FK_IdPlantilla equals p.PK_IdPlantilla
            where p.IsDeleted == null || p.IsDeleted == false
            select pc.NombreCampoBartender
        ).Distinct().ToListAsync();

        // --------------------------
        // 2. Obtener los labels compuestos usados en etiquetas activas
        // --------------------------
        // Se hace join entre:
        // - Plantilla → Etiqueta → EtiquetaLabels → TiposLabels
        // Para obtener el nombre del tipo de label + "Label" como campo usado
        // Ejemplo: si una etiqueta activa usa un tipo de label con Descripcion="Conservacion", se incluye "ConservacionLabel"

        var labelsEnUso = await (
            from p in _dbContext.Plantilla
            join e in _dbContext.Etiqueta on p.PK_IdPlantilla equals e.FK_IdPlantilla
            join el in _dbContext.EtiquetaLabels on e.PK_IdEtiqueta equals el.FK_IdEtiqueta
            join tl in _dbContext.TiposLabels on el.FK_IdTipoLabel equals tl.PK_IdTipoLabel
            where (p.IsDeleted == null || p.IsDeleted == false) &&
                  (e.IsDeleted == null || e.IsDeleted == false)
            select tl.Descripcion + "LABEL"
        ).Distinct().ToListAsync();

        // 3. Unir todos los nombres usados (campos y labels)
        var usados = camposEnUso.Union(labelsEnUso).ToList();

        // 4. Obtener los campos ya registrados en la tabla CamposWSBartender
        var yaRegistrados = await _dbContext.CamposWSBartender
            .Select(c => c.NombreCampoBartender)
            .ToListAsync();

        // 5. Devuelve los campos que están en uso (en plantillas o etiquetas activas)
        // pero que todavía no están registrados en la tabla CamposWSBartender.
        // Ejemplo: usados = ["CodigoProducto", "FechaCaducidad"], yaRegistrados = ["CodigoProducto"]
        // Resultado: ["FechaCaducidad"]
        return usados.Where(c => !yaRegistrados.Contains(c)).OrderBy(x => x).ToList();
    }

    /// <summary>
    /// Registra en la base de datos solo los campos que están en uso (y faltan) en el WS
    /// Verifica primero que realmente sean válidos para evitar duplicados
    /// </summary>
    /// <param name="nombres">Lista de nombres que el usuario ha seleccionado para registrar</param>
    /// <returns>Lista de los campos realmente insertados</returns>
    public async Task<List<string>> RegistrarCamposAsync(List<string> nombres)
    {
        // --------------------------
        // 1. Obtener los nombres faltantes reales desde base de datos
        // --------------------------
        var faltantes = await GetCamposFaltantesAsync();

        // --------------------------
        // 2. Validar que los que nos llegan desde el frontend están dentro de los faltantes
        // (el usuario podría haber enviado algo que ya está registrado)
        // --------------------------
        var aInsertar = nombres.Intersect(faltantes).Distinct().ToList();

        if (!aInsertar.Any())
            throw new Exception("No hay campos válidos para registrar.");

        // --------------------------
        // 3. Log de lo que vamos a insertar
        // --------------------------
        Log.Information("Datos insertados en el WS: " + string.Join(",", aInsertar));

        // --------------------------
        // 4. Insertar en la tabla CamposWSBartender con fecha y marcado como presente en el WS
        // --------------------------
        foreach (var nombre in aInsertar)
        {
            _dbContext.CamposWSBartender.Add(new CamposWSBartender
            {
                NombreCampoBartender = nombre,
                FechaRegistro = DateTime.Now,
                EstaEnWS = true
            });
        }

        // --------------------------
        // 5. Guardar en base de datos
        // --------------------------
        await _dbContext.SaveChangesAsync();
        return aInsertar;
    }

    /* ----------------------------------------
        EJEMPLOS:
        ------------------------------------------
        Plantilla P1:
        - Usa campos: CodigoProducto, FechaCaducidad
        - Estos campos tienen NombreCampoBartender: "CodigoProducto", "FechaCaducidad"

        Etiqueta E1:
        - Usa label tipo "OvaloCE" → se considera campo "OvaloCELabel"

        Tabla CamposWSBartender:
        - Ya registrados: "CodigoProducto"

        Resultado de GetCamposFaltantesAsync():
        - ["FechaCaducidad", "OvaloCELabel"]

        Si el usuario elige registrar solo "OvaloCELabel" → RegistrarCamposAsync insertará solo ese
        Si elige uno ya registrado ("CodigoProducto"), no se inserta duplicado
    */

}
