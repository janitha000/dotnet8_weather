using Microsoft.EntityFrameworkCore;

public static class WebApplicationExtensions
{
    public static WebApplication InitializeDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Migrations apply to relational DBs (SQLite/SQL Server).
        // InMemory (used in API tests) has no migrations — use EnsureCreated.
        if (db.Database.IsRelational())
            db.Database.Migrate();
        else
            db.Database.EnsureCreated();

        if (!db.Cities.Any())
        {
            db.Cities.AddRange(
                new City { Name = "Tehran", Country = "Iran", Latitude = "35.7152", Longitude = "51.4043" },
                new City { Name = "Colombo", Country = "Sri Lanka", Latitude = "6.9271", Longitude = "79.8612" },
                new City { Name = "Mumbai", Country = "India", Latitude = "19.0760", Longitude = "72.8777" });
            db.SaveChanges();
        }

        return app;
    }

    public static WebApplication UsePipeline(this WebApplication app)
    {
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowLocalOrigins");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapGrpcService<CityLookupGrpcService>();

        return app;
    }
}
