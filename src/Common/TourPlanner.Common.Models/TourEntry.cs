namespace TourPlanner.Common.Models
{
    public class TourEntry
    {
        private float difficulty;
        private float rating;

        public int Id { get; set; }
        public int TourId { get; set; }
        public string Comment { get; set; } = "";
        public float Difficulty { get => difficulty; set => difficulty = Math.Clamp(value, 0, 10); }
        public float Rating { get => rating; set => rating = Math.Clamp(value, 0, 5); }
        public DateTime Date { get; set; }
        /// <summary>
        /// Given in seconds
        /// </summary>
        public int Duration { get; set; }
        public float Distance { get; set; }

        public override bool Equals(object? obj)
        {
            var equals = obj is TourEntry entry &&
                   Id == entry.Id &&
                   TourId == entry.TourId &&
                   Date.ToString("DT") == entry.Date.ToString("DT") &&
                   Duration == entry.Duration &&
                   Distance == entry.Distance;

            return equals;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, TourId, Date.ToString("DT"), Duration, Distance);
        }
    }
}
