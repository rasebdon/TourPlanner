namespace TourPlanner.Common.Models
{
    public class Tour
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public float? Distance { get; set; }
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