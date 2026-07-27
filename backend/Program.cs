using BoardApi.Data;
using BoardApi.Exceptions;
using BoardApi.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Formatting.Compact;


// Log
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithThreadName()
    .WriteTo.Console()
    .WriteTo.File(
        formatter: new CompactJsonFormatter(),
        path: "Logs/board-api-.json",
        rollingInterval: RollingInterval.Day
    )
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BoardDbContext>(options =>
    options.UseSqlite("Data Source=board.db"));

// Exception
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Log
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IPostService, PostService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Log
app.UseSerilogRequestLogging();

// Exception
app.UseExceptionHandler();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
