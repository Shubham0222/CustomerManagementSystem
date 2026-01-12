using CustomerManagementSystem.Models;
using Dapper;
using System.Data;

namespace CustomerManagementSystem.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _db;

        public UserRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<UserDto?> ValidateUserAsync(string username)
        {
            return await _db.QueryFirstOrDefaultAsync<UserDto>(
                "sp_ValidateUser",
                new { Username = username },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}