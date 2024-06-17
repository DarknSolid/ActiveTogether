using EntityLib;
using EntityLib.Entities.PostsAndComments;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.DTOs.Posts;
using static ModelLib.Repositories.RepositoryEnums;

namespace ModelLib.Repositories
{
    public interface IPostRepository
    {
        Task<int?> CreateComment(int userId, CommentCreateDTO dto);
        Task<(ResponseType, PostCreateResult)> CreatePost(int userId, PostCreateDTO postCreateDTO);
        Task<ResponseType> DeleteComment(int userId, int commentId);
        Task<ResponseType> DeletePost(int userId, int postId);
        Task<DateTimePaginationResult<CommentDetailedDTO>> GetComments(CommentGetRequest request);
        Task<PostDetailedDTO?> GetPostAsync(int id);
        Task<DateTimePaginationResult<PostDetailedDTO>> GetPosts(PostDateTimePaginationRequest request);
        Task LikeComment(int userId, int commentId);
        Task LikePost(int userId, int postId);
        Task<CommentDetailedDTO?> GetComment(int commentId);
    }

    public class PostRepository : RepositoryBase, IPostRepository
    {
        private readonly IBlobStorageRepository _blobStorageRepository;
        public PostRepository(IApplicationDbContext context, IBlobStorageRepository blobStorageRepository) : base(context)
        {
            _blobStorageRepository = blobStorageRepository;
        }

        public async Task<(ResponseType, PostCreateResult)> CreatePost(int userId, PostCreateDTO postCreateDTO)
        {
            var result = new PostCreateResult
            {
                Success = true,
            };

            var entity = new Post
            {
                UserId = userId,
                Body = postCreateDTO.Body,
                Area = postCreateDTO.Area,
                Category = postCreateDTO.Category,
                PlaceId = postCreateDTO.PlaceId,
            };
            _context.Posts.Add(entity);
            await _context.SaveChangesAsync();

            result.PostId = entity.Id;

            if (postCreateDTO.Media is not null)
            {
                var uriTokenTuples = await _blobStorageRepository.CreatePostFileBlobUploadUrls(postCreateDTO.Media, entity.Id);

                entity.Media = uriTokenTuples.Select(t => t.Item1).ToArray();
                await _context.SaveChangesAsync();

                result.BlobUploadUrls = uriTokenTuples.Select(t => t.Item2).ToArray();
            }

            return (ResponseType.Created, result);
        }

        public async Task<ResponseType> DeletePost(int userId, int postId)
        {
            var post = await _context.Posts.Where(post => post.Id == postId && post.UserId == userId).FirstOrDefaultAsync();
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                await _blobStorageRepository.DeletePublicPostAsync(postId);
            }
            return ResponseType.Deleted;
        }

        public async Task<PostDetailedDTO?> GetPostAsync(int id)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Where(p => p.Id == id)
                .Select(p => new PostDetailedDTO
                {
                    Body = p.Body,
                    DateTime = p.DateTime,
                    Id = id,
                    MediaUrls = p.Media,
                    UserId = p.UserId,
                    UserFirstName = p.User.FirstName,
                    UserLastName = p.User.LastName,
                    UserImageUrl = p.User.ProfileImageUrl,
                    Area = p.Area,
                    Category = p.Category,
                })
                .FirstOrDefaultAsync();

            if (post is not null)
            {
                await SetPostImageUrls(post);

            }
            return post;
        }

        public async Task<DateTimePaginationResult<PostDetailedDTO>> GetPosts(PostDateTimePaginationRequest request)
        {
            var query = _context.Posts
                .Include(p => p.User)
                .Include(p => p.PostLikes)
                .Include(p => p.PostComments)
                .Include(p => p.User.Companies)
                .AsQueryable();
            if (request.Filter is not null)
            {
                var filter = request.Filter;
                if (filter.IncludePlaceDetails)
                {
                    query = query.Include(p => p.Place);
                }
                if (filter.Area is not null)
                {
                    query = query.Where(p => p.Area == filter.Area);
                }
                if (filter.Category is not null)
                {
                    query = query.Where(p => p.Category == filter.Category);
                }
                if (filter.UserId is not null)
                {
                    query = query.Where(p => p.UserId == filter.UserId);
                }
                if (filter.PlaceId is not null)
                {
                    query = query.Where(p => p.PlaceId == filter.PlaceId);
                }
            }

            var select = (IQueryable<Post> q) => q.Select(p => new PostDetailedDTO
            {
                Likes = p.PostLikes.Where(l => l.IsLike).Select(l => new LikeDetailedDTO
                {
                    UserId = l.UserId,
                    UserName = l.User.FirstName + " " + l.User.LastName
                }).ToList(),
                Id = p.Id,
                UserId = p.UserId,
                DateTime = p.DateTime,
                Body = p.Body,
                UserFirstName = p.User.FirstName,
                UserLastName = p.User.LastName,
                UserImageUrl = p.User.ProfileImageUrl,
                MediaUrls = p.Media,
                TotalComments = p.PostComments.Count,
                Area = p.Area,
                Category = p.Category,
                PlaceId = p.PlaceId,
                UserProfession =
                p.User.Companies.Any() ?
                    _context.Places.Where(c => c.Id == p.User.Companies.First().PlaceId).Select(p => p.FacilityType).First() :
                    null,

            });
            if (request.Filter is not null && request.Filter.IncludePlaceDetails)
            {
                select = (IQueryable<Post> q) => q.Select(p => new PostDetailedDTO
                {
                    Likes = p.PostLikes.Where(l => l.IsLike).Select(l => new LikeDetailedDTO
                    {
                        UserId = l.UserId,
                        UserName = l.User.FirstName + " " + l.User.LastName
                    }).ToList(),
                    Id = p.Id,
                    UserId = p.UserId,
                    DateTime = p.DateTime,
                    Body = p.Body,
                    UserFirstName = p.User.FirstName,
                    UserLastName = p.User.LastName,
                    UserImageUrl = p.User.ProfileImageUrl,
                    MediaUrls = p.Media,
                    TotalComments = p.PostComments.Count,
                    Area = p.Area,
                    Category = p.Category,
                    PlaceId = p.PlaceId,
                    PlaceImageUrl = p.Place.ProfileImageBlobUrl,
                    PlaceName = p.Place.Name,
                    PlaceFacilityType = p.Place.FacilityType,
                    UserProfession =
                        p.User.Companies.Any() ?
                            _context.Places.Where(c => c.Id == p.User.Companies.First().PlaceId).Select(p => p.FacilityType).First() :
                            null,
                });
            }

            var paginationResult = await RepositoryUtils.PaginateByDateAsync(query, select, request);

            if (request.Filter is not null && request.Filter.IncludePlaceDetails)
            {
                foreach (var post in paginationResult.Result)
                {
                    post.PlaceImageUrl = (await _blobStorageRepository.GetPublicImageUrl(post.PlaceImageUrl)).Item2?.ToString() ?? post.PlaceImageUrl;
                }
            }

            foreach (var post in paginationResult.Result)
            {
                await SetPostImageUrls(post);
            }

            var paginationResultX = new DateTimePaginationResult<PostDetailedDTO>
            {
                CurrentPage = paginationResult.CurrentPage,
                Result = paginationResult.Result.Select(p => (PostDetailedDTO)p).ToList(),
                HasNext = paginationResult.HasNext,
                LastDate = paginationResult.LastDate,
                LastId = paginationResult.LastId,
                Total = paginationResult.Total,
            };


            return paginationResultX;
        }

        private async Task SetPostImageUrls(PostDetailedDTO post)
        {
            post.UserImageUrl = (await _blobStorageRepository.GetPublicImageUrl(post.UserImageUrl)).Item2?.ToString() ?? post.UserImageUrl;
            if (post.MediaUrls != null)
            {
                post.MediaUrls = post.MediaUrls.Select(async m => (await _blobStorageRepository.GetPublicImageUrl(m)).Item2?.ToString()).Select(m => m.Result).Where(m => m is not null).ToArray();
            }
        }

        public async Task<int?> CreateComment(int userId, CommentCreateDTO dto)
        {
            var exists = _context.Posts.Any(post => post.UserId == userId);
            if (!exists)
            {
                return -1;
            }
            var comment = new PostComment
            {
                UserId = userId,
                PostId = dto.PostId,
                Text = dto.Content,
            };
            _context.PostComments.Add(comment);
            await _context.SaveChangesAsync();
            return comment.Id;
        }

        public async Task<ResponseType> DeleteComment(int userId, int commentId)
        {
            _context.PostComments.Remove(_context.PostComments.Where(pc => pc.UserId == userId && pc.Id == commentId).First());
            await _context.SaveChangesAsync();
            return ResponseType.Deleted;
        }

        public async Task<DateTimePaginationResult<CommentDetailedDTO>> GetComments(CommentGetRequest request)
        {
            var query = _context.PostComments
                    .Include(p => p.User)
                    .Include(p => p.CommentLikes)
                    .Where(p => p.PostId == request.PostId)
                    .AsQueryable();

            var select = (IQueryable<PostComment> q) => q.Select(p => new CommentDetailedDTO
            {
                Likes = p.CommentLikes.Where(l => l.IsLike).Select(l => new LikeDetailedDTO
                {
                    UserId = l.UserId,
                    UserName = l.User.FirstName + " " + l.User.LastName
                }).ToList(),
                Text = p.Text,
                UserId = p.UserId,
                UserImageUrl = p.User.ProfileImageUrl,
                UserName = p.User.FirstName + " " + p.User.LastName,
                Id = p.Id
            });

            var result = await RepositoryUtils.PaginateByDateAsync(query, select, request);

            foreach (var dto in result.Result)
            {
                dto.UserImageUrl = (await _blobStorageRepository.GetPublicImageUrl(dto.UserImageUrl)).Item2?.ToString() ?? dto.UserImageUrl;
            }

            return result;
        }

        public async Task LikePost(int userId, int postId)
        {
            await Like(userId, postId, _context.PostLikes, () =>
            _context.PostLikes.Add(new PostLike
            {
                IsLike = true,
                TargetId = postId,
                UserId = userId,
            }));
        }

        public async Task LikeComment(int userId, int commentId)
        {
            await Like(userId, commentId, _context.CommentLikes, () =>
            _context.CommentLikes.Add(new CommentLike
            {
                IsLike = true,
                TargetId = commentId,
                UserId = userId,
            }));
        }

        private async Task Like(int userId, int targetId, IQueryable<LikeBase> query, Action onCreate)
        {
            var existing = await query.Where(l => l.UserId == userId && l.TargetId == targetId).FirstOrDefaultAsync();
            if (existing is null)
            {
                onCreate();
                await _context.SaveChangesAsync();
            }
            else
            {
                existing.IsLike = !existing.IsLike;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CommentDetailedDTO?> GetComment(int commentId)
        {
            var comment = await _context.PostComments
                .Include(p => p.CommentLikes)
                .Include(p => p.User)
                .Where(p => p.Id == commentId)
                .Select(p => new CommentDetailedDTO
                {
                    DateTime = DateTime.Now,
                    Id = p.Id,
                    Likes = p.CommentLikes.Where(p => p.IsLike).Select(p => new LikeDetailedDTO
                    {
                        UserId = p.UserId,
                        UserName = p.User.FirstName + " " + p.User.LastName,
                    }).ToList(),
                    UserName = p.User.FirstName + " " + p.User.LastName,
                    Text = p.Text,
                    UserId = p.UserId,
                    UserImageUrl = p.User.ProfileImageUrl
                })
                .FirstOrDefaultAsync();

            if (comment.UserImageUrl is not null)
            {
                comment.UserImageUrl = (await _blobStorageRepository.GetPublicImageUrl(comment.UserImageUrl)).Item2?.ToString() ?? comment.UserImageUrl;
            }
            return comment;
        }
    }
}
