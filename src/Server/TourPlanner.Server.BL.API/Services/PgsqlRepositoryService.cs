using Npgsql;
using System.Data;
using TourPlanner.Common.Models;
using TourPlanner.Server.DAL;
using TourPlanner.Server.DAL.Repositories;
using TourPlanner.Server.DAL.Repositories.Pgsql;

namespace TourPlanner.Server.BL.API.Services
{
    public class PgsqlRepositoryService : IRepositoryService, IDisposable
    {
        private readonly Dictionary<Type, object> _repositories = new();
        private readonly PgsqlDatabase _database;

        public PgsqlRepositoryService()
        {
            _database = new PgsqlDatabase(
                "Server = 127.0.0.1; Port = 5432; Database = tour_planner; User Id = tour_planner_admin; Password = tour_planner_1234;");

            if (!_database.OpenConnection())
                throw new Exception("Could not connect to database!");


            var tourEntryRepo = new PgsqlTourEntryRepository(_database);

            // Register repositories
            _repositories = new()
            {
                {
                    typeof(TourEntry),
                    tourEntryRepo
                },
                {
                    typeof(Tour),
                    new PgsqlTourRepository(_database, tourEntryRepo)
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
