using WebEscuela.Repository.Models;

namespace WebEscuela.Service.Interfaces
{
    public interface IRolService
    {
        Task<ICollection<Rol>> GetAllRolesAsync();
    }
}
