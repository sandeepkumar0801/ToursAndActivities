using Microsoft.Extensions.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Isango.Persistence
{
        public class DatabasePersistence 
        {
        private readonly string _isangoLiveDbConnectionString;
        private readonly string _primalidentitiesString;


        public DatabasePersistence(IConfiguration configuration)
        {
            _isangoLiveDbConnectionString = configuration.GetConnectionString("IsangoLiveDB");
            _primalidentitiesString = configuration.GetConnectionString("DefaultConnection");
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_isangoLiveDbConnectionString);
        }
        public SqlCommand GetStoredProcCommand(string procedure, SqlConnection connection)
        {
            return new SqlCommand(procedure, connection);

        }
        public void AddParameterWithValue(DbCommand command, string parameterName, DbType dbType, object value)
        {
            command.Parameters.Add(new SqlParameter
            {
                ParameterName = parameterName,
                DbType = dbType,
                Value = value
            });
        }

        public SqlConnection CreateDefaultConnection()
        {
            return new SqlConnection(_primalidentitiesString);
        }

    }
}
