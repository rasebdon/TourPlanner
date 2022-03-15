using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Common.Models;
using Npgsql;
using System.Data;

namespace TourPlanner.Server.DAL.Repositories
{
    public class PgsqlTourRepository : IRepository<Tour>
    {
        private readonly NpgsqlConnection _database;

        public PgsqlTourRepository(NpgsqlConnection database)
        {
            _database = database;
        }

        public bool Delete(int id)
        {
            NpgsqlCommand cmd = new();
            cmd.Connection = _database;
            cmd.CommandText = "DELETE FROM tours WHERE id=$id;";
            cmd.Parameters.AddWithValue("id", id);

            return cmd.ExecuteNonQuery() == 1;
        }

        public Tour? Get(int id)
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
