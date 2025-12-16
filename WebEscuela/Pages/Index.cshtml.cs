using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebEscuela.Pages
{
    public class IndexModel : PageModel
    {
        // Solo conservamos el logger
        private readonly ILogger<IndexModel> _logger;

        // Constructor solo con ILogger
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
            _logger.LogInformation("Página Index cargada exitosamente.");
        }
    }
}