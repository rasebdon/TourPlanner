namespace TourPlanner.Common.Models
{
    public class Tour
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<TourEntry> Entries { get; set; } = new();
    }
}