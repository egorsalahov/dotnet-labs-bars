using RestApiHomework.DTO_s;

namespace RestApiHomework.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> RegisterAsync(RegisterDto dto);
        Task<UserResponseDto?> LoginAsync(LoginDto dto);
        Task<UserResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<UserResponseDto>> GetFilteredAsync(UserFilterDto filter);
        Task<bool> UpdateAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
