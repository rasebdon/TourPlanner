using Microsoft.AspNetCore.Mvc;
using TourPlanner.Server.BL.MapQuestAPI;

namespace TourPlanner.Server.BL.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TourController : ControllerBase
    {
        private readonly MapQuestService _mapQuestService;
        private readonly ILogger<TourController> _logger;

        public TourController(
            ILogger<TourController> logger,
            MapQuestService mapQuestService)
        {
            _logger = logger;
            _mapQuestService = mapQuestService;
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
        public async Task<IActionResult> GetRouteMap([FromRoute] int routeId)
        {
            var map = await _mapQuestService.GetRouteMap("Nußdorfer Straße, Wien, AUT", "Höchstädtplatz, Wien, AUT");
            return File(map, "image/jpeg");
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