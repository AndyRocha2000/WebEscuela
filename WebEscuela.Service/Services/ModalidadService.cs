using Microsoft.EntityFrameworkCore;
using WebEscuela.Repository.Data;
using WebEscuela.Service.DTOs;
using WebEscuela.Service.Interfaces;

namespace WebEscuela.Service.Services
{
    public class ModalidadService : IModalidadService
    {
        private readonly AppDbContext _context;

        public ModalidadService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<LookupDTO>> ObtenerTodasLasModalidadesAsync()
        {
            return await _context.Modalidades
            .AsNoTracking()
            .OrderBy(m => m.Nombre)
            .Select(m => new LookupDTO
            {
                Id = m.Id,
                Nombre = m.Nombre
            })
            .ToListAsync();
        }
    }
}
