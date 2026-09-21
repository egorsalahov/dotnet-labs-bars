using Microsoft.EntityFrameworkCore;
using RestApiHomework.Data;
using RestApiHomework.DTO_s;
using RestApiHomework.Models;

namespace RestApiHomework.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            throw new InvalidOperationException("Пользователь с таким именем уже есть");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new InvalidOperationException("Пользователь с такой почтой уже есть");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return MapToResponse(user);
    }

    public async Task<UserResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        return MapToResponse(user);
    }

    public async Task<UserResponseDto?> GetByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user == null ? null : MapToResponse(user);
    }

    //Get всех пользователей
    public async Task<IEnumerable<UserResponseDto>> GetFilteredAsync(UserFilterDto filter)
    {
        var query = _context.Users.AsNoTracking().AsQueryable();

        if (filter.CreatedFrom.HasValue)
            query = query.Where(u => u.CreatedAt >= filter.CreatedFrom.Value);

        if (filter.CreatedTo.HasValue)
            query = query.Where(u => u.CreatedAt <= filter.CreatedTo.Value);

        if (filter.UpdatedFrom.HasValue)
            query = query.Where(u => u.UpdatedAt >= filter.UpdatedFrom.Value);

        if (filter.UpdatedTo.HasValue)
            query = query.Where(u => u.UpdatedAt <= filter.UpdatedTo.Value);

        var users = await query.ToListAsync();
        return users.Select(MapToResponse);
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        if (await _context.Users.AnyAsync(u => u.Username == dto.Username && u.Id != id))
            throw new InvalidOperationException("Такое имя уже зарегестрировано");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id))
            throw new InvalidOperationException("Такая почта уже зарегестрирована");

        user.Username = dto.Username;
        user.Email = dto.Email;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }

    private static UserResponseDto MapToResponse(User user) =>
        new(user.Id, user.Username, user.Email, user.CreatedAt, user.UpdatedAt);
}