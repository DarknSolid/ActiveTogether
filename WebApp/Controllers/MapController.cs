using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLib;
using ModelLib.ApiDTOs;
using ModelLib.Constants;
using ModelLib.DTOs.DogPark;
using NetTopologySuite.Geometries;

namespace WebApp.Controllers
{
    [Route(ApiEndpoints.MAP)]
    [ApiController]
    public class MapController : ControllerBase
    {

        private readonly IDogParkRepository _dogParkRepository;

        public MapController(IDogParkRepository dogParkRepository)
        {
            _dogParkRepository = dogParkRepository;
        }

        [Authorize]
        [HttpPost(ApiEndpoints.MAP_POINTS)]
        public async Task<ActionResult<List<DogParkListDTO>>> GetPoints([FromBody] MapSearchDTO map)
        {
            Point upperLeft = new Point(map.West, map.North);
            Point lowerRight = new Point(map.East, map.South);
            return await _dogParkRepository.GetInAreaAsync(upperLeft, lowerRight);
        }
    }
}
