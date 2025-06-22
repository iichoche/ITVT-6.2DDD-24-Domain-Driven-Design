using BCImplementatie.Infrastructure.Persistence; // Ensure this namespace exists and is correctly referenced.
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DEBUG: print the actual connection string
Console.WriteLine("DB Conn: " + builder.Configuration.GetConnectionString("ImplementatieDb"));

//log string to verify the connection string is being read correctly
var conn = builder.Configuration.GetConnectionString("ImplementatieDb");
Console.WriteLine("DEBUG CONNECTION STRING", $"▶ ConnectionString → '{conn}'");

builder.Services.AddDbContext<ImplementatieDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("ImplementatieDb"),
                      sql => sql.EnableRetryOnFailure()));

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Auto-apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ImplementatieDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.Run();