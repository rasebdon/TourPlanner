using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Common.Models;
using Npgsql;
using System.Data;
using System.Collections.Specialized;

namespace TourPlanner.Server.DAL.Repositories
{
    public class PgsqlTourRepository : IRepository<Tour>
    {
        private readonly PgsqlDatabase _database;

        public PgsqlTourRepository(PgsqlDatabase database)
        {
            _database = database;
        }

        public bool Delete(int id)
        {
            NpgsqlCommand cmd = new();
            cmd.CommandText = "DELETE FROM tours WHERE id=$id;";
            cmd.Parameters.AddWithValue("id", id);

            return _database.ExecuteNonQuery(cmd) == 1;
        }

        public Tour? Get(int id)
        {
            NpgsqlCommand cmd = new();
            cmd.CommandText = "SELECT * FROM tours WHERE id=$id;";
            cmd.Parameters.AddWithValue("id", id);

            var result = _database.SelectSingle(cmd);

            return ParseFromRow(result);
        }

        private Tour? ParseFromRow(OrderedDictionary result)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tour> GetAll()
        {
            throw new NotImplementedException();
        }

        public bool Insert(ref Tour item)
        {
            // Insert tour points

            // Insert tour entries

            // Insert tour

            throw new NotImplementedException();
        }

        public bool Update(ref Tour item)
        {
            throw new NotImplementedException();
        }

    }
}
