using AutoMapper;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly DataContext _dataContext;
        private readonly ITokenServices _tokenServices;
        private readonly IMapper _mapper;

        public AccountController(DataContext dataContext, ITokenServices tokenServices,IMapper mapper)
        {
            _dataContext = dataContext;
            _tokenServices = tokenServices;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName)) return BadRequest("Данное имя пользователя уже занято");

            var user = _mapper.Map<AppUser>(registerDto);

            using var hmac = new HMACSHA512();


            user.UserName = registerDto.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            await _dataContext.AddAsync(user);
            await _dataContext.SaveChangesAsync();
            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenServices.CreateToken(user),
              //  PhotoUrl = user?.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender,
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _dataContext.Users
                                        .Include(x=>x.Photos)
                                        .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if (user is null) return Unauthorized("Пользователь не найден");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (var i = 0; i < computedHash.Length; i++)
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Пароль не совпадает");

            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenServices.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url
            };
        }

        private async Task<bool> UserExists(string userName) => await _dataContext.Users.AnyAsync(x => x.UserName == userName.ToLower());

    }
}
