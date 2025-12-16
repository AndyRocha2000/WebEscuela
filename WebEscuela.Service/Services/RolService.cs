using Microsoft.EntityFrameworkCore;
using WebEscuela.Repository.Data;
using WebEscuela.Repository.Models;
using WebEscuela.Service.Interfaces;


namespace WebEscuela.Service.Services
{
    public class RolService : IRolService
    {
        // Declaramos el contexto para poder hacer consultas
        private readonly AppDbContext _context;

        // Constructor: Aquí recibimos el AppDbContext por inyección de dependencias
        public RolService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Rol>> GetAllRolesAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
