using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebEscuela.Service.DTOs;

namespace WebEscuela.Service.Interfaces
{
    public interface IModalidadService
    {
        Task<ICollection<LookupDTO>> ObtenerTodasLasModalidadesAsync();
    }
}
