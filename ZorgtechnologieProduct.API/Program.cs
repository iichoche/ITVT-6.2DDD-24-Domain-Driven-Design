using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using ZorgtechnologieProduct.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.EntityFrameworkCore.InMemory;

namespace ZorgtechnologieProduct.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environment == "Testing")
            {
                builder.Services.AddDbContext<ZorgtechnologieProductDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            }
            else
            {
                builder.Services.AddDbContext<ZorgtechnologieProductDbContext>(options =>
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly("ZorgtechnologieProduct.Infrastructure")
                    )
                );
            }

            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddLogging();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors();
            app.UseAuthorization();
            app.MapControllers();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine($"[DEBUG] Connection string: {connectionString}");

            app.Run();
        }
    }
}