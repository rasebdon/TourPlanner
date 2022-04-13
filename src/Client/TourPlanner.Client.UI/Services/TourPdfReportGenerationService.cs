using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Common.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text.pdf.draw;

namespace TourPlanner.Client.UI.Services
{
    public class TourPdfReportGenerationService : ITourReportGenerationService
    {
        public byte[] GenerateReport(Tour tour)
        {
            using MemoryStream memoryStream = new();
            Document document = new(PageSize.A4, 55, 55, 45, 55);

            var headingFont = FontFactory.GetFont(FontFactory.HELVETICA, 20f, BaseColor.BLACK);

            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            var headingPara = new Paragraph($"{tour.Name} Tour Report\n", headingFont);
            headingPara.SpacingBefore = 20;
            headingPara.SpacingAfter = 20;

            document.Add(headingPara);

            // Paragraph tour info
            Paragraph mainInfoPara = new(
                $"Description: {tour.Description}\n" +
                $"Distance: {tour.Distance}\n" +
                $"Estimated Time: {tour.EstimatedTime}\n" +
                $"Transport Type: {tour.TransportType}\n" +
                $"Child Friendliness: {tour.ChildFriendliness}\n" +
                $"Popularity: {tour.Popularity}\n"
                );
            mainInfoPara.SpacingBefore = 20;
            mainInfoPara.SpacingAfter = 20;

            document.Add(mainInfoPara);
            document.Add(new LineSeparator());

            // Start point
            Paragraph startPointPara = new();
            // Add heading
            startPointPara.Add(
                new Chunk(
                    $"Start Point\n",
                    headingFont));
            startPointPara.Add(
                new Chunk(
                    $"Latitude: {tour.StartPoint?.Latitude}\n" +
                    $"Longitude: {tour.StartPoint?.Longitude}\n"));
            startPointPara.SpacingBefore = 20;
            startPointPara.SpacingAfter = 20;

            document.Add(startPointPara);

            // Start point
            Paragraph endPointPara = new();
            // Add heading
            endPointPara.Add(
                new Chunk(
                    $"End Point\n",
                    headingFont));
            endPointPara.Add(
                new Chunk(
                    $"Latitude: {tour.EndPoint?.Latitude}\n" +
                    $"Longitude: {tour.EndPoint?.Longitude}\n"));

            endPointPara.SpacingBefore = 20;
            endPointPara.SpacingAfter = 20;

            document.Add(endPointPara);
            document.NewPage();

            // Paragraph tour logs
            Paragraph logsPara = new();
            logsPara.Add(
                new Chunk(
                    $"Tour Logs\n",
                    headingFont));
            logsPara.SpacingBefore = 20;
            logsPara.SpacingAfter = 20;

            PdfPTable logsTable = new(6);
            logsTable.AddCell("Date");
            logsTable.AddCell("Comment");
            logsTable.AddCell("Distance");
            logsTable.AddCell("Duration");
            logsTable.AddCell("Rating");
            logsTable.AddCell("Difficulty");

            foreach (var log in tour.Entries)
            {
                logsTable.AddCell($"{log.Date}");
                logsTable.AddCell($"{log.Comment}");
                logsTable.AddCell($"{log.Distance}");
                logsTable.AddCell($"{log.Duration}");
                logsTable.AddCell($"{log.Rating}");
                logsTable.AddCell($"{log.Difficulty}");
            }

            logsTable.SpacingBefore = 20;
            logsTable.SpacingAfter = 20;

            document.Add(logsPara);
            document.Add(logsTable);

            document.Close();
            return memoryStream.ToArray();
        }
    }
}
