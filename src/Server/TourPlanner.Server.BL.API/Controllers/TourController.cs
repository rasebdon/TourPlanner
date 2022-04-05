using Microsoft.AspNetCore.Mvc;
using TourPlanner.Common.Models;
using TourPlanner.Server.BL.API.Services;
using TourPlanner.Server.BL.Common.Interfaces;
using TourPlanner.Server.DAL.Repositories.Pgsql;

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
        private readonly PgsqlTourEntryRepository _tourEntryRepository;

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
                    throw new MissingRepositoryException(typeof(PgsqlTourRepository));
                _tourRepository = repo;
            }
            // Get tour entry repository
            {
                if (repositoryService.GetRepository<TourEntry>() is not PgsqlTourEntryRepository repo)
                    throw new MissingRepositoryException(typeof(PgsqlTourEntryRepository));
                _tourEntryRepository = repo;
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
                return StatusCode(201, tour);
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
        public IActionResult UpdateTour([FromBody] Tour tour)
        {
            try
            {
                var updated = _tourRepository.Update(ref tour);
                if (!updated)
                    return BadRequest();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return StatusCode(500);
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
            try
            {
                entry.TourId = tourId;
                var inserted = _tourEntryRepository.Insert(ref entry);
                if (!inserted)
                    return BadRequest();
                return StatusCode(201, entry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return StatusCode(500);
        }

        [HttpDelete("{tourEntryId}/Entry")]
        public IActionResult DeleteTourEntry([FromRoute] int tourEntryId)
        {
            try
            {
                var deleted = _tourEntryRepository.Delete(tourEntryId);
                if (!deleted)
                    return BadRequest();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return StatusCode(500);
        }

        [HttpPut("{tourId}/Entry")]
        public IActionResult UpdateTourEntry([FromRoute] int tourId, [FromBody] TourEntry entry)
        {
            try
            {
                entry.TourId = tourId;
                var updated = _tourEntryRepository.Update(ref entry);
                if (!updated)
                    return BadRequest();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return StatusCode(500);
        }
    }
}