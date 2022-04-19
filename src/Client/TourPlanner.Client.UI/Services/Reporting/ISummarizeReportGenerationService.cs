﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.Services.Reporting
{
    public interface ISummarizeReportGenerationService
    {
        public byte[] GenerateReport(IEnumerable<Tour> tours);
    }
}
