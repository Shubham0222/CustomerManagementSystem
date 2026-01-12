using CustomerManagementSystem.Models;

namespace CustomerManagementSystem.Infrastructure
{
    public interface ICustomerRepository
    {
        Task<CustomerReportDto> GetCustomersAsync(
            int page, int pageSize, string search);
        Task<(bool IsSuccess, string Reason)> AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int customerId);
    }

}
