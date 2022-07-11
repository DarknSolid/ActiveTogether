using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLib.Constants;
using ModelLib.DTOs.DogPark;
using ModelLib.Repositories;

namespace WebApp.Controllers
{
    [Route(ApiEndpoints.DOG_PARKS)]
    [ApiController]
    public class DogParkController : ControllerBase
    {
        private readonly IDogParkRepository _dogParkRepository;

        public DogParkController(IDogParkRepository dogParkRepository)
        {
            _dogParkRepository = dogParkRepository;
        }

        [Authorize]
        [HttpGet(ApiEndpoints.DOG_PARKS_GET + "{id}")]
        public async Task<ActionResult<DogParkDetailedDTO?>> GetPoints(int id)
        {
            return await _dogParkRepository.GetAsync(id);
        }

    }
}
