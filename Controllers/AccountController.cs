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

        public AccountController(DataContext dataContext, ITokenServices tokenServices)
        {
            _dataContext = dataContext;
            _tokenServices = tokenServices;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName)) return BadRequest("Данное имя пользователя уже занято");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            await _dataContext.AddAsync(user);
            await _dataContext.SaveChangesAsync();
            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenServices.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if (user is null) return Unauthorized("Пользователь не найден");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (var i = 0; i < computedHash.Length; i++)
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Пароль не совпадает");

            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenServices.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string userName) => await _dataContext.Users.AnyAsync(x => x.UserName == userName.ToLower());

    }
}
