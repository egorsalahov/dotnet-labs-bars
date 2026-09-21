using Microsoft.AspNetCore.Mvc;
using RestApiHomework.DTO_s;
using RestApiHomework.Services;

namespace RestApiHomework.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    //регистрация
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var user = await _userService.RegisterAsync(dto);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //авторизация
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userService.LoginAsync(dto);
        if (user == null)
            return Unauthorized("Неверное имя пользователя или пароль.");

        return Ok(new { Message = "Успешная авторизация!", User = user });
    }


    //get
    [HttpGet]
    public async Task<ActionResult<UserResponseDto>> GetUserById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound("User не найден");

        return Ok(user);
    }

    //get по определенному отрезку времени
    [HttpGet("range")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsersByDateRange([FromQuery] UserFilterDto filter)
    {
        var users = await _userService.GetFilteredAsync(filter);
        return Ok(users);
    }

    //update
    [HttpPut]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        try
        {
            var updated = await _userService.UpdateAsync(id, dto);
            if (!updated) return NotFound("User не найден");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var deleted = await _userService.DeleteAsync(id);
        if (!deleted) return NotFound("User не найден");

        return NoContent();
    }
}