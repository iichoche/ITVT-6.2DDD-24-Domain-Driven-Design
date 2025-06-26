using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1) Lees base‐URL’s uit config of omgevingsvariabelen
var implBase = builder.Configuration["IMPLEMENTATIE_API_URL"]!;

// 2) Registreer je BFF‐HttpClient en voeg de API‐key header toe
builder.Services.AddHttpClient("BFF", client =>
{
    client.BaseAddress = new Uri(implBase);
    client.DefaultRequestHeaders.Add("X-API-KEY",
        builder.Configuration["X_API_KEY"]!);
});

// 3) MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BFF API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
