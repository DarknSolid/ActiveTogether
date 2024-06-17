using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLib.ApiDTOs;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.Constants;
using ModelLib.DTOs;
using ModelLib.DTOs.Instructors;
using ModelLib.Repositories;

namespace Web.Server.Controllers
{
    [Route(ApiEndpoints.DOG_INSTRUCTORS)]
    [ApiController]
    public class DogInstructorsController : CustomControllerBase
    {
        private readonly IDogInstructorRepository _instructorRepository;

        public DogInstructorsController(UserManager<ApplicationUser> userManager, IDogInstructorRepository instructorRepository) : base(userManager)
        {
            _instructorRepository = instructorRepository;
        }

        [Authorize]
        [HttpPost(ApiEndpoints.DOG_INSTRUCTORS_CREATE)]
        public async Task<ActionResult<int>> Create([FromBody] InstructorCreateDTO dto)
        {
            var (response, id) = await _instructorRepository.CreateInstructorAsync(userId: (await GetAuthorizedUserIdAsync()).Value, createDto: dto);
            return ResolveRepositoryResponse(response, id);
        }

        [Authorize]
        [HttpDelete(ApiEndpoints.DOG_INSTRUCTOR_DELETE)]
        public async Task<IActionResult> Delete()
        {
            var response = await _instructorRepository.DeleteInstructorAsync(userId: (await GetAuthorizedUserIdAsync()).Value);
            return ResolveRepositoryResponse(response);
        }

        [Authorize]
        [HttpPut(ApiEndpoints.DOG_INSTRUCTORS_UPDATE)]
        public async Task<IActionResult> Put([FromBody] InstructorUpdateDTO dto)
        {
            var response = await _instructorRepository.UpdateInstructorAsync(userId: (await GetAuthorizedUserIdAsync()).Value, updateDto: dto);
            return ResolveRepositoryResponse(response);
        }

        [HttpGet(ApiEndpoints.DOG_INSTRUCTORS_GET + "{dogInstructorId}")]
        public async Task<ActionResult<InstructorDetailedDTO>> Get(int dogInstructorId)
        {
            var (response, instructor) = await _instructorRepository.GetInstructorAsync(await GetAuthorizedUserIdAsync(), dogInstructorId);
            return ResolveRepositoryResponse(response, instructor);
        }

        [HttpPost(ApiEndpoints.DOG_INSTRUCTORS_LIST)]
        public async Task<ActionResult<DistancePaginationResult<DogTrainerListDTO>>> GetList([FromBody] DogTrainerRequest request)
        {
            var instructors = await _instructorRepository.GetInstructorsAsync(request);
            return Ok(instructors);
        }

        [HttpPost(ApiEndpoints.DOG_INSTRUCTORS_AREA)]
        public async Task<ActionResult<IEnumerable<DogTrainerListDTO>>> GetArea([FromBody] SearchAreaDTO searchArea)
        {
            var instructors = await _instructorRepository.GetInstructorsInAreaAsync(searchArea);
            return Ok(instructors);
        }
        
        [HttpGet(ApiEndpoints.DOG_TRAINING + "{id}")]
        public async Task<ActionResult<DogTrainingDetailsDTO?>> GetTraining(int id)
        {
            var (response, training) = await _instructorRepository.GetDogTrainingAsync(id);
            return ResolveRepositoryResponse(response, training);
        }

        [HttpPost(ApiEndpoints.DOG_TRAINING_LIST)]
        public async Task<ActionResult<DistancePaginationResult<DogTrainingListDTO>>> GetTrainingList([FromBody] DogTrainingRequest request)
        {
            var training = await _instructorRepository.GetDogTrainingsAsync(request);
            return Ok(training);
        }

        [HttpGet(ApiEndpoints.DOG_TRAINING_LIST + "{instructorCompanyId}")]
        public async Task<ActionResult<IEnumerable<DogTrainingListDTO>>> GetTrainingListFromInstructorId(int instructorCompanyId)
        {
            var training = await _instructorRepository.GetUpcommingDogTrainingsAsync(instructorCompanyId);
            return Ok(training);
        }

        [HttpGet(ApiEndpoints.DOG_TRAINING_LIST_ALL + "{instructorCompanyId}")]
        public async Task<ActionResult<AllDogTrainingsInstructorListDTO>> GetAllInstructorTrainings(int instructorCompanyId)
        {
            var training = await _instructorRepository.GetAllDogTrainingsAsync(instructorCompanyId);
            return Ok(training);
        }

        [Authorize]
        [HttpPost(ApiEndpoints.DOG_TRAINING_CREATE)]
        public async Task<ActionResult<int>> CreateTraining([FromBody] DogTrainingCreateDTO dto)
        {
            var userId = await GetAuthorizedUserIdAsync();
            var (response, id) = await _instructorRepository.CreateDogTrainingAsync(userId.Value, dto);
            return ResolveRepositoryResponse(response, id);
        }

        [Authorize]
        [HttpPut(ApiEndpoints.DOG_TRAINING_UPDATE)]
        public async Task<IActionResult> UpdateTraining([FromBody] DogTrainingUpdateDTO dto)
        {
            var userId = await GetAuthorizedUserIdAsync();
            var response = await _instructorRepository.UpdateDogTrainingAsync(userId.Value, dto);
            return ResolveRepositoryResponse(response, null);
        }

        [Authorize]
        [HttpDelete(ApiEndpoints.DOG_TRAINING + "{id}")]
        public async Task<ActionResult<int>> DeleteTraining(int id)
        {
            var userId = await GetAuthorizedUserIdAsync();
            var response = await _instructorRepository.DeleteDogTrainingAsync(userId.Value, id);
            return ResolveRepositoryResponse(response);
        }

        [HttpGet(ApiEndpoints.TRAINER_NAMES)]
        public async Task<ActionResult<Dictionary<int,string>>> GetTrainerNames()
        {
            return await _instructorRepository.GetTrainerNamesAsync();
        }
    }
}
