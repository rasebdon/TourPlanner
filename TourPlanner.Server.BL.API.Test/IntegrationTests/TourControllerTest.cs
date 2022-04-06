using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using TourPlanner.Common.Models;

namespace TourPlanner.Server.BL.API.Test.IntegrationTests
{
    public class TourControllerTest : IntegrationTestInitializer
    {
        Tour tour1;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            tour1 = new()
            {
                Name = "New Tour 1",
                Description = "This is a new tour!",
                Distance = 0.42f,
                EstimatedTime = 10,
                TransportType = TransportType.AUTO,
                StartPoint = new()
                {
                    Latitude = 24,
                    Longitude = 29,
                },
                EndPoint = new()
                {
                    Latitude = 25,
                    Longitude = 30,
                },
                Entries = new()
                {
                    new()
                    {
                        Distance = 742.42f,
                        Date = DateTime.SpecifyKind(DateTime.Parse("2022-04-04T21:29:27.919Z"), DateTimeKind.Unspecified),
                        Duration = 3242,
                        Rating = 4.2f,
                        Comment = "This is a very nice tour!",
                    },
                    new()
                    {
                        Distance = 841.79f,
                        Date = DateTime.SpecifyKind(DateTime.Parse("2022-04-03T21:29:27.919Z"), DateTimeKind.Unspecified),
                        Duration = 12423,
                        Rating = 3.24f,
                        Comment = "It was a bit too hard for us...",
                        Difficulty = 8.64f,
                    },
                }
            };
        }

        [Test]
        public async Task GetTourTestReturnsTourAnd200()
        {
            // Arrange
            // Insert a tour into the database
            Assert.IsTrue(_tourRepository.Insert(ref tour1));

            // Act
            // Get tour with http client
            var result = await _client.GetAsync($"https://localhost:7222/Tour/{tour1.Id}");
            Tour? tour = JsonConvert.DeserializeObject<Tour>(
                await result.Content.ReadAsStringAsync());

            // Assert
            Assert.IsNotNull(tour);
            Assert.AreEqual(tour1, tour);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TearDown]
        public void TearDown()
        {
            _tourRepository.Delete(tour1.Id);
        }
    }
}
