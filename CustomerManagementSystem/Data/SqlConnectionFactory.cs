using Microsoft.Data.SqlClient;
using System.Data;

namespace CustomerManagementSystem.Data
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;
        public SqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            var cs = _configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(cs);
        }
    }
}
