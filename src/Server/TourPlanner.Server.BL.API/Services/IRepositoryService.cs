using TourPlanner.Server.DAL.Repositories;

namespace TourPlanner.Server.BL.API.Services
{
    public interface IRepositoryService
    {
        public IRepository<T>? GetRepository<T>();
    }
}
