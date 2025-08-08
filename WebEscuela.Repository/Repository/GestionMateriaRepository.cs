using Microsoft.EntityFrameworkCore;
using WebEscuela.Repository.Data;
using WebEscuela.Repository.Models;

namespace WebEscuela.Repository.Repository
{
    public class GestionMateriaRepository
    {
        private readonly AppDbContext _context;

        public GestionMateriaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Materia>> ObtenerTodasAsync()
        {
            return await _context.Materias.ToListAsync();
        }

        public async Task<Materia?> ObtenerPorIdAsync(int id)
        {
            return await _context.Materias.FindAsync(id);
        }

        public async Task CrearAsync(Materia materia)
        {
            _context.Materias.Add(materia);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Materia materia)
        {
            _context.Materias.Update(materia);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var materia = await _context.Materias.FindAsync(id);
            if (materia != null)
            {
                _context.Materias.Remove(materia);
                await _context.SaveChangesAsync();
            }
        }
    }
}
