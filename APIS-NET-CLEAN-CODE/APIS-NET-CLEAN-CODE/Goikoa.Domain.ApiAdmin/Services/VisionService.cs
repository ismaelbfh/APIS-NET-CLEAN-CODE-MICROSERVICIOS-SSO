using AutoMapper;
using Goikoa.Domain.ApiAdmin.DAL.Context;
using Goikoa.Domain.ApiAdmin.DAL.Models;
using Goikoa.Domain.ApiAdmin.DTOs.Requests;
using Goikoa.Domain.ApiAdmin.DTOs.Responses;
using Goikoa.Domain.ApiAdmin.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Log = Serilog.Log;

namespace Goikoa.Domain.ApiAdmin.Services
{
    public class VisionService : IVisionService
    {
        private readonly ApiAdminContext _context;
        private readonly IMapper _mapper;

        public VisionService(ApiAdminContext pContext, IMapper pMapper)
        {
            _context = pContext;
            _mapper = pMapper;
        }

        /// <summary>
        /// IMPORTANTE:
        /// Ahora el resumen se reutiliza por:
        /// - OrdenFabricacion
        /// - IpCamara
        ///
        /// Ya NO se crea uno distinto por tipo.
        /// El tipo quedará auditado a nivel de VisionLectura.
        /// </summary>
        public async Task<VisionOrdenResumenDTO> IniciarOrdenAsync(VisionIniciarOrdenRequest pRequest)
        {
            if (string.IsNullOrWhiteSpace(pRequest.OrdenFabricacion))
            {
                throw new ArgumentException("La OrdenFabricacion es obligatoria.");
            }

            if (string.IsNullOrWhiteSpace(pRequest.IpCamara))
            {
                throw new ArgumentException("La IpCamara es obligatoria.");
            }

            VisionOrdenResuman? lResumenExistente = await _context.VisionOrdenResumen
                .FirstOrDefaultAsync(x =>
                    x.OrdenFabricacion == pRequest.OrdenFabricacion &&
                    x.IpCamara == pRequest.IpCamara);

            if (lResumenExistente != null)
            {
                // Reutilizamos el registro de esa orden + cámara
                lResumenExistente.CodigoProducto = pRequest.CodigoProducto;
                lResumenExistente.DescripcionProducto = pRequest.DescripcionProducto;
                lResumenExistente.FechaProduccion = pRequest.FechaProduccion;
                lResumenExistente.Linea = pRequest.Linea;

                // Mantenemos estas propiedades como "último estado lanzado"
                // aunque ya no formen la clave del resumen.
                lResumenExistente.TipoEtiqueta = pRequest.TipoEtiqueta;
                lResumenExistente.PosicionQr = pRequest.PosicionQr;
                lResumenExistente.CodigoEsperado = pRequest.CodigoEsperado;
                lResumenExistente.IpCamara = pRequest.IpCamara;

                lResumenExistente.Activa = true;

                await _context.SaveChangesAsync();

                Log.Information(
                    "OK, se reutiliza resumen de visión para Orden {OrdenFabricacion} e IP {IpCamara}. ResumenId {ResumenId}",
                    lResumenExistente.OrdenFabricacion,
                    lResumenExistente.IpCamara,
                    lResumenExistente.Id);

                return _mapper.Map<VisionOrdenResumenDTO>(lResumenExistente);
            }

            VisionOrdenResuman lNuevoResumen = _mapper.Map<VisionOrdenResuman>(pRequest);

            lNuevoResumen.Id = Guid.NewGuid();
            lNuevoResumen.FechaHoraInicio = DateTime.UtcNow;
            lNuevoResumen.FechaHoraUltimaLectura = null;
            lNuevoResumen.TotalLecturas = 0;
            lNuevoResumen.TotalOk = 0;
            lNuevoResumen.TotalNok = 0;
            lNuevoResumen.Activa = true;
            lNuevoResumen.IpCamara = pRequest.IpCamara;

            await _context.VisionOrdenResumen.AddAsync(lNuevoResumen);
            await _context.SaveChangesAsync();

            Log.Information(
                "OK, se ha creado resumen de visión para Orden {OrdenFabricacion} e IP {IpCamara}. ResumenId {ResumenId}",
                lNuevoResumen.OrdenFabricacion,
                lNuevoResumen.IpCamara,
                lNuevoResumen.Id);

            return _mapper.Map<VisionOrdenResumenDTO>(lNuevoResumen);
        }

        public async Task<VisionLecturaDTO> RegistrarLecturaAsync(VisionRegistrarLecturaRequest pRequest)
        {
            VisionOrdenResuman? lResumen = await _context.VisionOrdenResumen
                .FirstOrDefaultAsync(x => x.Id == pRequest.VisionOrdenResumenId);

            if (lResumen == null)
            {
                Log.Error("No se encuentra el resumen de visión con Id {ResumenId}", pRequest.VisionOrdenResumenId);
                throw new KeyNotFoundException($"No se encuentra el resumen de visión '{pRequest.VisionOrdenResumenId}'");
            }

            string lCodigoEsperado = (lResumen.CodigoEsperado ?? string.Empty).Trim();
            string lCodigoLeido = (pRequest.CodigoLeido ?? string.Empty).Trim();

            bool lEsOk = string.Equals(lCodigoEsperado, lCodigoLeido, StringComparison.OrdinalIgnoreCase);

            VisionLectura lLectura = new VisionLectura
            {
                Id = Guid.NewGuid(),
                VisionOrdenResumenId = lResumen.Id,
                FechaHoraLectura = DateTime.UtcNow,
                CodigoEsperado = lCodigoEsperado,
                CodigoLeido = lCodigoLeido,
                Resultado = lEsOk ? "OK" : "NOK",
                RawMensaje = pRequest.RawMensaje,
                Observaciones = pRequest.Observaciones,
                TipoEtiqueta = pRequest.TipoEtiqueta
            };

            await _context.VisionLecturas.AddAsync(lLectura);

            lResumen.TotalLecturas += 1;
            lResumen.FechaHoraUltimaLectura = lLectura.FechaHoraLectura;

            if (lEsOk)
                lResumen.TotalOk += 1;
            else
                lResumen.TotalNok += 1;

            await _context.SaveChangesAsync();

            Log.Information(
                "OK, lectura registrada para Orden {OrdenFabricacion}, IP {IpCamara}, Tipo {TipoEtiqueta}. Esperado: {Esperado}, Leído: {Leido}, Resultado: {Resultado}",
                lResumen.OrdenFabricacion,
                lResumen.IpCamara,
                pRequest.TipoEtiqueta,
                lCodigoEsperado,
                lCodigoLeido,
                lLectura.Resultado);

            return _mapper.Map<VisionLecturaDTO>(lLectura);
        }

        public async Task<VisionOrdenResumenDTO> GetResumenByOrdenAsync(string pOrdenFabricacion)
        {
            VisionOrdenResuman? lResumen = await _context.VisionOrdenResumen
                .Where(x => x.OrdenFabricacion == pOrdenFabricacion)
                .OrderByDescending(x => x.FechaHoraInicio)
                .FirstOrDefaultAsync();

            if (lResumen == null)
            {
                Log.Error("No se encuentra resumen de visión para la orden {OrdenFabricacion}", pOrdenFabricacion);
                throw new KeyNotFoundException($"No se encuentra resumen de visión para la orden '{pOrdenFabricacion}'");
            }

            Log.Information(
                "OK, se ha encontrado el resumen de visión para la orden {OrdenFabricacion}",
                pOrdenFabricacion);

            return _mapper.Map<VisionOrdenResumenDTO>(lResumen);
        }

        public async Task<VisionOrdenResumenDTO> FinalizarOrdenAsync(VisionFinalizarOrdenRequest pRequest)
        {
            VisionOrdenResuman? lResumen = await _context.VisionOrdenResumen
                .FirstOrDefaultAsync(x => x.Id == pRequest.VisionOrdenResumenId);

            if (lResumen == null)
            {
                Log.Error("No se encuentra el resumen de visión con Id {ResumenId} para finalizar", pRequest.VisionOrdenResumenId);
                throw new KeyNotFoundException($"No se encuentra el resumen de visión '{pRequest.VisionOrdenResumenId}'");
            }

            lResumen.Activa = false;

            await _context.SaveChangesAsync();

            Log.Information(
                "OK, se ha finalizado el resumen de visión {ResumenId} para Orden {OrdenFabricacion} e IP {IpCamara}",
                lResumen.Id,
                lResumen.OrdenFabricacion,
                lResumen.IpCamara);

            return _mapper.Map<VisionOrdenResumenDTO>(lResumen);
        }

        public async Task<List<VisionLecturaDTO>> GetLecturasByResumenIdAsync(Guid pVisionOrdenResumenId)
        {
            List<VisionLectura> lLecturas = await _context.VisionLecturas
                .Where(x => x.VisionOrdenResumenId == pVisionOrdenResumenId)
                .OrderByDescending(x => x.FechaHoraLectura)
                .ToListAsync();

            Log.Information(
                "OK, se han encontrado {LecturaCount} lecturas para el resumen de visión {ResumenId}",
                lLecturas.Count,
                pVisionOrdenResumenId);

            return _mapper.Map<List<VisionLecturaDTO>>(lLecturas);
        }

        public async Task<List<VisionOrdenResumenDTO>> GetResumenesByOrdenAsync(string pOrdenFabricacion)
        {
            List<VisionOrdenResuman> lResumenes = await _context.VisionOrdenResumen
                .Where(x => x.OrdenFabricacion == pOrdenFabricacion)
                .OrderByDescending(x => x.FechaHoraInicio)
                .ToListAsync();

            return _mapper.Map<List<VisionOrdenResumenDTO>>(lResumenes);
        }

        /// <summary>
        /// Se mantiene por compatibilidad.
        /// Ya NO se usa para cambiar la clave del resumen, solo como
        /// actualización del estado actual del resumen reutilizable.
        /// </summary>
        public async Task<VisionOrdenResumenDTO> ActualizarOrdenAsync(VisionActualizarOrdenRequest pRequest)
        {
            VisionOrdenResuman? lResumen = await _context.VisionOrdenResumen
                .FirstOrDefaultAsync(x => x.Id == pRequest.VisionOrdenResumenId);

            if (lResumen == null)
            {
                Log.Error("No se encuentra el resumen de visión con Id {ResumenId} para actualizar", pRequest.VisionOrdenResumenId);
                throw new KeyNotFoundException($"No se encuentra el resumen de visión '{pRequest.VisionOrdenResumenId}'");
            }

            lResumen.CodigoEsperado = pRequest.CodigoEsperado;
            lResumen.TipoEtiqueta = pRequest.TipoEtiqueta;
            lResumen.PosicionQr = pRequest.PosicionQr;
            lResumen.Activa = true;

            await _context.SaveChangesAsync();

            Log.Information(
                "OK, se ha actualizado el resumen de visión {ResumenId} para Orden {OrdenFabricacion}",
                lResumen.Id,
                lResumen.OrdenFabricacion);

            return _mapper.Map<VisionOrdenResumenDTO>(lResumen);
        }
    }
}