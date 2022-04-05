using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TourPlanner.Common.Models;
using TourPlanner.Server.DAL;
using TourPlanner.Server.DAL.Repositories;
using TourPlanner.Server.DAL.Repositories.Pgsql;

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
                Distance = 0,
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
                        Duration = 32.42f,
                    },
                    new()
                    {
                        Distance = 841.79f,
                        Date = DateTime.SpecifyKind(DateTime.Parse("2022-04-03T21:29:27.919Z"), DateTimeKind.Unspecified),
                        Duration = 95.421f,
                    },
                }
            };
        }

        [Test]
        public async Task GetTour()
        {
            // Arrange
            // Insert a tour into the database
            Assert.IsTrue(_tourRepository.Insert(ref tour1));

            // Act
            // Get tour with http client
            var result = await _client.GetAsync($"https://localhost:7222/Tour/{tour1.Id}");
            Tour? tour = JsonConvert.DeserializeObject<Tour>(
                await result.Content.ReadAsStringAsync());

            Assert.IsNotNull(tour);
            Assert.AreEqual(tour1, tour);
        }

        [TearDown]
        public void TearDown()
        {
            _tourRepository.Delete(tour1.Id);
        }
    }
}
