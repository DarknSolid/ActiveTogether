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
                .Include(r => r.User)
                .Where(r => r.PlaceId == request.PlaceId)
                .OrderByDescending(r => r.Date);

            var (paginationResult, paginatedQuery) = await RepositoryUtils.GetPaginationQuery(query, request.PaginationRequest);

            var dtoResult = await paginatedQuery.Select(r => new ReviewDetailedDTO
            {
                ReviewerFirstName = r.User.FirstName,
                ReviewerLastName = r.User.LastName,
                Description = r.Description,
                ReviewerId = r.UserId,
                Date = r.Date,
                PlaceId = r.PlaceId,
                Rating = r.Rating,
                Title = r.Title,
            })
            .ToListAsync();

            return (paginationResult, dtoResult);
        }

        public async Task<(ResponseType, ReviewCreatedDTO)> CreateReviewAsync(int reviewerId, ReviewCreateDTO dto)
        {
            if (!await _context.Places.AnyAsync(p => p.Id == dto.PlaceId))
            {
                return new
                    (
                        ResponseType.NotFound,
                        null
                    );
            }

            var entity = await _context.Reviews
                .Where(r =>
                    r.UserId == reviewerId &&
                    r.PlaceId == dto.PlaceId
                )
                .FirstOrDefaultAsync();


            var responseType = ResponseType.Conflict;

            if (entity == null)
            {
                responseType = ResponseType.Created;
                entity = new Review()
                {
                    UserId = reviewerId,
                    PlaceId = dto.PlaceId,
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
                RevieweeId = entity.PlaceId,
                ReviewerId = entity.UserId,
            });
        }
    }
}
