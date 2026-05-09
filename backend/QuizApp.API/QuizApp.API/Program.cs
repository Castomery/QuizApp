
using Microsoft.EntityFrameworkCore;
using QuizApp.Application.Interfaces;
using QuizApp.Infrastructure.Persistence;
using StackExchange.Redis;
using QuizApp.Infrastructure.Cache;
using QuizApp.API.Hubs;
using QuizApp.Infrastructure.Persistence.Repositories;
using QuizApp.Application.Services;
using QuizApp.Infrastructure.AI;

namespace QuizApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddOpenApi();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .SetIsOriginAllowed(_ => true);
                    });
            });

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379"));
            builder.Services.AddScoped<IGameStateRepository, RedisGameStateRepository>();

            builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
            builder.Services.AddScoped<IGameSessionRepository, GameSessionRepository>();
            builder.Services.AddSingleton<ScoringService>();

            builder.Services.AddScoped<IGameService, GameService>();
            builder.Services.AddHttpClient<IAiClient, GroqClient>();
            builder.Services.AddScoped<AiHostService>();

            builder.Services.AddSignalR();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.MapHub<GameHub>("/hubs/game");

            app.Run();
        }
    }
}
