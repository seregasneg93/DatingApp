using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Exstensions;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _dataContext;

        public LikesRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _dataContext.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PagesList<LikesDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _dataContext.Users.OrderBy(u=>u.UserName).AsQueryable();
            var likes = _dataContext.Likes.AsQueryable();

            if(likesParams.Predicate == "liked")
            {
                likes = likes.Where(x => x.SourceUserId == likesParams.UserId);
                users = likes.Select(x => x.LikedUser);
            }

            if(likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(x => x.LikedUserId == likesParams.UserId);
                users = likes.Select(x => x.SourceUser);
            }

            var likedUser = users.Select(users => new LikesDto
            {
                UserName = users.UserName,
                KnowsAs = users.KnownAs,
                Age = users.DateBurth.CalculateAge(),
                PhotoUrl = users.Photos.FirstOrDefault(p=>p.IsMain).Url,
                City = users.City,
                Id = users.Id
            });

            return await PagesList<LikesDto>.CreateAsync(likedUser,likesParams.PageNumber,likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _dataContext.Users
                                     .Include(x => x.LikedUsers)
                                     .FirstOrDefaultAsync(x => x.Id == userId);
                            
        }
    }
}
