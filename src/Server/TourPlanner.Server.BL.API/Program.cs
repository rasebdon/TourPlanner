using TourPlanner.Server.BL.API.Services;
using TourPlanner.Server.BL.MapQuestAPI;

namespace TourPlanner.Server.BL.API
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure DAL layer services
            Npgsql.NpgsqlConnection connection = new("CONNECTION_STRING");
            connection.Open();
            IRepositoryService repositoryService = new PgsqlRepositoryService(connection);

            // Configure mapquestapi
            var apiKey = builder.Configuration.GetValue(typeof(string), "apiKey") as string;
            MapQuestService mapQuestService = new(apiKey ?? "");

            // Add services to the container.
            builder.Services.AddSingleton(repositoryService);
            builder.Services.AddSingleton(mapQuestService);

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