using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Common.Models
{
	public class TourEntry
	{
		public int Id { get; set; }
		public int TourId { get; set; }
		public DateTime Date { get; set; }
		public float Duration { get; set; }
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
