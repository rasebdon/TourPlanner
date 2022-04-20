using Microsoft.Extensions.Logging;
using Npgsql;
using System.Collections.Specialized;
using System.Data;
using TourPlanner.Server.DAL.Configuration;

namespace TourPlanner.Server.DAL
{
    /// <summary>
    /// Class for managing the general database communication
    /// </summary>
    public class PgsqlDatabase : IDatabase, IDisposable
    {
        private bool _disposed;

        private IDbConnection? _connection = null;
        private readonly object _connectionLock = new();
        private readonly object _transactionLock = new();
        private readonly ILogger<PgsqlDatabase> _logger;
        private readonly IConfiguration _configuration;

        public PgsqlDatabase(
            ILogger<PgsqlDatabase> logger,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _disposed = false;

            if(!OpenConnection(GetConnectionString()))
                _logger.LogError("Connection to database failed!");
        }

        private string GetConnectionString()
        {
            string connectionString = "";
            int fails = 0;
            if (_configuration.TryGetObject("database_address", out var dbAddress))
                connectionString += $"Server = {dbAddress};";
            else
            {
                _configuration.AddObject("database_address", "127.0.0.1");
                fails++;
            }

            if (_configuration.TryGetObject("database_port", out var port))
                connectionString += $"Port = {port};";
            else
            {
                _configuration.AddObject("database_port", 5432);
                fails++;
            }

            if (_configuration.TryGetObject("database_name", out var name))
                connectionString += $"Database = {name};";
            else
                {
                _configuration.AddObject("database_name", "tour_planner");
                fails++;
            }

            if (_configuration.TryGetObject("database_username", out var username))
                connectionString += $"Username = {username};";
            else
                {
                _configuration.AddObject("database_username", "tour_planner_admin");
                fails++;
            }

            if (_configuration.TryGetObject("database_password", out var password))
                connectionString += $"Password = {password};";
            else
                {
                _configuration.AddObject("database_password", "tour_planner_1234");
                fails++;
            }

            if (fails > 0)
                return GetConnectionString();
            else 
                return connectionString;
        }

        /// <summary>
        /// Opens the database connection
        /// </summary>
        public bool OpenConnection(string connectionString)
        {
            try
            {
                // Connect to postgresql database
                _connection = new NpgsqlConnection(connectionString);

                if (_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                if (_connection.State == ConnectionState.Open)
                    return false;

                _connection.Open();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
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
            try
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                // Execute sql
                OrderedDictionary[] results = Select(cmd);

                // Return data
                if (results.Length > 0)
                    return results[0];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return new OrderedDictionary();
        }

        /// <summary>
        /// Returns all results of the given sql select query
        /// </summary>
        /// <param name="sql">The sql query</param>
        /// <returns>All rows of the result or null if no results where found</returns>
        public OrderedDictionary[] Select(IDbCommand cmd)
        {
            try
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
                    return rows.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return Array.Empty<OrderedDictionary>();
        }

        /// <summary>
        /// Executes the given sql statement and returns the number of rows that it affected.
        /// </summary>
        /// <param name="cmd">The command that will be executed</param>
        /// <returns>Number of rows affected</returns>
        public int ExecuteNonQuery(IDbCommand cmd)
        {
            try
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
            catch (Exception ex) 
            { 
                _logger.LogError(ex.ToString());
            }
            return 0;
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
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
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
