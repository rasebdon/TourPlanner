using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Server.BL.Common.Models
{
    public class RouteInfo
    {
        /// <summary>
        /// Distance is given in kilometers
        /// </summary>
        public float Distance { get; set; }
        /// <summary>
        /// Time is given in seconds
        /// </summary>
        public int Time { get; set; }
    }
}
