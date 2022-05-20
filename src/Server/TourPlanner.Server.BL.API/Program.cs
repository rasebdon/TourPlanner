using TourPlanner.Common.Models;
using TourPlanner.Server.BL.Common.Interfaces;
using TourPlanner.Server.BL.MapQuestAPI;
using TourPlanner.Server.DAL;
using TourPlanner.Server.DAL.Repositories;
using TourPlanner.Server.DAL.Repositories.Pgsql;

namespace TourPlanner.Server.BL.API
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            if (!ConfigureServices(builder))
                return;

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    
        private static bool ConfigureServices(WebApplicationBuilder builder)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            // Check if there is config file
            if (File.Exists("config.json"))
            {
                configBuilder.AddJsonFile("config.json");
            }
            // Get via environment variables
            else
            {
                configBuilder.AddEnvironmentVariables();
            }
            IConfigurationRoot? config = configBuilder.Build();

            // Configure DAL access
            builder.Services.AddSingleton(config);
            builder.Services.AddSingleton<IDatabase, PgsqlDatabase>();

            // Add repositories
            builder.Services.AddSingleton<IRepository<TourEntry>, PgsqlTourEntryRepository>();
            builder.Services.AddSingleton<IRepository<Tour>, PgsqlTourRepository>();

            string apiKey = config.GetValue<string>("MAPQUESTAPI_KEY");

            // Configure mapquestapi
            IMapService mapService = new MapQuestMapService(apiKey);
            IRouteService tourService = new MapQuestTourService(apiKey);
            ICoordinatesService coordinatesService = new MapQuestCoordinatesService(apiKey);

            // Add services to the container
            builder.Services.AddSingleton(mapService);
            builder.Services.AddSingleton(tourService);
            builder.Services.AddSingleton(coordinatesService);

            return true;
        }
    }
}