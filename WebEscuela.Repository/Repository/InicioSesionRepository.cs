using Microsoft.EntityFrameworkCore;
using WebEscuela.Repository.Data;
using WebEscuela.Repository.Models;

namespace WebEscuela.Repository.Repository
{
    public class InicioSesionRepository
    {
        private readonly AppDbContext _context;

        public InicioSesionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CorreoElectronico == correo);
        }

        public async Task<string?> ObtenerNombreRolAsync(int rolId)
        {
            var rol = await _context.Roles.FindAsync(rolId);
            return rol?.Nombre;
        }
    }
}
