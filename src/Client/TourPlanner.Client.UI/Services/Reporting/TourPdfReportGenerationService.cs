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
using System.Windows.Media.Imaging;
using TourPlanner.Client.UI.ViewModels;

namespace TourPlanner.Client.UI.Services.Reporting
{
    public class TourPdfReportGenerationService : ITourReportGenerationService
    {
        public byte[] GenerateReport(Tour tour)
        {
            // Get tour image
            Image? tourImage = null;
            try
            {
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), $"assets/images/{tour.Id}.jpg");
                if (File.Exists(imgPath))
                    tourImage = Image.GetInstance(imgPath);
            }
            catch
            {
                tourImage = null;
            }

            using MemoryStream memoryStream = new();
            Document document = new(PageSize.A4, 55, 55, 45, 55);

            var headingFont = FontFactory.GetFont(FontFactory.HELVETICA, 20f, BaseColor.BLACK);

            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            PdfBackgroundHelper pageEventHelper = new();
            writer.PageEvent = pageEventHelper;

            document.Open();

            var headingPara = new Paragraph($"{tour.Name} Tour Report\n", headingFont);
            headingPara.SpacingBefore = 20;
            headingPara.SpacingAfter = 20;

            document.Add(headingPara);

            // Paragraph tour info
            Paragraph mainInfoPara = new(
                $"Description: {tour.Description}\n" +
                $"Distance: {tour.Distance}m\n" +
                $"Estimated Time: {tour.EstimatedTime}s\n" +
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

            // End point
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

            // Add image
            if(tourImage != null)
            {
                tourImage.ScalePercent(35f);
                tourImage.Alignment = 1;

                document.Add(tourImage);
            }

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
            logsTable.AddCell("Distance (m)");
            logsTable.AddCell("Duration (s)");
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
