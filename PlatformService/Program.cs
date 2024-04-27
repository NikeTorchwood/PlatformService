
using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

namespace PlatformService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //Add services to the container.
            // Add Database Contexts
            var isDevelopment = builder.Environment.IsDevelopment();
            if (isDevelopment)
            {
                Console.WriteLine("--> Using InMem Db");
                builder.Services.AddDbContext<AppDbContext>(
                    opt => opt.UseInMemoryDatabase("InMem")
                    );
            }
            else
            {
                Console.WriteLine("--> Using SQL Server Db");
                Console.WriteLine(builder.Configuration.GetConnectionString("PlatformConn"));
                builder.Services.AddDbContext<AppDbContext>(
                    opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformConn")));
            }
            builder.Services.AddHttpClient<ICommandDataClient, HttpDataClient>();
            builder.Services.AddGrpc();
            // Add AutoMapper&DI
            builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
            builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // Add Controllers
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            PrepDb.PrepPopulation(app, isDevelopment);

            // Configure the HTTP request pipeline.
            

            Console.WriteLine($"--> CommandService Endpoint {app.Configuration["CommandService"]}");

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapGrpcService<GrpcPlatformService>();
            app.MapGet("/protos/platforms.proto", async context =>
            {
                await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
            });
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.MapControllers();

            app.Run();

        }
    }
}
