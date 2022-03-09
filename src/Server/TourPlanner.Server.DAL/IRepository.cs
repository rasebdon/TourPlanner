using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Server.DAL
{
    internal interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T Get(int id);
        bool Insert(T item);
        bool Update(T item);
        bool Delete(T item);
    }
}
