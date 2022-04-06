namespace TourPlanner.Common.Models
{
    public enum TransportType
    {
        AUTO,
        WALKING,
        BICYCLE
    }

    public class Tour
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public float Distance { get; set; }
        public float EstimatedTime { get; set; }
        public TransportType TransportType { get; set; }


        // Computed values (not saved)
        /// <summary>
        /// Value between 0 (not child friendly) and 10 (very child friendly)
        /// </summary>
        public float ChildFriendliness 
        { 
            get
            {
                float cf = 0;
                foreach (var entry in Entries)
                {
                    float timeScaling = (float)(1 - Math.Pow(Math.E, (-entry.Duration) * 0.0001d));
                    float distanceScaling = (float)(1 - Math.Pow(Math.E, (-entry.Distance) * 0.075d));
                    float difficultyScaling = entry.Difficulty / 10f;

                    cf += (timeScaling + distanceScaling + difficultyScaling) / 3f * 10f;
                }
                return 10 - (cf / Entries.Count);
            } 
        }
        /// <summary>
        /// Value between 0 (not popular) and 1 (popular)
        /// </summary>
        public float Popularity
        {
            get
            {
                return (float)(1 - Math.Pow(Math.E, (-Entries.Count) * 0.025d)) * 10;
            }
        }

        public TourPoint? StartPoint { get; set; }
        public TourPoint? EndPoint { get; set; }
        public List<TourEntry> Entries { get; set; } = new();

        public override bool Equals(object? obj)
        {
            var equals = new HashSet<TourEntry>(
                Entries, EqualityComparer<TourEntry>.Default).SetEquals(((Tour)obj).Entries);

            return obj is Tour tour &&
                   Id == tour.Id &&
                   Name == tour.Name &&
                   Description == tour.Description &&
                   Distance == tour.Distance &&
                   EstimatedTime == tour.EstimatedTime &&
                   TransportType == tour.TransportType &&
                   StartPoint.Equals(tour.StartPoint) &&
                   EndPoint.Equals(tour.EndPoint) &&
                   equals;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Description, Distance, StartPoint, EndPoint, Entries);
        }
    }
}