﻿using Npgsql;
using System.Collections.Specialized;
using System.Data;
using TourPlanner.Common.Models;

namespace TourPlanner.Server.DAL.Repositories.Pgsql
{
    public class PgsqlTourRepository : IRepository<Tour>
    {
        private readonly PgsqlDatabase _database;
        private readonly PgsqlTourEntryRepository _tourEntryRepository;

        public PgsqlTourRepository(PgsqlDatabase database, PgsqlTourEntryRepository tourEntryRepository)
        {
            _database = database;
            _tourEntryRepository = tourEntryRepository;

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

        public void DeleteTables()
        {
            NpgsqlCommand cmd = new();
            cmd.CommandText = "DROP TABLE tour_entries; DROP TABLE tours; DROP TABLE tour_points;";

            _database.ExecuteNonQuery(cmd);
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
                estimated_time REAL,
                transport_type INT,
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
                duration INT,
                comment VARCHAR(500),
                difficulty REAL,
                rating REAL,
                date TIMESTAMP,
                tour_id INT,
                FOREIGN KEY (tour_id)
                    REFERENCES tours (id) ON DELETE CASCADE);";
            _database.ExecuteNonQuery(cmd);
        }

        public bool Delete(int id)
        {
            // Get tour for points
            Tour? tour = Get(id);
            if (tour == null)
                return false;

            NpgsqlCommand cmd = new();
            cmd.CommandText = "DELETE FROM tours WHERE id=@id;";
            cmd.Parameters.AddWithValue("id", id);

            bool success = _database.ExecuteNonQuery(cmd) == 1;
            if (!success)
                return false;

            cmd.CommandText = "DELETE FROM tour_entries WHERE tour_id=@id;";
            _database.ExecuteNonQuery(cmd);

            cmd.CommandText = "DELETE FROM tour_points WHERE id=@id";
            cmd.Parameters["id"].Value = tour.StartPoint?.Id;

            success = _database.ExecuteNonQuery(cmd) == 1;
            if (!success)
                return false;

            cmd.Parameters["id"].Value = tour.EndPoint?.Id;
            success = _database.ExecuteNonQuery(cmd) == 1;
            if (!success)
                return false;

            return true;
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

        public IEnumerable<Tour> GetAll()
        {
            List<Tour> tours = new();

            try
            {
                NpgsqlCommand cmd = new();
                cmd.CommandText = "SELECT * FROM tours;";

                // Get base tour data
                var tourData = _database.Select(cmd);

                if (tourData == null)
                    return Enumerable.Empty<Tour>();

                foreach (var tour in tourData)
                {
                    // Get tour entries and start + endpoint
                    var t = GetTourFromTourData(tour);

                    if (t != null)
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
                if (item.StartPoint == null || item.EndPoint == null ||
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
                cmd.CommandText = @"INSERT INTO tours (name, description, distance, start_point, end_point,
                estimated_time, transport_type) VALUES (@name, @desc, @dist, @start, @end, @eta, @tt) RETURNING id";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("name", item.Name);
                cmd.Parameters.AddWithValue("desc", item.Description);
                cmd.Parameters.AddWithValue("dist", item.Distance);
                cmd.Parameters.AddWithValue("eta", item.EstimatedTime);
                cmd.Parameters.AddWithValue("tt", (int)item.TransportType);
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
                cmd.CommandText = $@"INSERT INTO tour_entries (distance, date, duration, tour_id,
                comment, difficulty, rating) VALUES (@dist, @date, @dur, @tour_id, @com, @dif, @rat)
                RETURNING id;";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("dist", NpgsqlTypes.NpgsqlDbType.Real);
                cmd.Parameters.Add("date", NpgsqlTypes.NpgsqlDbType.Timestamp);
                cmd.Parameters.Add("dur", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters.Add("tour_id", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters.Add("com", NpgsqlTypes.NpgsqlDbType.Varchar);
                cmd.Parameters.Add("dif", NpgsqlTypes.NpgsqlDbType.Real);
                cmd.Parameters.Add("rat", NpgsqlTypes.NpgsqlDbType.Real);

                foreach (var entry in item.Entries)
                {
                    entry.TourId = item.Id;
                    cmd.Parameters["dist"].Value = entry.Distance;
                    cmd.Parameters["date"].Value = DateTime.SpecifyKind(entry.Date, DateTimeKind.Unspecified);
                    cmd.Parameters["dur"].Value = entry.Duration;
                    cmd.Parameters["tour_id"].Value = entry.TourId;
                    cmd.Parameters["com"].Value = entry.Comment;
                    cmd.Parameters["dif"].Value = entry.Difficulty;
                    cmd.Parameters["rat"].Value = entry.Rating;

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

        public bool Update(ref Tour tour)
        {
            if (tour.StartPoint == null || tour.EndPoint == null ||
            tour.Name == null || tour.Description == null)
                return false;

            try
            {
                NpgsqlCommand cmd = new();
                // Update tour data
                cmd.CommandText = @"UPDATE tours SET name=@name, description=@desc, distance=@dist,
                start_point=@start_point_id, end_point=@end_point_id, estimated_time=@eta,
                transport_type=@tt WHERE tours.id=@id;";
                cmd.Parameters.AddWithValue("name", tour.Name);
                cmd.Parameters.AddWithValue("desc", tour.Description);
                cmd.Parameters.AddWithValue("dist", tour.Distance);
                cmd.Parameters.AddWithValue("start_point_id", tour.StartPoint.Id);
                cmd.Parameters.AddWithValue("end_point_id", tour.EndPoint.Id);
                cmd.Parameters.AddWithValue("eta", tour.EstimatedTime);
                cmd.Parameters.AddWithValue("tt", (int)tour.TransportType);
                cmd.Parameters.AddWithValue("id", tour.Id);

                if (_database.ExecuteNonQuery(cmd) != 1)
                    return false;

                // Update start point
                cmd = new();
                cmd.CommandText = $@"UPDATE tour_points SET latitude=@lat, longitude=@lon WHERE tour_points.id=@id;";
                cmd.Parameters.AddWithValue("lat", tour.StartPoint.Latitude);
                cmd.Parameters.AddWithValue("lon", tour.StartPoint.Longitude);
                cmd.Parameters.AddWithValue("id", tour.StartPoint.Id);

                if (_database.ExecuteNonQuery(cmd) != 1)
                    return false;

                // Update end point
                cmd.Parameters["lat"].Value = tour.EndPoint.Latitude;
                cmd.Parameters["lon"].Value = tour.EndPoint.Longitude;
                cmd.Parameters["id"].Value = tour.EndPoint.Id;

                if (_database.ExecuteNonQuery(cmd) != 1)
                    return false;

                // Update entries
                // Get old entries
                cmd = new();
                cmd.CommandText = "SELECT * FROM tour_entries WHERE tour_id=@id;";
                cmd.Parameters.AddWithValue("id", tour.Id);

                var oldEntriesData = _database.Select(cmd);

                var edited = new List<TourEntry>();

                // Update / Delete old entries
                if (oldEntriesData != null || oldEntriesData?.Length > 0)
                {
                    foreach (var entryData in oldEntriesData)
                    {
                        // Get old entry
                        var oldEntry = PgsqlTourEntryRepository.ParseFromRow(entryData);

                        if (oldEntry == null)
                            continue;

                        // Check if it still exists
                        var newEntry = tour.Entries.Where(e => e.Id == oldEntry.Id).FirstOrDefault();

                        // If it does exist => Update it
                        if (newEntry != null)
                        {
                            if (!_tourEntryRepository.Update(ref newEntry))
                                return false;
                            edited.Add(newEntry);
                        }
                        // If it does not exist => Delete it
                        else
                        {
                            _tourEntryRepository.Delete(oldEntry.Id);
                        }

                    }
                }

                // Insert rest of new entries
                for (int i = 0; i < tour.Entries.Count; i++)
                {
                    TourEntry entry = tour.Entries[i];

                    // Check if it was already edited
                    if (!edited.Where(e => e.Id == entry.Id).Any())
                    {
                        // Insert the not edited entry
                        if (!_tourEntryRepository.Insert(ref entry))
                            return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
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

        private static Tour? ParseFromRow(
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
                    var entryObj = PgsqlTourEntryRepository.ParseFromRow(entry);
                    if (entryObj != null)
                        entries.Add(entryObj);
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
                    EstimatedTime = float.Parse(tourData["estimated_time"]?.ToString() ?? ""),
                    TransportType = (TransportType)int.Parse(tourData["transport_type"]?.ToString() ?? ""),
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
    }
}
