using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public UserRepository(DataContext dataContext,IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string userName)
        {
            return await _dataContext.Users
                                .Where(x=>x.UserName == userName)
                                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                                .SingleOrDefaultAsync();
        }

        public async Task<PagesList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _dataContext.Users.AsQueryable();

            query = query.Where(x => x.UserName != userParams.CurrentUserName);
            query = query.Where(x => x.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(x => x.DateBurth >= minDob && x.DateBurth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x=>x.Created),
                _ => query.OrderByDescending(x=>x.LastActive)
            };

            return await PagesList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking()
                                                         ,userParams.PageNumber,userParams.PageSize);
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
           return await _dataContext.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string userName)
        {
           return await _dataContext.Users.SingleOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _dataContext.Users
                                     .Include(x=>x.Photos)
                                     .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public void Update(AppUser appUser)
        {
           _dataContext.Entry(appUser).State = EntityState.Modified;
        }
    }
}
