using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLib.ApiDTOs;
using ModelLib.Constants;
using ModelLib.DTOs.Dogs;
using ModelLib.Repositories;

namespace WebApp.Controllers
{
    [Authorize]
    [Route(ApiEndpoints.DOGS)]
    [ApiController]
    public class DogsController : ControllerBase
    {
        private readonly IDogRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;


        public DogsController(IDogRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        [HttpPost(ApiEndpoints.DOGS_CREATE)]
        public async Task<ActionResult<DogCreatedDTO>> Create([FromBody] DogCreateDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);
            return Ok(await _repository.CreateAsync(user.Id, dto));
        }

        [HttpPost(ApiEndpoints.DOGS_UPDATE)]
        public async Task<ActionResult<DogUpdatedDTO>> Update([FromBody] DogUpdateDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);
            return Ok(await _repository.UpdateAsync(user.Id, dto));
        }

        [HttpGet(ApiEndpoints.DOGS_LIST + "{id}")]
        public async Task<ActionResult<List<DogListDTO>>> GetListView(int id)
        {
            return Ok(await _repository.GetAsync(id));
        }

        [HttpGet(ApiEndpoints.DOGS_DETAILED + "{id}")]
        public async Task<ActionResult<DogDetailedDTO?>> GetDetailed(int id)
        {
            return Ok(await _repository.GetDetailedAsync(id));
        }

        [HttpGet(ApiEndpoints.DOG_BREEDS)]
        public async Task<ActionResult<IDictionary<int, string>>> GetDogBreeds()
        {
            return Ok(await _repository.GetDogBreedsAsync());
        }

    }
}
