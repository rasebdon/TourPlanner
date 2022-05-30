namespace TourPlanner.Common.Models
{
    public class TourPoint
    {
        public int Id { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is TourPoint point &&
                   Id == point.Id &&
                   Longitude == point.Longitude &&
                   Latitude == point.Latitude;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Longitude, Latitude);
        }
    }
}
