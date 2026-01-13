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
            int page,
            int pageSize,
            string? search,
            string sortColumn,
            string sortDirection)

        {
            var sql = @"
                SELECT COUNT(*)
                FROM Customers
                WHERE (@Search IS NULL 
                       OR FirstName LIKE '%' + @Search + '%'
                       OR LastName LIKE '%' + @Search + '%'
                       OR Email LIKE '%' + @Search + '%'
                       OR Phone LIKE '%' + @Search + '%')
                AND IsActive = 1;
                
                SELECT c.CustomerID, c.FirstName, c.LastName, c.Email,
                       c.Phone, co.CountryID, co.CountryName, c.IsActive
                FROM Customers c
                INNER JOIN Countries co ON c.CountryID = co.CountryID
                WHERE (@Search IS NULL 
                       OR c.FirstName LIKE '%' + @Search + '%'
                       OR c.LastName LIKE '%' + @Search + '%'
                       OR c.Email LIKE '%' + @Search + '%'
                       OR c.Phone LIKE '%' + @Search + '%')
                AND c.IsActive = 1
                ORDER BY
                    CASE WHEN @SortColumn = 'FirstName' AND @SortDirection = 'ASC' THEN c.FirstName END ASC,
                    CASE WHEN @SortColumn = 'FirstName' AND @SortDirection = 'DESC' THEN c.FirstName END DESC,
                
                    CASE WHEN @SortColumn = 'LastName' AND @SortDirection = 'ASC' THEN c.LastName END ASC,
                    CASE WHEN @SortColumn = 'LastName' AND @SortDirection = 'DESC' THEN c.LastName END DESC,
                
                    CASE WHEN @SortColumn = 'Email' AND @SortDirection = 'ASC' THEN c.Email END ASC,
                    CASE WHEN @SortColumn = 'Email' AND @SortDirection = 'DESC' THEN c.Email END DESC,
                
                    CASE WHEN @SortColumn = 'Phone' AND @SortDirection = 'ASC' THEN c.Phone END ASC,
                    CASE WHEN @SortColumn = 'Phone' AND @SortDirection = 'DESC' THEN c.Phone END DESC,

                    CASE WHEN @SortColumn = 'CountryName' AND @SortDirection = 'ASC' THEN co.CountryName END ASC,
                    CASE WHEN @SortColumn = 'CountryName' AND @SortDirection = 'DESC' THEN co.CountryName END DESC,
                
                    c.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                ";


            using var connection = _context.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(sql, new
            {
                Search = search,
                Offset = (page - 1) * pageSize,
                PageSize = pageSize,
                SortColumn = sortColumn,
                SortDirection = sortDirection
            });


            var totalRecords = await multi.ReadFirstAsync<int>();
            var customers = (await multi.ReadAsync<CustomerReportVM>()).ToList();

            return new CustomerReportDto
            {
                Customers = customers,
                PageNumber = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Search = search
            };
        }
        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            using var connection = _context.CreateConnection();

            var sql = "SELECT CountryID, CountryName FROM Countries ORDER BY CountryName";

            return await connection.QueryAsync<Country>(sql);
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



        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            using var connection = _context.CreateConnection();

            var rows = await connection.ExecuteAsync(
                "sp_UpdateCustomer",
                new
                {
                    customer.CustomerID,
                    customer.FirstName,
                    customer.LastName,
                    customer.Email,
                    customer.Phone,
                    customer.CountryID,
                },
                commandType: CommandType.StoredProcedure);

            return rows > 0;
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