using Microsoft.AspNetCore.Mvc;
using TourPlanner.Common.Models;
using TourPlanner.Server.BL.API.Services;
using TourPlanner.Server.BL.MapQuestAPI;
using TourPlanner.Server.DAL.Repositories;

namespace TourPlanner.Server.BL.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TourController : ControllerBase
    {
        private readonly MapQuestService _mapQuestService;
        private readonly ILogger<TourController> _logger;
        private readonly PgsqlTourRepository _tourRepository;

        public TourController(
            ILogger<TourController> logger,
            MapQuestService mapQuestService,
            IRepositoryService repositoryService)
        {
            _logger = logger;
            _mapQuestService = mapQuestService;

            // Get tour repository
            {
                if (repositoryService.GetRepository<Tour>() is not PgsqlTourRepository repo)
                    throw new Exception($"No {nameof(PgsqlTourRepository)} is registered in the repository service!");
                _tourRepository = repo;
            }
        }

        [HttpGet]
        public IActionResult GetRoutes()
        {
            return NotFound();
        }

        [HttpGet("{id}")]
        public IActionResult GetTour(int id)
        {
            return NotFound();
        }

        [HttpGet("{searchTerm}")]
        public IActionResult GetRoutes([FromRoute] string searchTerm)
        {
            return NotFound();
        }


        [HttpPost]
        public IActionResult AddTour([FromBody] Tour tour)
        {
            // Check if tour is complete
            if(!IsValid(tour))
                return BadRequest();

            // Add tour to database through repository
            if(!_tourRepository.Insert(ref tour))
                return StatusCode(StatusCodes.Status500InternalServerError);

            // Return tour with ids
            return CreatedAtAction(nameof(GetTour), new { id = tour.Id }, tour);
        }

        private static bool IsValid(Tour tour)
        {
            return tour != null &&
                tour.Name != null &&
                tour.Name.Length > 0;
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