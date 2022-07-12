using EntityLib;
using EntityLib.Entities;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.DTOs.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EntityLib.Entities.Enums;
using static ModelLib.Repositories.RepositoryEnums;

namespace ModelLib.Repositories
{
    public interface IReviewsRepository
    {
        public Task<(PaginationResult, List<ReviewDetailedDTO>)> GetReviewsAsync(ReviewsDTO request);
        public Task<(ResponseType, ReviewCreatedDTO)> CreateReviewAsync(int reviewee, ReviewCreateDTO dto);
    }

    public class ReviewsRepository : IReviewsRepository
    {
        private readonly IApplicationDbContext _context;

        public ReviewsRepository(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<(PaginationResult, List<ReviewDetailedDTO>)> GetReviewsAsync(ReviewsDTO request)
        {
            var query = _context.Reviews
                .Include(r => r.Reviewer)
                .Where(r => r.RevieweeId == request.RevieweeId && r.ReviewType == request.ReviewType)
                .OrderByDescending(r => r.Date);

            var (paginationResult, paginatedQuery) = await RepositoryUtils.GetPaginationQuery(query, request.PaginationRequest);

            var dtoResult = await paginatedQuery.Select(r => new ReviewDetailedDTO
            {
                ReviewerFirstName = r.Reviewer.FirstName,
                ReviewerLastName = r.Reviewer.LastName,
                Description = r.Description,
                ReviewerId = r.ReviewerId,
                Date = r.Date,
                RevieweeId = r.RevieweeId,
                Rating = r.Rating,
                Title = r.Title,
                ReviewType = r.ReviewType
            })
                .ToListAsync();

            return (paginationResult, dtoResult);
        }

        public async Task<(ResponseType, ReviewCreatedDTO)> CreateReviewAsync(int reviewerId, ReviewCreateDTO dto)
        {
            var entity = await _context.Reviews.Where(r =>
                                            r.ReviewerId == reviewerId &&
                                            r.RevieweeId == dto.RevieweeId &&
                                            r.ReviewType == dto.ReviewType)
                .FirstOrDefaultAsync();

            if (!await RevieweeExists(dto.RevieweeId, dto.ReviewType))
            {
                return new
                    (
                        ResponseType.NotFound,
                        null
                    );
            }

            var responseType = ResponseType.Conflict;

            if (entity == null)
            {
                responseType = ResponseType.Created;
                entity = new Review()
                {
                    ReviewerId = reviewerId,
                    RevieweeId = dto.RevieweeId,
                    ReviewType = dto.ReviewType,
                    Title = dto.Title,
                    Description = dto.Description,
                    Rating = dto.Rating,
                    Date = DateTime.UtcNow,
                };
                _context.Reviews.Add(entity);
            }
            else
            {
                responseType = ResponseType.Updated;
                entity.Title = dto.Title;
                entity.Description = dto.Description;
                entity.Rating = dto.Rating;
                entity.Date = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return (responseType ,new ReviewCreatedDTO
            {
                RevieweeId = entity.RevieweeId,
                ReviewerId = entity.ReviewerId,
                ReviewType = entity.ReviewType
            });
        }

        private async Task<bool> RevieweeExists(int revieweeId, ReviewType reviewType)
        {
            IQueryable<SimplePrimaryKey> query;
            switch (reviewType)
            {
                case ReviewType.DogPark:
                    query = _context.DogParks;
                    break;
                default:
                    throw new NotImplementedException($"Review type: {reviewType}");
            }

            return await query.FirstOrDefaultAsync(x => x.Id == revieweeId) != null;
        }

    }
}
