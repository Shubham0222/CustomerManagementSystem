using CustomerManagementSystem.Models;

namespace CustomerManagementSystem.Infrastructure
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<CustomerReportVM>> GetCustomersAsync(
            int page, int pageSize, string search);
        Task AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task SoftDeleteCustomerAsync(int customerId);
    }

}
