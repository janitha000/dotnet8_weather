using Cities.Grpc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalOrigins", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.Configure<WeatherAPIOptions>(
    builder.Configuration.GetSection(WeatherAPIOptions.SectionName));
builder.Services.Configure<CitiesApiOptions>(
    builder.Configuration.GetSection(CitiesApiOptions.SectionName));

var citiesOpts = builder.Configuration
    .GetSection(CitiesApiOptions.SectionName)
    .Get<CitiesApiOptions>() ?? new CitiesApiOptions();

// Keep HTTP adapter available (typed client registers CityLookupHttpClient).
builder.Services.AddHttpClient<CityLookupHttpClient>((sp, client) =>
{
    var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<CitiesApiOptions>>().Value;
    client.BaseAddress = new Uri(opts.BaseUrl.TrimEnd('/') + "/");
    client.Timeout = Timeout.InfiniteTimeSpan;
})
.AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
});

// Keep gRPC adapter available (generated CityLookupClient + wrapper).
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
builder.Services.AddGrpcClient<CityLookup.CityLookupClient>(o =>
{
    o.Address = new Uri(citiesOpts.GrpcUrl);
});
builder.Services.AddScoped<CityLookupGrpcClient>();

// Weather always depends on ICityLookup — switch transport via config.
if (string.Equals(citiesOpts.LookupMode, "Grpc", StringComparison.OrdinalIgnoreCase))
    builder.Services.AddScoped<ICityLookup>(sp => sp.GetRequiredService<CityLookupGrpcClient>());
else
    builder.Services.AddScoped<ICityLookup>(sp => sp.GetRequiredService<CityLookupHttpClient>());

builder.Services.AddHttpClient<IWeatherService, WeatherService>((sp, client) =>
{
    var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<WeatherAPIOptions>>().Value;
    client.BaseAddress = new Uri(opts.BaseUrl);
    client.Timeout = Timeout.InfiniteTimeSpan;
})
.AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseCors("AllowLocalOrigins");
app.MapControllers();
app.Run();
