using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TourPlanner.Server.BL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoordinatesController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCoordinates([FromQuery] string address)
        {
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> GetMapFromCoordinates([FromQuery] float lat, [FromQuery] float lon)
        {
            return null;
        }
    }
}
