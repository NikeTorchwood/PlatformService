
using CommandService.AsyncDataServices;
using CommandService.Data;
using CommandService.EventProcessing;
using CommandService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

namespace CommandService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(
                opt => 
                opt.UseInMemoryDatabase("InMem"));
            builder.Services.AddScoped<ICommandRepository, CommandRepository>();
            builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
            builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();
            builder.Services.AddHostedService<MessageBusSubscriber>();
            builder.Services.AddControllers();
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            PrepDb.PrepPopulation(app);

            app.Run();
        }
    }
}
