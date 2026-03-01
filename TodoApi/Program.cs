using Microsoft.Data.Sqlite;
using Serilog;
using TodoApi.DTOs.CommonDTOs;
using TodoApi.Interfaces;
using TodoApi.Middlewares.ExceptionMiddleware;
using TodoApi.Repository;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging
string logPath = builder.Configuration["LogFilePath"] ?? "Logs/todoapi-.txt";

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the TodoService with the DI
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<IRepository<Todo>, TodoRepository>();

var app = builder.Build();

// Initialize the database
await app.InitializeDatabaseAsync(app.Configuration);

// Configure the HTTP request pipeline.

// Use global exception handling middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

