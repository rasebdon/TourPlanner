using Microsoft.AspNetCore.Mvc;
using TourPlanner.Common.Models;
using TourPlanner.Server.BL.API.Services;
using TourPlanner.Server.BL.Common.Interfaces;
using TourPlanner.Server.BL.MapQuestAPI;
using TourPlanner.Server.DAL.Repositories;

namespace TourPlanner.Server.BL.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TourController : ControllerBase
    {
        private readonly IMapService _mapService;
        private readonly ITourService _tourService;
        private readonly ILogger<TourController> _logger;
        private readonly PgsqlTourRepository _tourRepository;

        public TourController(
            ILogger<TourController> logger,
            IMapService mapService,
            ITourService tourService,
            IRepositoryService repositoryService)
        {
            _logger = logger;
            _mapService = mapService;
            _tourService = tourService;

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
            try
            {
                var tours = _tourRepository.GetAll();

                if (tours == null)
                    return NotFound();
                return Ok(tours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return StatusCode(500);
        }

        [HttpGet("{tourId}")]
        public IActionResult GetTour(int tourId)
        {
            try
            {
                var tour = _tourRepository.Get(tourId);

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

        [HttpGet("Search/{searchTerm}")]
        public IActionResult GetTours([FromRoute] string searchTerm)
        {
            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> AddTour([FromBody] Tour tour)
        {
            try
            {
                // Check if tour is complete
                if (!IsValid(tour))
                    return BadRequest();

                // Get distance of tour
                tour.Distance = await _tourService.GetDistance(tour.StartPoint, tour.EndPoint);

                // Add tour to database through repository
                if (!_tourRepository.Insert(ref tour))
                    return StatusCode(StatusCodes.Status500InternalServerError);

                // Return tour with ids
                return CreatedAtAction(nameof(GetTour), new { id = tour.Id }, tour);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return StatusCode(500);
        }

        private static bool IsValid(Tour tour)
        {
            return tour != null &&
                tour.Name != null &&
                tour.Name.Length > 0 &&
                tour.StartPoint != null &&
                tour.EndPoint != null;
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
                var tour = _tourRepository.Get(tourId);

                if(tour == null)
                    return NotFound();

                if (tour.StartPoint == null || tour.EndPoint == null)
                    return StatusCode(500);

                // Get tour map from map service
                var map = await _mapService.GetRouteMap(tour.StartPoint, tour.EndPoint);
                return File(map.ToArray(), "image/jpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return StatusCode(500);
        }

        [HttpPost("{tourId}/Entry")]
        public IActionResult AddTourEntry([FromRoute] int tourId, [FromBody] TourEntry entry)
        {
            return NotFound();
        }

        [HttpDelete("{tourId}/Entry")]
        public IActionResult DeleteTourEntry([FromRoute] int tourId)
        {
            return NotFound();
        }

        [HttpPut("{tourId}/Entry")]
        public IActionResult UpdateTourEntry([FromRoute] int tourId, [FromBody] TourEntry entry)
        {
            return NotFound();
        }
    }
}