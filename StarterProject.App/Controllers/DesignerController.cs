using Editor.Controllers;
using StarterProject.Shared.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace StarterProject.App.Controllers;

public class DesignerController : Controller
{
        private readonly ILogger<DesignerController> _logger;

        public DesignerController(ILogger<DesignerController> logger, IOptions<ApplicationSettings> settings)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
}
