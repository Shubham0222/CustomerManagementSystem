using CustomerManagementSystem.DbModels;
using CustomerManagementSystem.Models;
using CustomerManagementSystem.Utility;
using Dapper;
using System.Data;

namespace CustomerManagementSystem.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<UserDto?> ValidateUserAsync(string username)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<UserDto>(
                "sp_ValidateUser",
                new { Username = username, },
                commandType: CommandType.StoredProcedure
            );

        }
    }
}