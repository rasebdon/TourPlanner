using Npgsql;
using System.Collections.Specialized;
using System.Data;

namespace TourPlanner.Server.DAL
{
    /// <summary>
    /// Class for managing the general database communication
    /// </summary>
    public class PgsqlDatabase : IDatabase, IDisposable
    {
        private bool _disposed;

        private readonly IDbConnection _connection;
        private readonly object _connectionLock = new();
        private readonly object _transactionLock = new();

        public PgsqlDatabase(string connectionString)
        {
            _disposed = false;

            // Connect to postgresql database
            _connection = new NpgsqlConnection(connectionString);
        }

        /// <summary>
        /// Opens the database connection
        /// </summary>
        public bool OpenConnection()
        {
            try
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                if (_connection.State == ConnectionState.Open)
                    return false;

                _connection.Open();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the first result of the given sql select query
        /// </summary>
        /// <param name="sql">The sql query</param>
        /// <returns>The first row of the result or an empty collection if no results where found</returns>
        public OrderedDictionary SelectSingle(IDbCommand cmd)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            // Execute sql
            OrderedDictionary[] results = Select(cmd);

            // Return data
            if (results.Length > 0)
                return results[0];
            return new OrderedDictionary();
        }

        /// <summary>
        /// Returns all results of the given sql select query
        /// </summary>
        /// <param name="sql">The sql query</param>
        /// <returns>All rows of the result or null if no results where found</returns>
        public OrderedDictionary[] Select(IDbCommand cmd)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            cmd.Connection = this._connection;

            List<OrderedDictionary> rows = new();

            lock (_connectionLock)
            {
                // Execute query
                var rdr = cmd.ExecuteReader();

                // Run reader over results
                while (rdr.Read())
                {
                    // Parse data into dictionaries
                    OrderedDictionary row = new();
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        row.Add(rdr.GetName(i), rdr[i]);
                    }
                    rows.Add(row);
                }

                // Close reader and return results
                rdr.Close();
            }

            return rows.ToArray();
        }

        /// <summary>
        /// Executes the given sql statement and returns the number of rows that it affected.
        /// </summary>
        /// <param name="cmd">The command that will be executed</param>
        /// <returns>Number of rows affected</returns>
        public int ExecuteNonQuery(IDbCommand cmd)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            cmd.Connection = this._connection;
            int rowsAffected;

            lock (_connectionLock)
            {
                rowsAffected = cmd.ExecuteNonQuery();
            }
            return rowsAffected;
        }


        public bool ExecuteNonQueryTransaction(IEnumerable<TransactionObject> objects)
        {
            bool success = true;

            lock (_transactionLock)
            {
                IDbTransaction transaction = _connection.BeginTransaction();
                try
                {
                    foreach (TransactionObject obj in objects)
                    {
                        obj.Command.Transaction = transaction;
                        obj.Command.Connection = _connection;

                        if (obj.Command.ExecuteNonQuery() != obj.ExpectedAffectedRows)
                        {
                            throw new Exception("ExpectedAffectedRows does not match!");
                        }
                    }

                    if (success)
                        transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    success = false;
                }
            }

            return success;
        }

        public void Dispose()
        {
            _disposed = true;
            lock (_connectionLock) _connection.Dispose();
        }
    }
}
