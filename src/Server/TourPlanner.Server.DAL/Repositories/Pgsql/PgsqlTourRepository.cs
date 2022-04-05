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
                distance REAL,
                start_point INT UNIQUE,
                end_point INT UNIQUE,
                FOREIGN KEY (start_point)
                    REFERENCES tour_points (id) ON DELETE CASCADE,
                FOREIGN KEY (end_point)
                    REFERENCES tour_points (id) ON DELETE CASCADE);";
            _database.ExecuteNonQuery(cmd);

            cmd.CommandText = @"CREATE TABLE tour_entries (
                id SERIAL PRIMARY KEY,
                distance REAL,
                duration REAL,
                date TIMESTAMP,
                tour_id INT,
                FOREIGN KEY (tour_id)
                    REFERENCES tours (id) ON DELETE CASCADE);";
            _database.ExecuteNonQuery(cmd);
        }

        public bool Delete(int id)
        {
            NpgsqlCommand cmd = new();
            cmd.CommandText = "DELETE FROM tours WHERE id=@id;";
            cmd.Parameters.AddWithValue("id", id);

            return _database.ExecuteNonQuery(cmd) == 1;
        }

        public Tour? Get(int id)
        {
            try
            {
                NpgsqlCommand cmd = new();
                cmd.CommandText = "SELECT * FROM tours WHERE id=@id;";
                cmd.Parameters.AddWithValue("id", id);

                // Get base tour data
                var tourData = _database.SelectSingle(cmd);

                if (tourData == null)
                    return null;

                return GetTourFromTourData(tourData);
            }
            catch (Exception ex)
            {
                // LOG ERROR
                Console.WriteLine(ex.ToString());
            }

            return null;
        }

        private Tour? GetTourFromTourData(OrderedDictionary tourData)
        {
            NpgsqlCommand cmd = new();

            // Get tour points
            cmd.CommandText = "SELECT * FROM tour_points WHERE id=@id;";
            cmd.Parameters.AddWithValue("id", int.Parse(tourData["start_point"]?.ToString() ?? ""));
            var tourStartPoint = _database.SelectSingle(cmd);

            cmd.Parameters["id"].Value = int.Parse(tourData["end_point"]?.ToString() ?? "");
            var tourEndPoint = _database.SelectSingle(cmd);

            // Get tour entries
            cmd.CommandText = "SELECT * FROM tour_entries WHERE tour_id=@id;";
            cmd.Parameters["id"].Value = int.Parse(tourData["id"]?.ToString() ?? "");

            var tourEntries = _database.Select(cmd);

            return ParseFromRow(tourData, tourEntries, tourStartPoint, tourEndPoint);
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
                    Distance = float.Parse(tourData["distance"]?.ToString() ?? ""),
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
            List<Tour> tours = new();

            try
            {
                NpgsqlCommand cmd = new();
                cmd.CommandText = "SELECT * FROM tours;";

                // Get base tour data
                var tourData = _database.Select(cmd);

                foreach (var tour in tourData)
                {
                    // Get tour entries and start + endpoint
                    var t = GetTourFromTourData(tour);

                    if(t != null)
                        tours.Add(t);
                }

                return tours;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Enumerable.Empty<Tour>();
        }

        public bool Insert(ref Tour item)
        {
            try
            {
                if (item.StartPoint == null || item.EndPoint == null || item.Distance == null ||
                item.Name == null || item.Description == null)
                    return false;

                // Insert start point
                NpgsqlCommand cmd = new();
                cmd.CommandText = $@"INSERT INTO tour_points (latitude, longitude) VALUES (@lat, @lon) RETURNING id;";
                cmd.Parameters.AddWithValue("lat", item.StartPoint.Latitude);
                cmd.Parameters.AddWithValue("lon", item.StartPoint.Longitude);

                var result = _database.SelectSingle(cmd);
                if (result == null)
                    return false;

                // Change start point id
                item.StartPoint.Id = int.Parse(result["id"]?.ToString() ?? "");

                // Insert end point
                cmd.Parameters["lat"].Value = item.EndPoint.Latitude;
                cmd.Parameters["lon"].Value = item.EndPoint.Longitude;

                result = _database.SelectSingle(cmd);
                if (result == null)
                {
                    // TODO : Delete start point

                    return false;
                }

                // Change end point id
                item.EndPoint.Id = int.Parse(result["id"]?.ToString() ?? "");

                // Insert tour
                cmd.CommandText = @"INSERT INTO tours (name, description, distance, start_point, end_point)
            VALUES (@name, @desc, @dist, @start, @end) RETURNING id";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("name", item.Name);
                cmd.Parameters.AddWithValue("desc", item.Description);
                cmd.Parameters.AddWithValue("dist", item.Distance);
                cmd.Parameters.AddWithValue("start", item.StartPoint.Id);
                cmd.Parameters.AddWithValue("end", item.EndPoint.Id);

                result = _database.SelectSingle(cmd);
                if (result == null)
                {
                    // Delete start and end point
                    return false;
                }

                // Change tour id
                item.Id = int.Parse(result["id"]?.ToString() ?? "");

                // Insert tour entries
                cmd.CommandText = $@"INSERT INTO tour_entries (distance, date, duration)
                VALUES (@dist, @date, @dur) RETURNING id;";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("dist", NpgsqlTypes.NpgsqlDbType.Real);
                cmd.Parameters.Add("date", NpgsqlTypes.NpgsqlDbType.TimestampTz);
                cmd.Parameters.Add("dur", NpgsqlTypes.NpgsqlDbType.Real);

                foreach (var entry in item.Entries)
                {
                    cmd.Parameters["dist"].Value = entry.Distance;
                    cmd.Parameters["date"].Value = entry.Date;
                    cmd.Parameters["dur"].Value = entry.Duration;

                    result = _database.SelectSingle(cmd);
                    if (result == null)
                    {
                        // Delete start and end point and tour and all added entries
                        return false;
                    }

                    // Change entry id
                    entry.Id = int.Parse(result["id"]?.ToString() ?? "");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        public bool Update(ref Tour item)
        {
            throw new NotImplementedException();
        }

    }
}
