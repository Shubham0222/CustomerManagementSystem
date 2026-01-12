using CustomerManagementSystem.DbModels;
using CustomerManagementSystem.Infrastructure;
using CustomerManagementSystem.Models;
using Dapper;
using System.Data;

namespace CustomerManagementSystem.Services
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DapperContext _context;

        public CustomerRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerReportVM>> GetCustomersAsync(
            int page, int pageSize, string search)
        {
            var sql = @"
        SELECT c.CustomerID, c.FirstName, c.LastName, c.Email,
               c.Phone, co.CountryName, c.IsActive
        FROM Customers c
        INNER JOIN Countries co ON c.CountryID = co.CountryID
        WHERE (@Search IS NULL OR c.FirstName LIKE '%' + @Search + '%') AND IsActive = 1
        ORDER BY c.CreatedAt
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<CustomerReportVM>(sql, new
            {
                Search = search,
                Offset = (page - 1) * pageSize,
                PageSize = pageSize
            });
        }

        public async Task AddCustomerAsync1(Customer customer)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                "sp_InsertCustomer",
                customer,
                commandType: CommandType.StoredProcedure);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            using var connection = _context.CreateConnection();

            var parameters = new
            {
                customer.FirstName,
                customer.LastName,
                customer.Email,
                customer.Phone,
                customer.CountryID
            };

            await connection.ExecuteAsync(
                "sp_InsertCustomer",
                parameters,
                commandType: CommandType.StoredProcedure);
        }


        public async Task UpdateCustomerAsync(Customer customer)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                "sp_UpdateCustomer",
                customer,
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                "UPDATE Customers SET IsActive = 0 WHERE CustomerID = @Id",
                new { Id = customerId });
        }
    }
}