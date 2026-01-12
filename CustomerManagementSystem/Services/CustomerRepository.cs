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

        public async Task<CustomerReportDto> GetCustomersAsync(
    int page, int pageSize, string search)
        {
            var sql = @"
                    SELECT COUNT(*)
                    FROM Customers
                    WHERE (@Search IS NULL OR FirstName LIKE '%' + @Search + '%')
                          AND IsActive = 1;

                    SELECT c.CustomerID, c.FirstName, c.LastName, c.Email,
                           c.Phone, co.CountryName, c.IsActive
                    FROM Customers c
                    INNER JOIN Countries co ON c.CountryID = co.CountryID
                    WHERE (@Search IS NULL OR c.FirstName LIKE '%' + @Search + '%' OR c.LastName LIKE '%' + @Search + '%' OR c.Email LIKE '%' + @Search + '%' OR c.Phone LIKE '%' + @Search + '%' )
                          AND c.IsActive = 1
                    ORDER BY c.CreatedAt
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                    ";

            using var connection = _context.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(sql, new
            {
                Search = search,
                Offset = (page - 1) * pageSize,
                PageSize = pageSize
            });

            var totalRecords = await multi.ReadFirstAsync<int>();
            var customers = (await multi.ReadAsync<CustomerReportVM>()).ToList();

            return new CustomerReportDto
            {
                Customers = customers,
                CurrentPage = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Search = search
            };
        }


        public async Task AddCustomerAsync1(Customer customer)
        {
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                "sp_InsertCustomer",
                customer,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<(bool IsSuccess, string Reason)> AddCustomerAsync(Customer customer)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@FirstName", customer.FirstName);
            parameters.Add("@LastName", customer.LastName);
            parameters.Add("@Email", customer.Email);
            parameters.Add("@Phone", customer.Phone);
            parameters.Add("@CountryID", customer.CountryID);
            parameters.Add("@ResultMessage", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "sp_InsertCustomer",
                parameters,
                commandType: CommandType.StoredProcedure);

            var message = parameters.Get<string>("@ResultMessage");

            return (message == "Success", message);
        }



        public async Task UpdateCustomerAsync(Customer customer)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var parameters = new
                {
                    customer.CustomerID,
                    customer.FirstName,
                    customer.LastName,
                    customer.Email,
                    customer.Phone,
                    customer.CountryID,
                };


                await connection.ExecuteAsync(
                   "sp_UpdateCustomer",
                   parameters,
                   commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {

                throw;
            }
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