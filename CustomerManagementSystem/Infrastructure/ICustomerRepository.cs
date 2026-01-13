using CustomerManagementSystem.Models;

namespace CustomerManagementSystem.Infrastructure
{
    public interface ICustomerRepository
    {
        Task<CustomerReportDto> GetCustomersAsync(
                    int page,
                    int pageSize,
                    string? search,
                    string sortColumn,
                    string sortDirection);
        Task<(bool IsSuccess, string Reason)> AddCustomerAsync(Customer customer);
        Task<bool> UpdateCustomerAsync(Customer customer);
        Task<IEnumerable<Country>> GetCountriesAsync();
        Task DeleteCustomerAsync(int customerId);
    }

}
