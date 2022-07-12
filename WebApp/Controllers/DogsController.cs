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
    public class DogsController : CustomControllerBase
    {
        private readonly IDogRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;


        public DogsController(IDogRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        [HttpPost(ApiEndpoints.DOGS_CREATE)]
        public async Task<ActionResult<int>> Create([FromBody] DogCreateDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);
            var (response, id) = await _repository.CreateAsync(user.Id, dto);
            return ResolveRepositoryResponse(response, id);
        }

        [HttpPost(ApiEndpoints.DOGS_UPDATE)]
        public async Task<ActionResult<int>> Update([FromBody] DogUpdateDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);
            var (response, id) = await _repository.UpdateAsync(user.Id, dto);
            return ResolveRepositoryResponse(response, id);
        }

        [HttpDelete(ApiEndpoints.DOGS_DELETE + "{id}/")]
        public async Task<ActionResult<int>> Delete(int dogId)
        {
            var user = await _userManager.GetUserAsync(User);
            var response = await _repository.DeleteDogAsync(user.Id, dogId);
            return ResolveRepositoryResponse(response);
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
