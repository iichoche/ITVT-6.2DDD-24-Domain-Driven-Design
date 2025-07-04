using System;
using System.Threading;
using Azure.Core;
using Azure.Identity;
using BCImplementatie.Application.Interfaces;
using BCImplementatie.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var swaggerOn = builder.Configuration.GetValue<bool>("ENABLE_SWAGGER");

// decide whether to force SQL auth
var useSql = builder.Configuration.GetValue<bool>("UseSqlAuth");

// 1) grab base connection string
var baseConn = builder.Configuration
    .GetConnectionString("ImplementatieDb")
    ?? throw new InvalidOperationException("ConnectionStrings:ImplementatieDb ontbreekt");

// 2) factory to build either SQL-auth or AAD token connection
SqlConnection CreateSqlConnection()
{
    if (useSql || builder.Environment.IsDevelopment())
    {
        return new SqlConnection(
            baseConn + ";User ID=impadmin;Password=i2ukdJN19mVAXg;"
        );
    }
    else
    {
        var conn = new SqlConnection(baseConn);
        conn.AccessToken = new DefaultAzureCredential()
            .GetToken(
                new TokenRequestContext(new[] { "https://database.windows.net/.default" }),
                CancellationToken.None
            ).Token;
        return conn;
    }
}

// 3) register DbContext
builder.Services.AddDbContext<ImplementatieDbContext>(opts =>
    opts.UseSqlServer(
        CreateSqlConnection(),
        sqlOpts => sqlOpts.EnableRetryOnFailure()
    )
);

// 4) your application services
builder.Services.AddScoped<IGebruikRepository, GebruikRepository>();
builder.Services.AddScoped<IEventPublisher, EventPublisher>();
builder.Services.AddMediatR(typeof(Program).Assembly);

// 5) controllers & swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 6) configure forwarded‐headers so UseHttpsRedirection sees the original scheme
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
});

//builder.WebHost.UseUrls("http://0.0.0.0:8080");

// build the app
var app = builder.Build();

// 7) auto-migrate (optional)
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider
         .GetRequiredService<ImplementatieDbContext>()
         .Database.Migrate();
}

// 8) HTTP pipeline

// swagger only in dev
if (app.Environment.IsDevelopment() || swaggerOn)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// **must be first** so we pick up X-Forwarded-Proto
app.UseForwardedHeaders();

// redirect HTTP → HTTPS
app.UseHttpsRedirection();

// set up routing for middleware
app.UseRouting();

// your API-key check
app.UseMiddleware<ApiKeyMiddleware>();

// auth & controllers
app.UseAuthorization();
app.MapControllers();

app.Run();
