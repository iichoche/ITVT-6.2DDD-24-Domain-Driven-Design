using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// read toggle & base‐URL + API‐key from config
var swaggerOn = builder.Configuration.GetValue<bool>("ENABLE_SWAGGER");
var implBase = builder.Configuration["IMPLEMENTATIE_API_URL"]!;

// 1) register BFF‐client
builder.Services.AddHttpClient("BFF", c =>
{
    c.BaseAddress = new Uri(implBase);
    c.DefaultRequestHeaders.Add("X-API-KEY",
        builder.Configuration["X_API_KEY"]!);
});

// 2) controllers & swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BFF API", Version = "v1" });
});

var app = builder.Build();

// 3) swagger in dev or when toggled
if (app.Environment.IsDevelopment() || swaggerOn)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 4) HTTPS redirect
app.UseHttpsRedirection();

// 5) routing
app.UseAuthorization();
app.MapControllers();

app.Run();
