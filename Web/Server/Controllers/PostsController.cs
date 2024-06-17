using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.Constants;
using ModelLib.DTOs.Posts;
using ModelLib.Repositories;

namespace Web.Server.Controllers
{
    [Route(ApiEndpoints.POSTS)]
    [Authorize]
    [ApiController]
    public class PostsController : CustomControllerBase
    {
        private readonly IPostRepository _postRepository;
        public PostsController(UserManager<ApplicationUser> userManager, IPostRepository postRepository) : base(userManager)
        {
            _postRepository = postRepository;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<DateTimePaginationResult<PostDetailedDTO>>> GetPosts([FromBody] PostDateTimePaginationRequest request)
        {
            return Ok(await _postRepository.GetPosts(request));
        }

        [HttpPost(ApiEndpoints.POSTS_CREATE)]
        public async Task<ActionResult<PostCreateResult>> CreatePost([FromBody] PostCreateDTO postCreateDTO)
        {
            if (postCreateDTO.Media is not null && postCreateDTO.Media.Length > 10)
            {
                return BadRequest("At most 10 files allowed");
            }
            if (postCreateDTO.Body is null && postCreateDTO.Media is null)
            {
                return BadRequest("Either Body or Media must have a value");
            }

            var (response, createdResult) = await _postRepository.CreatePost((await GetAuthorizedUserIdAsync()).Value, postCreateDTO);
            return ResolveRepositoryResponse(response, createdResult);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var response = await _postRepository.DeletePost((await GetAuthorizedUserIdAsync()).Value, id);
            return ResolveRepositoryResponse(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostDetailedDTO?>> GetPost(int id)
        {
            return await _postRepository.GetPostAsync(id);
        }

        [HttpGet(ApiEndpoints.POSTS_LIKE + "{id}")]
        public async Task<ActionResult> LikePost(int id)
        {
            await _postRepository.LikePost((await GetAuthorizedUserIdAsync()).Value, id);
            return Ok();
        }

        [HttpPost(ApiEndpoints.POSTS_CREATE_COMMENT)]
        public async Task<ActionResult<int?>> CreateComment([FromBody] CommentCreateDTO commentCreateDTO)
        {
            var id = await _postRepository.CreateComment((await GetAuthorizedUserIdAsync()).Value, commentCreateDTO);
            return Ok(id);
        }

        [AllowAnonymous]
        [HttpPost(ApiEndpoints.POSTS_GET_COMMENTS)]
        public async Task<ActionResult<DateTimePaginationResult<CommentDetailedDTO>>> GetComments([FromBody] CommentGetRequest request)
        {
            return Ok(await _postRepository.GetComments(request));
        }

        [HttpDelete(ApiEndpoints.POSTS_DELETE_COMMENT + "{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var result = await _postRepository.DeleteComment((await GetAuthorizedUserIdAsync()).Value, id);
            return ResolveRepositoryResponse(result);
        }

        [HttpGet(ApiEndpoints.POSTS_COMMENT_LIKE + "{id}")]
        public async Task<ActionResult> LikeComment(int id)
        {
            await _postRepository.LikeComment((await GetAuthorizedUserIdAsync()).Value, id);
            return Ok();
        }

        [HttpGet(ApiEndpoints.POSTS_GET_COMMENT + "{id}")]
        public async Task<ActionResult<CommentDetailedDTO?>> GetComment(int id)
        {
            return await _postRepository.GetComment(id);
        }
    }
}
