using Microsoft.AspNetCore.Mvc;
using TourPlanner.Common.Models;
using TourPlanner.Server.BL.API.Services;
using TourPlanner.Server.BL.Common.Interfaces;
using TourPlanner.Server.DAL.Repositories;
using TourPlanner.Server.DAL.Repositories.Pgsql;

namespace TourPlanner.Server.BL.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TourController : ControllerBase
    {
        private readonly IMapService _mapService;
        private readonly IRouteService _tourService;
        private readonly ILogger<TourController> _logger;
        private readonly IRepository<Tour> _tourRepository;
        private readonly IRepository<TourEntry> _tourEntryRepository;

        public TourController(
            ILogger<TourController> logger,
            IMapService mapService,
            IRouteService tourService,
            IRepositoryService repositoryService)
        {
            _logger = logger;
            _mapService = mapService;
            _tourService = tourService;

            // Get repositories
            _tourRepository = repositoryService.GetRepository<Tour>() ?? 
                throw new MissingRepositoryException(typeof(PgsqlTourRepository));

            _tourEntryRepository = repositoryService.GetRepository<TourEntry>() ??
                throw new MissingRepositoryException(typeof(PgsqlTourEntryRepository));
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

                // Get additional info of tour
                var info = await _tourService.GetRouteInfo(tour.StartPoint, tour.EndPoint, tour.TransportType);

                if(info == null)
                    return BadRequest();

                tour.Distance = info.Distance;
                tour.EstimatedTime = info.Time;

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
        public async Task<IActionResult> GetTourMap([FromRoute] int tourId, [FromQuery] float? width, [FromQuery] float? height)
        {
            try
            {
                // Get tour
                var tour = _tourRepository.Get(tourId);

                if(tour == null)
                    return NotFound();

                if (tour.StartPoint == null || tour.EndPoint == null)
                    return StatusCode(500);

                if(width == null || height == null)
                {
                    width = 0;
                    height = 0;
                }

                // Get tour map from map service
                var map = await _mapService.GetRouteMap(tour.StartPoint, tour.EndPoint, width.Value, height.Value);
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

        [HttpDelete("Entry/{entryId}")]
        public IActionResult DeleteTourEntry([FromRoute] int entryId)
        {
            try
            {
                var deleted = _tourEntryRepository.Delete(entryId);
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

        [HttpPut("Entry/{entryId}")]
        public IActionResult UpdateTourEntry([FromRoute] int entryId, [FromBody] TourEntry entry)
        {
            try
            {
                entry.Id = entryId;
                var updated = _tourEntryRepository.Update(ref entry);
                if (!updated)
                    return BadRequest();
                return Ok(entry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return StatusCode(500);
        }
    }
}