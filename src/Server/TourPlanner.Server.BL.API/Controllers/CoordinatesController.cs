using Microsoft.AspNetCore.Mvc;
using TourPlanner.Server.BL.Common.Interfaces;

namespace TourPlanner.Server.BL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoordinatesController : ControllerBase
    {
        private readonly IMapService _mapService;
        private readonly ILogger<TourController> _logger;
        private readonly ICoordinatesService _coordinatesService;

        public CoordinatesController(
            IMapService mapService,
            ICoordinatesService coordinatesService,
            ILogger<TourController> logger)
        {
            _mapService = mapService;
            _coordinatesService = coordinatesService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCoordinates([FromQuery] string address)
        {
            try
            {
                // Get coordinates from map service
                var map = await _coordinatesService.GetCoordinates(address);
                return Ok(map);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return StatusCode(500);
        }

        [HttpGet("Map")]
        public async Task<IActionResult> GetMapFromCoordinates([FromQuery] float lat, [FromQuery] float lon)
        {
            try
            {
                // Get tour map from map service
                var map = await _mapService.GetLocationMap(lat, lon);
                return File(map.ToArray(), "image/jpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return StatusCode(500);
        }
    }
}
