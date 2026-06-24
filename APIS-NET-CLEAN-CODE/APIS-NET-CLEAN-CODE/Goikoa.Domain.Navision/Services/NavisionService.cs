using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Goikoa.Domain.Navision.Producccion.DAL.Context;
using Goikoa.Domain.Navision.Producccion.DAL.Models;
using Goikoa.Domain.Navision.Producccion.DTOs.Responses;
using Goikoa.Domain.Navision.Producccion.Entities;
using Goikoa.Domain.Navision.Producccion.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Goikoa.Domain.Navision.Producccion.Services
{
    public class NavisionService : INavisionService
    {
        private readonly NAVBCContext _context;
        private readonly IMapper _mapper;

        public NavisionService(NAVBCContext pContext, IMapper pMapper)
        {
            _context = pContext;
            _mapper = pMapper;
        }

        public async Task<GKInfoOrdenCamaraDTO> GetOrdenFabricacion(string pOPFabricacion)
        {
            GoikoaOrdenProducciónFabricación? lOrdenMap = await _context.GoikoaOrdenProducciónFabricacións
                .FirstOrDefaultAsync(x => x.No.Equals(pOPFabricacion));
            if (lOrdenMap == null)
            {
                Log.Error("No se encuentra la orden {OrdenFab}", pOPFabricacion);
                throw new KeyNotFoundException($"No se encuentra la orden '{pOPFabricacion}'");
            }
            Log.Information("OK, se ha encontrado la orden {OrdenFab} en el endpoint {Endpoint}", lOrdenMap.No, "GetDatosOrdenProduccion");
            return _mapper.Map<GKInfoOrdenCamaraDTO>(lOrdenMap);
        }

        public async Task<GKProductoDTO?> GetCodigoBarrasByProductAndType(string pNumProduct, string pTipoCodigoBarras)
        {
            GoikoaItemCrossReference? lProductMap = await _context.GoikoaItemCrossReferences
                .FirstOrDefaultAsync(x => x.ItemNo.Equals(pNumProduct) && x.CrossReferenceTypeNo.Equals(pTipoCodigoBarras));
            if (lProductMap == null)
            {
                Log.Error("No se encuentra el codigo de barras para el producto {ProductoNum}", pNumProduct);
                throw new KeyNotFoundException($"No se encuentra el codigo de barras del producto '{pNumProduct}' con el tipo '{pTipoCodigoBarras}'");
            }
            Log.Information("OK, se ha encontrado el codigo de barras {CodBar} en el endpoint {Endpoint}", lProductMap.CrossReferenceNo, "GetCodificacionCodigoBarras");

            return _mapper.Map<GKProductoDTO?>(lProductMap);
        }

        public async Task<string> GetCodigoQrByProduct(string pNumProduct, string pQrType)
        {
            int crossType = pQrType switch
            {
                "QR_Arriba" => 5,
                "QR_Abajo" => 6,
                _ => throw new ArgumentException($"Tipo QR inválido: {pQrType}")
            };

            var row = await _context.GoikoaItemCrossReferences
                .FirstOrDefaultAsync(x => x.ItemNo == pNumProduct && x.CrossReferenceType == crossType);

            return row?.CrossReferenceNo ?? string.Empty;
        }

        public async Task<PaginatedResult<GKPrevisionDTO>> GetPrevisionesPorFechaAsync(DateTime pFecha, int pPageNumber, int pPageSize, string? filtro)
        {
            var fecha = pFecha.Date;

            // Nivel 1 - Base (SIN JOIN a Escandallo, usamos EXISTS con Any)
            var baseQuery =
                from linea in _context.GoikoaLineaPrevisionFabricacions
                join item in _context.GoikoaItems
                    on linea.CodProducto equals item.No
                join unitKG in _context.GoikoaItemUnitOfMeasures
                    on new { ItemNo = linea.CodProducto, Code = "KG" }
                    equals new { ItemNo = unitKG.ItemNo, unitKG.Code }
                where linea.Fecha.Date == fecha
                      && (linea.TipoDestino == 2 || linea.TipoDestino == 5)
                      // EXISTS: solo mantengo la línea si tiene al menos un escandallo con ProductoPrincipal = 1
                      && _context.GoikoaEscandalloFabricacións.Any(esca =>
                             esca.NoOpFabric == linea.CodProducto &&
                             esca.ProductoPrincipal == 1)
                select new
                {
                    linea.NºPrevision,
                    OP = linea.NºOrdenProduccion,
                    Maquina = linea.Linea,
                    Fecha = linea.Fecha,
                    PF = linea.CodProducto,
                    Descripcion = item.Description,
                    FechaCaducidad = linea.FechaCaducidad,
                    Lote = linea.Lote,
                    Cajas = (int)linea.CantidadEmbalaje,
                    Kilos = Math.Round((double)unitKG.QtyPerUnitOfMeasure, 3)
                };

            // Nivel 2 - EAN
            var base2 =
                from b in baseQuery
                join cr in _context.GoikoaItemCrossReferences
                    on b.PF equals cr.ItemNo
                where cr.CrossReferenceType == 3
                      && cr.CrossReferenceTypeNo == "EAN13"
                select new
                {
                    b.NºPrevision,
                    b.OP,
                    b.Maquina,
                    b.Fecha,
                    b.PF,
                    b.Descripcion,
                    b.FechaCaducidad,
                    b.Lote,
                    b.Cajas,
                    b.Kilos,
                    EAN = cr.CrossReferenceNo
                };

            // Nivel 3 - DUN
            var base3 =
                from b in base2
                join cr in _context.GoikoaItemCrossReferences
                    on b.PF equals cr.ItemNo
                where cr.CrossReferenceType == 4
                select new
                {
                    b.NºPrevision,
                    b.OP,
                    b.Maquina,
                    b.Fecha,
                    b.PF,
                    b.Descripcion,
                    b.FechaCaducidad,
                    b.Lote,
                    b.Cajas,
                    b.Kilos,
                    b.EAN,
                    DUN = cr.CrossReferenceNo
                };

            // Nivel Final - CAJA
            var finalQuery =
                from b in base3
                join unit in _context.GoikoaItemUnitOfMeasures
                    on new { ItemNo = b.PF, Code = "CAJA" }
                    equals new { ItemNo = unit.ItemNo, unit.Code }
                select new GKPrevisionDTO
                {
                    NUMEROPREVISION = b.NºPrevision,
                    OP = b.OP,
                    MAQUINA = b.Maquina,
                    FECHA = b.Fecha,
                    PF = b.PF,
                    DESCRIPCION = b.Descripcion,
                    FECHACADUCIDAD = b.FechaCaducidad.Date,
                    LOTE = b.Lote,
                    CAJAS = b.Cajas,
                    KILOS = (decimal)b.Kilos,
                    EAN = b.EAN,
                    DUN = b.DUN,
                    UNIDADESCAJA = Math.Round((decimal)unit.QtyPerUnitOfMeasure, 3),
                    PESONETO = Math.Round((decimal)unit.QtyPerUnitOfMeasure, 3) * (decimal)b.Kilos
                };

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                string term = filtro.ToLower();
                finalQuery = finalQuery.Where(x =>
                    x.OP.ToLower().Contains(term) ||
                    x.PF.ToLower().Contains(term) ||
                    x.MAQUINA.ToLower().Contains(term) ||
                    x.EAN.ToLower().Contains(term) ||
                    x.DUN.ToLower().Contains(term));
            }

            // Paginación
            var orderedQuery = finalQuery
                .OrderBy(x => x.MAQUINA);

            var totalRows = await orderedQuery.CountAsync();

            var pagedItems = await orderedQuery
                .Skip((pPageNumber - 1) * pPageSize)
                .Take(pPageSize)
                .ToListAsync();

            if ((pagedItems == null || !pagedItems.Any()) && string.IsNullOrWhiteSpace(filtro))
            {
                Log.Error("No se encuentran las ordenes prevision para la fecha {StartingDate:yyyy-MM-dd}", pFecha);
                throw new KeyNotFoundException(
                    $"No se encuentran las ordenes para la fecha '{pFecha:yyyy-MM-dd}'");
            }

            return new PaginatedResult<GKPrevisionDTO>
            {
                Items = pagedItems,
                TotalRows = totalRows,
                PageNumber = pPageNumber,
                PageSize = pPageSize
            };
        }



        public async Task<PaginatedResult<GKOrdenProdDTO>> GetOrdenesByLineaDestinoFecha(string pLinea, int pTipoDestino, DateTime pFecha, int pPageNumber, int pPageSize, string? filtro)
        {
            var query = _context.GoikoaLineaPrevisionFabricacions
                .Join(_context.GoikoaItems,
                      linea => linea.CodProducto,
                      item => item.No,
                      (linea, item) => new { linea, item })
                .Join(_context.GoikoaEscandalloFabricacións,
                      combo => combo.linea.CodProducto,
                      esca => esca.NoOpFabric,
                      (combo, esca) => new { combo.linea, combo.item, esca })
                .Where(x => x.linea.Fecha == pFecha &&
                            x.linea.Linea == pLinea &&
                            x.esca.ProductoPrincipal.ToString() == "1" &&
                            x.linea.TipoDestino == pTipoDestino)
                .Select(x => new GKOrdenProdDTO
                {
                    NumeroPrevision = x.linea.NºPrevision,
                    OP = x.linea.NºOrdenProduccion,
                    Orden = x.linea.Orden,
                    Maquina = x.linea.Linea,                           // [Linea] como Maquina
                    PF = x.linea.CodProducto,
                    Descripcion = x.item.Description,
                    FechaCaducidad = x.linea.FechaCaducidad.Date,
                    Fecha = x.linea.Fecha,
                    Lote = x.linea.Lote,
                    Cajas = (int)x.linea.CantidadEmbalaje,
                    UM = x.item.BaseUnitOfMeasure,
                    Sobres = (int)x.linea.CantidadPrincipal,
                    SobresHoraObjetivo = (int)x.item.SobresObjetivoHora,
                    TiempoProduccion = Math.Round((double)x.linea.HorasTrabajo, 2), // 2 decimales
                    NumeroPersonas = Math.Round((double)x.item.NoPersonas, 2), // 2 decimales
                    PC = x.esca.No,
                    DescripcionEsca = x.esca.Description,
                    CE = Math.Round((decimal)(x.esca.CantidadConRechazo * x.linea.CantidadPrincipal), 2)
                });

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                string term = filtro.ToLower();
                query = query.Where(x =>
                    (x.OP != null && x.OP.ToLower().Contains(term)) ||
                    (x.PF != null && x.PF.ToLower().Contains(term)) ||
                    (x.Maquina != null && x.Maquina.ToLower().Contains(term)) ||
                    (x.Descripcion != null && x.Descripcion.ToLower().Contains(term))
                );
            }

            // Total de registros sin paginación
            int totalRows = await query.CountAsync();

            // Aplicar paginación
            List<GKOrdenProdDTO>? lItems = await query
                                                    .OrderBy(x => x.Orden) // Ordenar por OP (puedes cambiar el criterio)
                                                    .Skip((pPageNumber - 1) * pPageSize)
                                                    .Take(pPageSize)
                                                    .ToListAsync();

            if (lItems == null || !lItems.Any() && string.IsNullOrWhiteSpace(filtro))
            {
                Log.Error("No se encuentran las ordenes para la linea {Line}, el tipo de destino {TipoDestino} y la fecha de inicio {StartingDate:yyyy-MM-dd}", pLinea, pTipoDestino, pFecha);
                string lTipoDestinoText = pTipoDestino == 2 ? "Loncheado" : (pTipoDestino == 5 ? "Empaquetado" : "");
                throw new KeyNotFoundException($"No se encuentran las ordenes para la linea '{pLinea}', el tipo de destino '{lTipoDestinoText}' y la fecha de inicio '{pFecha.ToString("yyyy-MM-dd")}'");
            }

            Log.Information("OK, se han encontrado {OrderCount} ordenes en total en el endpoint {Endpoint}", lItems.Count(), "GetOrdenesProduccion");

            return new PaginatedResult<GKOrdenProdDTO>
            {
                Items = lItems,
                TotalRows = totalRows,
                PageNumber = pPageNumber,
                PageSize = pPageSize
            };
        }
    }
}
