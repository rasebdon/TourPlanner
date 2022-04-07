using TourPlanner.Server.BL.API.Services;
using TourPlanner.Server.BL.Common.Interfaces;
using TourPlanner.Server.BL.MapQuestAPI;
using TourPlanner.Server.DAL;

namespace TourPlanner.Server.BL.API
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure repository service
            IRepositoryService repositoryService = new PgsqlRepositoryService(
                "tour_planner", "tour_planner_admin", "tour_planner_1234");

            // Configure mapquestapi
            var apiKey = builder.Configuration.GetValue(typeof(string), "apiKey") as string;
            IMapService mapService = new MapQuestMapService(apiKey ?? "");
            IRouteService tourService = new MapQuestTourService(apiKey ?? "");
            ICoordinatesService coordinatesService = new MapQuestCoordinatesService(apiKey ?? "");

            // Add services to the container.
            builder.Services.AddSingleton(repositoryService);
            builder.Services.AddSingleton(mapService);
            builder.Services.AddSingleton(tourService);
            builder.Services.AddSingleton(coordinatesService);

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
    }
}