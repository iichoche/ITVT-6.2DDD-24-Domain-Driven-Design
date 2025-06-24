using BCImplementatie.Application.Interfaces;
using BCImplementatie.Infrastructure.Persistence; // Ensure this namespace exists and is correctly referenced.
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DEBUG: print the actual connection string
Console.WriteLine("DB Conn: " + builder.Configuration.GetConnectionString("ImplementatieDb"));

//log string to verify the connection string is being read correctly
var conn = builder.Configuration.GetConnectionString("ImplementatieDb");
Console.WriteLine("DEBUG CONNECTION STRING", $"▶ ConnectionString → '{conn}'");


// using Azure.Identity;           // voor later Key Vault
// using Azure.Security.KeyVault.Secrets;



//API key in Azure Key Vault when in intune

// builder.Configuration.AddAzureKeyVault(
//      new Uri("https://<jouw-vault-naam>.vault.azure.net/"),
//      new DefaultAzureCredential());

// 1) DbContext en MediatR etc.
builder.Services.AddDbContext<ImplementatieDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("ImplementatieDb"),
                      sql => sql.EnableRetryOnFailure()));

builder.Services.AddScoped<IEventPublisher, EventPublisher>();
builder.Services.AddScoped<IGebruikRepository, GebruikRepository>();
builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("http://+:80");


var app = builder.Build();

// Auto-apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ImplementatieDbContext>();
    db.Database.Migrate();
}

// 4) Middleware-rij: zorg dat je key check **vóór** Swagger en controllers zit
app.UseMiddleware<ApiKeyMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();