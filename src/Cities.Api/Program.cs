using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddJWTAuth(builder.Configuration);
builder.Services.AddCors(builder.Configuration);
builder.Services.AddRabbitMq(builder.Configuration);

var app = builder.Build();

app.InitializeDatabase();   // migrate + seed
app.UsePipeline();
app.Run();

// Makes the implicit Program class public for WebApplicationFactory<Program>
public partial class Program { }

