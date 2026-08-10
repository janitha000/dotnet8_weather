var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalOrigins", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors("AllowLocalOrigins");
app.MapGet("/", () => Results.Ok(new
{
    service = "GatewayApi",
    tip = "Use /api/cities, /api/weather, /api/auth via this gateway",
    cities = "http://localhost:5260/api/cities/{name}",
    weather = "http://localhost:5260/api/weather/{city}",
    auth = "http://localhost:5260/api/auth/login"
}));
app.MapReverseProxy();
app.Run();
