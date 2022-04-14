using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.Services.Reporting
{
    public class SummarizePdfReportGenerationService : ISummarizeReportGenerationService
    {
        public byte[] GenerateReport(IEnumerable<Tour> tours)
        {
            using MemoryStream memoryStream = new();
            Document document = new(PageSize.A4, 55, 55, 45, 55);

            var headingFont = FontFactory.GetFont(FontFactory.HELVETICA, 20f, BaseColor.BLACK);

            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            PdfBackgroundHelper pageEventHelper = new();
            writer.PageEvent = pageEventHelper;

            document.Open();

            var headingPara = new Paragraph($"Summarize Report\n", headingFont)
            {
                SpacingBefore = 20,
                SpacingAfter = 20
            };

            document.Add(headingPara);

            // Paragraph tours info
            var avgSymbol = (char)0x00D8;

            PdfPTable logsTable = new(5)
            {
                SpacingBefore = 20,
                SpacingAfter = 20
            };
            logsTable.AddCell("Id");
            logsTable.AddCell("Name");
            logsTable.AddCell($"{avgSymbol} Time");
            logsTable.AddCell($"{avgSymbol} Distance");
            logsTable.AddCell($"{avgSymbol} Rating");

            foreach (var tour in tours)
            {
                logsTable.AddCell($"{tour.Id}");
                logsTable.AddCell($"{tour.Name}");
                logsTable.AddCell($"{tour.Entries.Average(t => t.Duration)}");
                logsTable.AddCell($"{tour.Entries.Average(t => t.Distance)}");
                logsTable.AddCell($"{tour.Entries.Average(t => t.Rating)}");
            }

            document.Add(logsTable);

            document.Close();
            return memoryStream.ToArray();
        }
    }
}
