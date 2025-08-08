using Microsoft.EntityFrameworkCore;
using WebEscuela.Repository.Data;
using WebEscuela.Repository.Models;

namespace WebEscuela.Repository.Repository
{
    public class GestionCarreraRepository
    {
        private readonly AppDbContext _context;

        public GestionCarreraRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Carrera>> ObtenerTodasAsync()
        {
            return await _context.Carreras.ToListAsync();
        }

        public async Task<Carrera?> ObtenerPorIdAsync(int id)
        {
            return await _context.Carreras.FindAsync(id);
        }

        public async Task CrearAsync(Carrera carrera)
        {
            _context.Carreras.Add(carrera);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Carrera carrera)
        {
            _context.Carreras.Update(carrera);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var carrera = await _context.Carreras.FindAsync(id);
            if (carrera != null)
            {
                _context.Carreras.Remove(carrera);
                await _context.SaveChangesAsync();
            }
        }
    }
}
