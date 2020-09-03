using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace PacklistsWeb.API.Repositories
{
    public class RepositoryBase
    {
        private readonly IConfiguration _config;

        internal IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public RepositoryBase(IConfiguration config)
        {
            _config = config;
        }
    }
}