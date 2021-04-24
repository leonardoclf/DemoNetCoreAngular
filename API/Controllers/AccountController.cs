using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto newUser)
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

            return user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(LoginDto loginDto)
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

            return user;
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

    JSON Web Tokns (JWT)
        RFC 7519 - credentials / claims / other information
*/