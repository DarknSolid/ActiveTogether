using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLib.Constants;
using ModelLib.DTOs.Places;
using ModelLib.Repositories;

namespace Web.Server.Controllers
{
    [Route(ApiEndpoints.PLACES)]
    [ApiController]
    public class PlacesController : CustomControllerBase
    {
        private readonly IPlacesRepository _placesRepository;

        public PlacesController(UserManager<ApplicationUser> userManager, IPlacesRepository placesRepository) : base(userManager)
        {
            _placesRepository = placesRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlaceListDTO?>> GetPlace(int id)
        {
            var (response, dto) = await _placesRepository.GetPlaceAsync(id);
            return ResolveRepositoryResponse(response, dto);
        }

    }
}
