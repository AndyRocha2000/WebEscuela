using Microsoft.EntityFrameworkCore;
using WebEscuela.Repository.Data;
using WebEscuela.Service.DTOs;
using WebEscuela.Service.Interfaces;

namespace WebEscuela.Service.Services
{
    public class TurnoService : ITurnoService
    {
        private readonly AppDbContext _context;

        public TurnoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<LookupDTO>> ObtenerTodosLosTurnosAsync()
        {
            return await _context.Turnos
            .AsNoTracking()
            .OrderBy(t => t.Nombre)
            .Select(t => new LookupDTO
            {
                Id = t.Id,
                Nombre = t.Nombre
            })
            .ToListAsync();
        }
    }
}
