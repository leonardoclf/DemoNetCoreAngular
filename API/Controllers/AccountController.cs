using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto newUser)
        {
            if (await UserExists(newUser.Username)) return BadRequest("Username is taken");


            // when finish this class, dispose
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = newUser.Username.ToLower(),
                // need to transform string to byte[]
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(newUser.Password)),
                PasswordSalt = hmac.Key
            };
            // Add - begins the tracking with EntityFramework
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(res => res.UserName == loginDto.Username); //only return one res, more than one error

            if (user == null) return Unauthorized("Invalid username");

            //need to pass the salt if not it will generate another key and won't match
            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // its a bitarray..loop 
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        // helper method
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(res => res.UserName == username.ToLower());
        }
    }
}

/*
    when send something via body, needs to be an object
    DTO - data transfer object -> hide / flat obj / avoid circular ref
    RegisterDto - é o objeto que está vindo do body

    JSON Web Tokens (JWT)
        RFC 7519 - credentials / claims / other information
        Browser storage holds token
        No session to manage - JWT are self contained tokens
        Portable - single token can be used with multiple backends
        No cookies required - mobile friendly
        Performance - once a token is issued, there is no to make another query to db
*/