namespace TourPlanner.Server.DAL.Repositories
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T? Get(int id);
        bool Insert(ref T item);
        bool Update(ref T item);
        bool Delete(int id);
    }
}
