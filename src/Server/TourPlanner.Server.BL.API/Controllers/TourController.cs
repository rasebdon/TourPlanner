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
        private readonly MapQuestMapService _mapQuestService;
        private readonly ILogger<TourController> _logger;
        private readonly PgsqlTourRepository _tourRepository;

        public TourController(
            ILogger<TourController> logger,
            MapQuestMapService mapQuestService,
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
        public IActionResult GetTours()
        {
            return NotFound();
        }

        [HttpGet("{id}")]
        public IActionResult GetTour(int id)
        {
            try
            {
                var tour = _tourRepository.Get(id);

                if(tour == null)
                    return NotFound();
                return Ok(tour);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return StatusCode(500);
        }

        [HttpGet("{searchTerm}")]
        public IActionResult GetTours([FromRoute] string searchTerm)
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

        [HttpPut("{tourId}")]
        public IActionResult UpdateTour([FromRoute] int tourId, [FromBody] Tour tour)
        {
            return NotFound();
        }

        [HttpDelete("{tourId}")]
        public IActionResult DeleteTour([FromRoute] int tourId)
        {
            try
            {
                var deleted = _tourRepository.Delete(tourId);
                if(!deleted)
                    return BadRequest();
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return StatusCode(500);
        }

        [HttpGet("{tourId}/Map")]
        public async Task<IActionResult> GetTourMap([FromRoute] int tourId)
        {
            try
            {
                // Get tour
                //var tour = _tourRepository.Get(tourId);
                var tour = new Tour()
                {
                    StartPoint = new()
                    {
                        Latitude = 40,
                        Longitude = 35,
                    },
                    EndPoint = new()
                    {
                        Latitude = 40.05f,
                        Longitude = 35.05f
                    }
                };

                if(tour == null)
                    return NotFound();

                // Get tour map from map service
                var map = await _mapQuestService.GetRouteMap(tour.StartPoint, tour.EndPoint);
                return File(map, "image/jpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return StatusCode(500);
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