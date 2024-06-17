using EntityLib;
using EntityLib.Entities;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.DTOs.Reviews;
using static EntityLib.Entities.Enums;
using static ModelLib.Repositories.RepositoryEnums;

namespace ModelLib.Repositories
{
    public interface IReviewsRepository
    {
        public Task<PaginationResult<ReviewDetailedDTO>> GetReviewsAsync(ReviewsDTOPaginationRequest request);
        public Task<(ResponseType, ReviewCreatedDTO)> CreateReviewAsync(int reviewee, ReviewCreateDTO dto);

        public Task<ReviewDetailedDTO?> GetReviewAsync(int userId, int placeId);
    }

    public class ReviewsRepository : RepositoryBase, IReviewsRepository
    {
        private readonly ICheckInRepository _checkInRepository;
        private readonly IBlobStorageRepository _blobStorageRepository;

        public ReviewsRepository(IApplicationDbContext context, ICheckInRepository checkInRepository, IBlobStorageRepository blobStorageRepository) : base(context)
        {
            _checkInRepository = checkInRepository;
            _blobStorageRepository = blobStorageRepository;
        }
        public async Task<PaginationResult<ReviewDetailedDTO>> GetReviewsAsync(ReviewsDTOPaginationRequest request)
        {
            var query = _context.Reviews
                .Include(r => r.User)
                .Where(r => r.PlaceId == request.PlaceId);

            var select = (IQueryable<Review> q) => q.Select(r => new ReviewDetailedDTO
            {
                ReviewerFirstName = r.User.FirstName,
                ReviewerLastName = r.User.LastName,
                UserId = r.UserId,
                Description = r.Description,
                ReviewerId = r.UserId,
                DateTime = r.DateTime,
                Id = r.PlaceId,
                Rating = r.Rating,
                Title = r.Title,
                ProfilePictureUrl = r.User.ProfileImageUrl
            });

            var paginationResult= await RepositoryUtils.PaginateByDateAsync(query, select, request.PaginationRequest);

            foreach( var dto in paginationResult.Result)
            {
                var (_, url) = await _blobStorageRepository.GetPublicImageUrl(dto.ProfilePictureUrl);
                if (url != null )
                {
                    dto.ProfilePictureUrl = url.ToString();
                }
            }

            return paginationResult;
        }

        public async Task<(ResponseType, ReviewCreatedDTO?)> CreateReviewAsync(int userId, ReviewCreateDTO dto)
        {
            // If the place doesn't exist:
            var place = await _context.Places.FirstOrDefaultAsync(p => p.Id == dto.Id);
            if (place is null)
            {
                return new(ResponseType.NotFound, null);
            }

            if (place.FacilityType == FacilityType.DogPark &&
                !await _checkInRepository.HasCheckedOutBeforeAsync(userId, dto.Id))
            {
                return (ResponseType.Conflict, null);
            }

            var entity = await _context.Reviews
                .Where(r =>
                    r.UserId == userId &&
                    r.PlaceId == dto.Id
                )
                .FirstOrDefaultAsync();


            ResponseType responseType;

            if (entity is null)
            {
                responseType = ResponseType.Created;
                entity = new Review()
                {
                    UserId = userId,
                    PlaceId = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    Rating = dto.Rating,
                    DateTime = DateTime.UtcNow,
                };
                _context.Reviews.Add(entity);
            }
            else
            {
                responseType = ResponseType.Updated;
                entity.Title = dto.Title;
                entity.Description = dto.Description;
                entity.Rating = dto.Rating;
                entity.DateTime = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return (responseType, new ReviewCreatedDTO
            {
                PlaceId = entity.PlaceId,
                UserId = entity.UserId,
            });
        }

        public async Task<ReviewDetailedDTO?> GetReviewAsync(int userId, int placeId)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.UserId == userId &&  r.PlaceId == placeId)
                .Select(r => new ReviewDetailedDTO
                {
                    DateTime = r.DateTime,
                    Description = r.Description,
                    Rating = r.Rating,
                    Title = r.Title,
                    ReviewerFirstName = r.User.FirstName,
                    ReviewerLastName = r.User.LastName,
                    UserId = r.UserId,
                    ProfilePictureUrl = r.User.ProfileImageUrl
                }).FirstOrDefaultAsync();

            if (review is not null)
            {
                var (_, url) = await _blobStorageRepository.GetPublicImageUrl(review.ProfilePictureUrl);
                if (url != null)
                {
                    review.ProfilePictureUrl = url.ToString();
                }
            }
            return review;
        }
    }
}
