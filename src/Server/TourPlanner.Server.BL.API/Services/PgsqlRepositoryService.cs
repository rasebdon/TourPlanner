using Npgsql;
using System.Data;
using TourPlanner.Common.Models;
using TourPlanner.Server.DAL.Repositories;

namespace TourPlanner.Server.BL.API.Services
{
    public class PgsqlRepositoryService : IRepositoryService
    {
        private readonly Dictionary<Type, object> _repositories = new();

        public PgsqlRepositoryService(NpgsqlConnection database)
        {
            // Register repositories
            _repositories = new()
            {
                {
                    typeof(Tour),
                    new PgsqlTourRepository(database)
                },
            };
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
