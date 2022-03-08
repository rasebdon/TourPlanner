using Microsoft.AspNetCore.Mvc;

namespace TourPlanner.BL.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TourController : ControllerBase
    {
        private readonly ILogger<TourController> _logger;

        public TourController(ILogger<TourController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetRoutes()
        {
            return NotFound();
        }

        [HttpGet("{searchTerm}")]
        public IActionResult GetRoutes([FromRoute] string searchTerm)
        {
            return NotFound();
        }


        [HttpPost]
        public IActionResult AddRoute([FromBody] object route)
        {
            return NotFound();
        }

        [HttpPut("{routeId}")]
        public IActionResult UpdateRoute([FromRoute] int routeId, [FromBody] object route)
        {
            return NotFound();
        }

        [HttpDelete("{routeId}")]
        public IActionResult DeleteRoute([FromRoute] int routeId)
        {
            return NotFound();
        }

        [HttpGet("{routeId}/Map")]
        public IActionResult GetRouteMap([FromRoute] int routeId)
        {
            return NotFound();
        }

        [HttpPost("{routeId}/Point")]
        public IActionResult AddRoutePoint([FromRoute] int routeId, [FromBody] object point)
        {
            return NotFound();
        }

        [HttpDelete("{routeId}/Point")]
        public IActionResult AddRoutePoint([FromRoute] int routeId)
        {
            return NotFound();
        }

        [HttpPut("{routeId}/Point")]
        public IActionResult UpdateRoutePoint([FromRoute] int routeId, [FromBody] object point)
        {
            return NotFound();
        }
    }
}