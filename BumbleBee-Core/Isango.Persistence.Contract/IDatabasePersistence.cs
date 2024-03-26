using System.Data;

namespace Isango.Persistence
{
    public interface IDatabasePersistence
    {
        IDbConnection GetConnection();
        IDbCommand CreateCommand(string commandText, CommandType commandType = CommandType.StoredProcedure);
        // Add other methods for executing queries, transactions, etc.
    }
}
