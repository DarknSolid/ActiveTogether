using EntityLib;
using EntityLib.Entities.Chatting;
using EntityLib.Entities.Matching;
using Microsoft.EntityFrameworkCore;
using static ModelLib.Repositories.RepositoryEnums;

namespace ModelLib.Repositories.Matching
{
    public interface ILikeRepository
    {
        Task<(ResponseType, bool)> LikeUserAsync(int userId, int likeeId);
        Task<ResponseType> RemoveLikeAsync(int userId, int likerId);
    }
    public class LikeRepository : RepositoryBase, ILikeRepository
    {
        public LikeRepository(IApplicationDbContext context) : base(context) { }

        public async Task<(ResponseType, bool)> LikeUserAsync(int userId, int likeeId)
        {
            var existing = await _context.Likes.Where(l => l.LikerId == userId && l.LikeeId == likeeId ||
            l.LikerId == likeeId && l.LikeeId == userId).FirstOrDefaultAsync();

            if (existing == null)
            {
                var like = new Like
                {
                    IsAccepted = false,
                    LikerId = userId,
                    LikeeId = likeeId,
                    IsIndividualLike = true,
                };
                _context.Likes.Add(like);
                await _context.SaveChangesAsync();
                return (ResponseType.Created, false);

            }

            if (existing.IsAccepted || existing.LikerId == userId)
            {
                return (ResponseType.Duplicate, false);
            }

            existing.IsAccepted = true;
            
            // create match

            var chat = new Chat
            {
                Matches = {
                    new Match {
                        IsIndividualMatch = true,
                        TargetOneId = existing.LikerId,
                        TargetTwoId = existing.LikeeId,
                        }
                },
                ChatMembers = {
                    new ChatMember
                    {
                        UserId = userId,
                    },
                    new ChatMember
                    {
                        UserId = likeeId,
                    }}
            };

            await _context.SaveChangesAsync();
            return (ResponseType.Created, true);
        }

        public async Task<ResponseType> RemoveLikeAsync(int userId, int likeeId)
        {
            var entity = await _context.Likes.Where(l => l.LikerId == userId && l.LikeeId == likeeId).FirstOrDefaultAsync();
            if (entity == null)
            {
                return ResponseType.NotFound;
            }
            _context.Likes.Remove(entity);
            await _context.SaveChangesAsync();
            return ResponseType.Deleted;
        }
    }
}
