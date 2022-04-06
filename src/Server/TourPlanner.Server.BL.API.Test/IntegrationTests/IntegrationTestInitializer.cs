using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using TourPlanner.Common.Models;
using TourPlanner.Server.BL.API.Services;
using TourPlanner.Server.DAL.Repositories;

namespace TourPlanner.Server.BL.API.Test.IntegrationTests
{
    /// <summary>
    /// All ASP.Net integration test classes must derive from this class. <br/>
    /// It provides a test version of the api server and a http client to communicate with it.
    /// </summary>
    public abstract class IntegrationTestInitializer
    {
        protected HttpClient _client;
        protected TestServer _server;

        protected IRepository<Tour> _tourRepository;

        [SetUp]
        public virtual void SetUp()
        {
            var repoService = new PgsqlRepositoryService(
                "tour_planner_test", "tour_planner_admin", "tour_planner_1234");
            ServiceDescriptor repositoryDescriptor = new(typeof(IRepositoryService), repoService);

            var application = new WebApplicationFactory<Program>()
                    .WithWebHostBuilder(builder =>
                    {
                        builder.ConfigureTestServices(services =>
                        {
                            services.Add(repositoryDescriptor);
                        });
                    });

            _server = application.Server;
            _client = application.CreateClient();

            _tourRepository = repoService.GetRepository<Tour>();
        }

        [TearDown]
        public virtual void TearDown()
        {
            _client.Dispose();
            _server.Dispose();
        }
    }
}
