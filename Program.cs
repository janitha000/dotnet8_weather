using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<ICityService, CityService>();

builder.Services.AddTransient<ICityNormalizer, CityNormalizer>();

builder.Services.Configure<CacheOptions>(
    builder.Configuration.GetSection(CacheOptions.SectionName));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalOrigins", builder =>
        {
            builder.WithOrigins(
                "http://localhost:5173"    // Vite default, if you use it
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});


builder.Services.AddMemoryCache();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // creates interview.db + Cities table if missing

    if (!db.Cities.Any())
    {
        db.Cities.AddRange(
            new City { Name = "Tehran", Country = "Iran", Latitude = "35.7152", Longitude = "51.4043" },
            new City { Name = "Colombo", Country = "Sri Lanka", Latitude = "6.9271", Longitude = "79.8612" },
            new City { Name = "Mumbai", Country = "India", Latitude = "19.0760", Longitude = "72.8777" },
            new City { Name = "Delhi", Country = "India", Latitude = "28.6139", Longitude = "77.2090" },
            new City { Name = "Bangalore", Country = "India", Latitude = "12.9716", Longitude = "77.5946" },
            new City { Name = "Hyderabad", Country = "India", Latitude = "17.3850", Longitude = "78.4867" },
            new City { Name = "Chennai", Country = "India", Latitude = "13.0827", Longitude = "80.2707" },
            new City { Name = "Kolkata", Country = "India", Latitude = "22.5726", Longitude = "88.3639" },
            new City { Name = "Jaipur", Country = "India", Latitude = "26.9197", Longitude = "75.7969" },
            new City { Name = "Ahmedabad", Country = "India", Latitude = "23.0225", Longitude = "72.5714" },
            new City { Name = "Surat", Country = "India", Latitude = "21.1702", Longitude = "72.8311" });
        db.SaveChanges();
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();
app.UseCors("AllowLocalOrigins");
app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
