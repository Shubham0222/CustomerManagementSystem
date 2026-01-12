using CustomerManagementSystem.Models;

public interface IUserRepository
{
    Task<UserDto?> ValidateUserAsync(string username);
}
