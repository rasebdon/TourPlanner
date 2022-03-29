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

            NpgsqlCommand cmd = new();
            cmd.CommandText = @"SELECT EXISTS (
                SELECT FROM 
                    pg_tables
                WHERE 
                    schemaname = 'public' AND 
                    tablename  = 'tours'
                );";
            var result = _database.SelectSingle(cmd);

            if (result == null || bool.Parse(result["exists"]?.ToString() ?? "") == false)
                CreateTables();
        }

        public void CreateTables()
        {
            NpgsqlCommand cmd = new();

            cmd.CommandText = @"CREATE TABLE tour_points (
                id SERIAL PRIMARY KEY,
                latitude REAL,
                longitude REAL);";
            _database.ExecuteNonQuery(cmd);

            cmd.CommandText = @"CREATE TABLE tours (
                id SERIAL PRIMARY KEY,
                name VARCHAR(50),
                description VARCHAR(500),
                start_point INT UNIQUE,
                end_point INT UNIQUE,
                FOREIGN KEY (start_point)
                    REFERENCES tour_points (id),
                FOREIGN KEY (end_point)
                    REFERENCES tour_points (id));";
            _database.ExecuteNonQuery(cmd);

            cmd.CommandText = @"CREATE TABLE tour_entries (
                id SERIAL PRIMARY KEY,
                distance REAL,
                duration REAL,
                date TIMESTAMP);";
            _database.ExecuteNonQuery(cmd);
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
            try
            {
                NpgsqlCommand cmd = new();
                cmd.CommandText = "SELECT * FROM tours WHERE id=$id;";
                cmd.Parameters.AddWithValue("id", id);

                // Get base tour data
                var tourData = _database.SelectSingle(cmd);

                if (tourData == null || tourData.Count != 1)
                    return null;

                // Get tour points
                cmd.CommandText = "SELECT * FROM tour_points WHERE id=$id;";
                cmd.Parameters["id"].Value = int.Parse(tourData["start_point"]?.ToString() ?? "");
                var tourStartPoint = _database.SelectSingle(cmd);

                cmd.Parameters["id"].Value = int.Parse(tourData["end_point"]?.ToString() ?? "");
                var tourEndPoint = _database.SelectSingle(cmd);

                // Get tour entries
                cmd.CommandText = "SELECT * FROM tour_entries WHERE tour_id=$id;";
                cmd.Parameters["id"].Value = int.Parse(tourData["id"]?.ToString() ?? "");

                var tourEntries = _database.Select(cmd);

                return ParseFromRow(tourData, tourEntries, tourStartPoint, tourEndPoint);
            }
            catch (Exception ex)
            {
                // LOG ERROR
                Console.WriteLine(ex.ToString());
            }

            return null;
        }

        private Tour? ParseFromRow(
            OrderedDictionary tourData,
            OrderedDictionary[] tourEntries,
            OrderedDictionary startPointData,
            OrderedDictionary endPointData)
        {
            try
            {
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
                        });
                }

                // Get tour start and endpoint
                TourPoint startPoint = new()
                {
                    Id = int.Parse(startPointData["id"]?.ToString() ?? ""),
                    Latitude = float.Parse(startPointData["latitude"]?.ToString() ?? ""),
                    Longitude = float.Parse(startPointData["longitude"]?.ToString() ?? ""),
                };
                
                TourPoint endPoint = new()
                {
                    Id = int.Parse(endPointData["id"]?.ToString() ?? ""),
                    Latitude = float.Parse(endPointData["latitude"]?.ToString() ?? ""),
                    Longitude = float.Parse(endPointData["longitude"]?.ToString() ?? ""),
                };


                // Build tour from rest of data
                return new()
                {
                    Id = int.Parse(tourData["id"]?.ToString() ?? ""),
                    Name = tourData["name"]?.ToString(),
                    Description = tourData["description"]?.ToString(),
                    Entries = entries,
                    StartPoint = startPoint,
                    EndPoint = endPoint,
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
