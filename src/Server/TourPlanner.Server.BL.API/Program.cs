using TourPlanner.Common.Models;
using TourPlanner.Server.BL.Common.Interfaces;
using TourPlanner.Server.BL.MapQuestAPI;
using TourPlanner.Server.DAL;
using TourPlanner.Server.DAL.Configuration;
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
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    
        private static bool ConfigureServices(WebApplicationBuilder builder)
        {
            // Configure DAL access
            DAL.Configuration.IConfiguration configuration = new JsonConfiguration(Directory.GetCurrentDirectory());

            builder.Services.AddSingleton(configuration);
            builder.Services.AddSingleton<IDatabase, PgsqlDatabase>();

            // Add repositories
            builder.Services.AddSingleton<IRepository<TourEntry>, PgsqlTourEntryRepository>();
            builder.Services.AddSingleton<IRepository<Tour>, PgsqlTourRepository>();

            if (!configuration.TryGetObject("api_key", out var apiKey))
                configuration.AddObject("api_key", "");

            // Configure mapquestapi
            IMapService mapService = new MapQuestMapService(apiKey?.ToString() ?? "");
            IRouteService tourService = new MapQuestTourService(apiKey?.ToString() ?? "");
            ICoordinatesService coordinatesService = new MapQuestCoordinatesService(apiKey?.ToString() ?? "");

            // Add services to the container
            builder.Services.AddSingleton(mapService);
            builder.Services.AddSingleton(tourService);
            builder.Services.AddSingleton(coordinatesService);

            return true;
        }
    }
}