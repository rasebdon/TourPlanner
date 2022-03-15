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

            // Get base tour data
            var result = _database.SelectSingle(cmd);

            // Get tour entries

            // Get tour points

            return null;
        }

        private Tour? ParseFromRow(
            OrderedDictionary tourData,
            OrderedDictionary[] tourEntries,
            OrderedDictionary[] tourPoints)
        {
            try
            {

                // Parse the tour points to a collection of tour point objects
                Dictionary<int, TourPoint> points = new();
                foreach (var point in tourPoints)
                {
                    int id = int.Parse(point["id"]?.ToString() ?? "");

                    points.Add(
                        id,
                        new()
                        {
                            Id = id,
                            Latitude = long.Parse(point["latidute"]?.ToString() ?? ""),
                            Longitude = long.Parse(point["longitude"]?.ToString() ?? ""),
                        });
                }

                // Parse the tour entries to a collection of tour entries
                List<TourEntry> entries = new();
                foreach (var entry in tourEntries)
                {
                    entries.Add(
                        new()
                        {
                            Id = int.Parse(entry["id"]?.ToString() ?? ""),
                            Date = DateTime.Parse(entry["date"]?.ToString() ?? ""),
                            Distance = float.Parse(entry["distance"]?.ToString() ?? ""),
                            Duration = float.Parse(entry["duration"]?.ToString() ?? ""),
                            StartPoint = points[int.Parse(entry["start_point"]?.ToString() ?? "")],
                            EndPoint = points[int.Parse(entry["end_point"]?.ToString() ?? "")],
                        });
                }

                // Build tour from rest of data
                return new()
                {
                    Id = int.Parse(tourData["id"]?.ToString() ?? ""),
                    Name = tourData["name"]?.ToString(),
                    Description = tourData["description"]?.ToString(),
                    Entries = entries,
                };
            }
            catch (Exception)
            {
                return null;
            }
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
