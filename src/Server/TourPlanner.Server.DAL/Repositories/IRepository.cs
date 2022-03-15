using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
