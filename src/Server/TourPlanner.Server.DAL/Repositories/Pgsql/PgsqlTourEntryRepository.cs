using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Common.Models;

namespace TourPlanner.Server.DAL.Repositories.Pgsql
{
    public class PgsqlTourEntryRepository : IRepository<TourEntry>
    {
        private readonly PgsqlDatabase _database;

        public PgsqlTourEntryRepository(PgsqlDatabase database)
        {
            _database = database;
        }

        public bool Delete(int id)
        {
            NpgsqlCommand cmd = new();
            cmd.CommandText = "DELETE FROM tour_entries WHERE id=@id;";
            cmd.Parameters.AddWithValue("id", id);

            return _database.ExecuteNonQuery(cmd) == 1;
        }

        public TourEntry? Get(int id)
        {
            try
            {
                NpgsqlCommand cmd = new();
                cmd.CommandText = "SELECT * FROM tour_entries WHERE id=@id;";
                cmd.Parameters.AddWithValue("id", id);

                // Get base tour data
                var tourData = _database.SelectSingle(cmd);

                if (tourData == null)
                    return null;

                return ParseFromRow(tourData);
            }
            catch (Exception ex)
            {
                // LOG ERROR
                Console.WriteLine(ex.ToString());
            }

            return null;
        }

        internal static TourEntry? ParseFromRow(OrderedDictionary row)
        {
            try
            {
                return new()
                {
                    Id = int.Parse(row["id"]?.ToString() ?? ""),
                    TourId = int.Parse(row["tour_id"]?.ToString() ?? ""),
                    Date = DateTime.Parse(row["date"]?.ToString() ?? ""),
                    Distance = float.Parse(row["distance"]?.ToString() ?? ""),
                    Duration = float.Parse(row["duration"]?.ToString() ?? ""),
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }

        public IEnumerable<TourEntry> GetAll()
        {
            try
            {
                NpgsqlCommand cmd = new();
                cmd.CommandText = "SELECT * FROM tour_entries;";

                // Get base tour data
                var tourData = _database.Select(cmd);

                if (tourData == null)
                    return Enumerable.Empty<TourEntry>();

                List<TourEntry> tourEntries = new();

                foreach (var entry in tourData)
                {
                    var e = ParseFromRow(entry);
                    if (e != null)
                        tourEntries.Add(e);
                }

                return tourEntries;
            }
            catch (Exception ex)
            {
                // LOG ERROR
                Console.WriteLine(ex.ToString());
            }

            return Enumerable.Empty<TourEntry>();
        }

        public bool Insert(ref TourEntry item)
        {
            try
            {
                // Insert tour entries
                NpgsqlCommand cmd = new();
                cmd.CommandText = $@"INSERT INTO tour_entries (distance, date, duration, tour_id)
                VALUES (@dist, @date, @dur, @tour_id) RETURNING id;";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("dist", item.Distance);
                cmd.Parameters.AddWithValue("date", item.Date);
                cmd.Parameters.AddWithValue("dur", item.Duration);
                cmd.Parameters.AddWithValue("tour_id", item.TourId);

                var result = _database.SelectSingle(cmd);
                if (result == null)
                    return false;

                // Change entry id
                item.Id = int.Parse(result["id"]?.ToString() ?? "");

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        public bool Update(ref TourEntry item)
        {
            try
            {
                NpgsqlCommand cmd = new();
                cmd.CommandText = @"UPDATE tour_entries SET tour_id=@tour_id, distance=@dist, duration=@dur, date=@date
                WHERE id=@id;";
                cmd.Parameters.AddWithValue("id", item.Id);
                cmd.Parameters.AddWithValue("tour_id", item.TourId);
                cmd.Parameters.AddWithValue("dist", item.Distance);
                cmd.Parameters.AddWithValue("dur", item.Duration);
                cmd.Parameters.AddWithValue("date", item.Date);

                return _database.ExecuteNonQuery(cmd) == 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }
    }
}
