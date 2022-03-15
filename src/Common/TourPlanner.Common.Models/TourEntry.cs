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
		public DateTime Date { get; set; }
		public float Duration { get; set; }
		public float Distance { get; set; }
		public TourPoint? StartPoint { get; set; }
		public TourPoint? EndPoint { get; set; }
	}
}
