using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SolicitudRepository : BaseRepository, ISolicitudRepository
    {
        public SolicitudRepository(AppDbContext context) : base(context) { }

        public async Task<IList<Solicitud>> GetSolicitudes()
        {
            return await context.Solicitudes
                .Include(s => s.Usuario)
                .Include(s => s.Computador)
                .Include(s => s.Sala)
                .ToListAsync();
        }

        public async Task<Solicitud> GetSolicitud(Guid id)
        {
            return await context.Solicitudes
                .Include(s => s.Usuario)
                .Include(s => s.Computador)
                .Include(s => s.Sala)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task Save(Solicitud solicitud)
        {
            try
            {
                await Beguin();
                await context.Solicitudes.AddAsync(solicitud);
                await context.SaveChangesAsync();
                await Comit();
            }
            catch
            {
                await RollBack();
                throw;
            }
        }

        public async Task Update(Solicitud solicitud)
        {
            try
            {
                await Beguin();
                context.Solicitudes.Update(solicitud);
                await context.SaveChangesAsync();
                await Comit();
            }
            catch
            {
                await RollBack();
                throw;
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                await Beguin();
                var entity = await context.Solicitudes.FirstOrDefaultAsync(s => s.Id == id);
                if (entity != null)
                {
                    context.Solicitudes.Remove(entity);
                    await context.SaveChangesAsync();
                }
                await Comit();
            }
            catch
            {
                await RollBack();
                throw;
            }
        }

        public async Task<IList<Solicitud>> GetByEstado(string estado)
        {
            return await context.Solicitudes
                .Where(s => s.Estado == estado)
                .Include(s => s.Usuario)
                .Include(s => s.Computador)
                .Include(s => s.Sala)
                .ToListAsync();
        }

        public async Task AcceptSolicitud(Guid solicitudId)
        {
            try
            {
                await Beguin();

                var solicitud = await context.Solicitudes
                    .Include(s => s.Computador)
                    .FirstOrDefaultAsync(s => s.Id == solicitudId);
                if (solicitud == null) { await Comit(); return; }

                // Manejar según el tipo de solicitud
                if (solicitud.Tipo == "Liberacion")
                {
                    var computador = await context.Computadores.FirstOrDefaultAsync(c => c.Id == solicitud.ComputadorId);
                    if (computador != null)
                    {
                        // Liberar el equipo
                        computador.Estado = "Disponible";
                        // Cerrar todas las solicitudes activas de este equipo
                        var hoy = DateTime.Today;
                        var solicitudesActivas = await context.Solicitudes
                            .Where(s => s.ComputadorId == solicitud.ComputadorId &&
                                       s.Estado == "Aceptado" &&
                                       s.Tipo != "Liberacion" &&
                                       s.FechaInicio.Date <= hoy &&
                                       s.FechaFin.Date >= hoy)
                            .ToListAsync();
                        
                        foreach (var solActiva in solicitudesActivas)
                        {
                            solActiva.FechaFin = DateTime.Today.AddSeconds(-1);
                            context.Solicitudes.Update(solActiva);
                        }
                        
                        context.Computadores.Update(computador);
                    }
                }
                else if (solicitud.Tipo == "Asignacion")
                {
                    var computador = await context.Computadores.FirstOrDefaultAsync(c => c.Id == solicitud.ComputadorId);
                    if (computador != null)
                    {
                        // Asignación: marcar como ocupado y asignar al usuario
                        computador.Estado = "Ocupado";
                        context.Computadores.Update(computador);
                    }
                }
                else if (solicitud.Tipo == "Prestamo")
                {
                    // Préstamo: solo reservar para las fechas indicadas, no cambiar estado todavía
                    // El equipo se marcará como ocupado cuando llegue la fecha de inicio
                    // Por ahora, el estado se mantiene como está
                    // No se asigna al usuario todavía, solo se reserva
                    // No hacer nada con el equipo
                }
                else if (solicitud.Tipo == "Danio" || solicitud.Tipo == "Asesoria")
                {
                    // Daño y Asesoría: NO hacer nada con el equipo, solo aceptar la solicitud
                    // El coordinador decide después si bloquear o no el equipo
                    // No cambiar estado, no asignar usuario, solo registrar la solicitud
                }

                solicitud.Estado = "Aceptado";
                context.Solicitudes.Update(solicitud);

                await context.SaveChangesAsync();
                await Comit();
            }
            catch
            {
                await RollBack();
                throw;
            }
        }

        public async Task DenySolicitud(Guid solicitudId)
        {
            try
            {
                await Beguin();
                var solicitud = await context.Solicitudes.FirstOrDefaultAsync(s => s.Id == solicitudId);
                if (solicitud != null)
                {
                    solicitud.Estado = "Denegado";
                    context.Solicitudes.Update(solicitud);
                    await context.SaveChangesAsync();
                }
                await Comit();
            }
            catch
            {
                await RollBack();
                throw;
            }
        }

        public async Task CerrarSolicitudesActivasPorEquipo(Guid computadorId)
        {
            try
            {
                await Beguin();
                var hoy = DateTime.Today;
                
                // Obtener solicitudes activas directamente del contexto
                var solicitudesActivas = await context.Solicitudes
                    .Where(s => s.ComputadorId == computadorId &&
                               s.Estado == "Aceptado" &&
                               s.Tipo != "Liberacion" &&
                               s.FechaInicio.Date <= hoy &&
                               s.FechaFin.Date >= hoy)
                    .ToListAsync();
                
                // Cerrar cada solicitud estableciendo fecha fin al inicio del día
                foreach (var solicitud in solicitudesActivas)
                {
                    solicitud.FechaFin = DateTime.Today.AddSeconds(-1);
                    context.Solicitudes.Update(solicitud);
                }
                
                await context.SaveChangesAsync();
                await Comit();
            }
            catch
            {
                await RollBack();
                throw;
            }
        }
    }
}

