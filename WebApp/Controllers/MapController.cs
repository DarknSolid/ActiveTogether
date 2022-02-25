using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLib;
using ModelLib.ApiDTOs;
using ModelLib.Constants;
using ModelLib.DTOs;
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
        public async Task<ActionResult<MapSearchResultDTO>> GetPoints([FromBody] MapSearchDTO map)
        {
            var dogs = await _dogParkRepository.GetInAreaAsync(map.BoundsDTO);
            return new MapSearchResultDTO()
            {
                DogParkListDTOs = dogs
            };
        }
    }
}
