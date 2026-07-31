using BoardApi.Data;
using BoardApi.Exceptions;
using BoardApi.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

// Log1
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

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
builder.Services.AddScoped<ICommentService, CommentService>();

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
