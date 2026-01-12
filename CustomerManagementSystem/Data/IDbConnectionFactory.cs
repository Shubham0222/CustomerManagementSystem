using System.Data;

namespace CustomerManagementSystem.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
