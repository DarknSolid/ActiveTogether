using EntityLib;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.DTOs.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.Repositories
{
    public interface IReviewsRepository
    {
        public Task<(PaginationResult, List<ReviewDetailedDTO>)> GetReviewsAsync(ReviewsDTO request);
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
                    AuthorFirstName = r.Reviewer.FirstName,
                    AuthorLastName = r.Reviewer.LastName,
                    Comment = r.Description,
                    ReviwerId = r.ReviewerId,
                    Date = r.Date,
                    RevieweeId = r.RevieweeId,
                    Rating = r.Rating,
                    Title = r.Title,
                    ReviewType = r.ReviewType
                })
                .ToListAsync();

            return (paginationResult, dtoResult);
        }
    }
}
