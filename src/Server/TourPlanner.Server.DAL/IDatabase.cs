using System.Collections.Specialized;
using System.Data;

namespace TourPlanner.Server.DAL
{
    public interface IDatabase : IDisposable
    {
        bool OpenConnection(string connectionString);
        OrderedDictionary SelectSingle(IDbCommand cmd);
        OrderedDictionary[] Select(IDbCommand cmd);
        int ExecuteNonQuery(IDbCommand cmd);
        bool ExecuteNonQueryTransaction(IEnumerable<TransactionObject> objects);
    }
}