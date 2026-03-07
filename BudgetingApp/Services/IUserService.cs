using BudgetingApp.DTOs;

namespace BudgetingApp.Services
{
    public interface IUserService
    {
        Task<UserDTO?> GetUserByIdAsync(Guid id);
        Task<UserDTO> CreateUserAsync(CreateUserDTO createUserDto);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
    }
}
