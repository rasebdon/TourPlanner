using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Server.DAL.Repositories;

namespace TourPlanner.Server.DAL.Configuration
{
    public interface IConfiguration
    {
        public bool TryGetObject(string name, out object? obj);
        public void AddObject(string name, object value);
        public void RemoveObject(string name);
        public void UpdateObject(string name, object value);
    }
}
