using TourPlanner.Server.DAL.Repositories;

namespace TourPlanner.Server.BL.API.Services
{
    public interface IRepositoryService
    {
        public IRepository<T>? GetRepository<T>();
    }

    public class MissingRepositoryException : Exception
    {
        public MissingRepositoryException(Type repositoryType) : base(
            $"No {repositoryType.Name} is registered in the repository service!") { }
    }
}
