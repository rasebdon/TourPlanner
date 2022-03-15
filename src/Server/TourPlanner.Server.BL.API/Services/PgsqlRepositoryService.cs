using Npgsql;
using System.Data;
using TourPlanner.Common.Models;
using TourPlanner.Server.DAL;
using TourPlanner.Server.DAL.Repositories;

namespace TourPlanner.Server.BL.API.Services
{
    public class PgsqlRepositoryService : IRepositoryService, IDisposable
    {
        private readonly Dictionary<Type, object> _repositories = new();
        private readonly PgsqlDatabase _database;

        public PgsqlRepositoryService()
        {
            _database = new PgsqlDatabase("CONNECTION_STRING");

            if (!_database.OpenConnection())
                throw new Exception("Could not connect to database!");

            // Register repositories
            _repositories = new()
            {
                {
                    typeof(Tour),
                    new PgsqlTourRepository(_database)
                },
            };
        }

        public void Dispose()
        {
            _database.Dispose();
        }

        public IRepository<T>? GetRepository<T>()
        {
            if(_repositories.TryGetValue(typeof(T), out object? repository))
            {
                return repository as IRepository<T>;
            }
            return null;
        }
    }
}
