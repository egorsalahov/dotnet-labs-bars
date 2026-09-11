using Microsoft.AspNetCore.Mvc;
using RestApiHomework.Data;
using RestApiHomework.DTO_s;
using RestApiHomework.Models;
using Microsoft.EntityFrameworkCore;


namespace RestApiHomework.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        //регистрация
        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("User с таким именем уже есть");

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("User с таким емайлом уже есть");

            //хеш
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new UserResponseDto(user.Id, user.Username, user.Email);

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, response);
        }

        //авторизация
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized("Такого User нет");
            }

            return Ok(new { Message = "Успешная авторизация!", UserId = user.Id, user.Username });
        }

        //get
        [HttpGet]
        public async Task<ActionResult<UserResponseDto>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User не найден");

            return new UserResponseDto(user.Id, user.Username, user.Email);
        }


        //update
        [HttpPut]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return NotFound("User не найден");

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username && u.Id != id))
                return BadRequest("Имя занято");

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id))
                return BadRequest("Почта занята");

            user.Username = dto.Username;
            user.Email = dto.Email;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //delete
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return NotFound("Пользователь не найден.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
