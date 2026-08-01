using BoardApi.Data;
using BoardApi.Exceptions;
using BoardApi.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
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
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.HttpOnly = true; // JavaScript에서 쿠키에 접근하지 못하도록 설정 (보안 강화)
    options.Cookie.SameSite = SameSiteMode.Lax; // CSRF 공격 방지 (Lax: 동일 사이트 요청과 일부 외부 사이트 요청 허용)
    options.ExpireTimeSpan = TimeSpan.FromDays(7); // 쿠키 만료 시간 설정 (예: 7일)
    options.SlidingExpiration = true; // 사용자가 활동 중이면 쿠키 만료 시간을 연장 (슬라이딩 만료)

    // 인증 실패 시 401 Unauthorized 또는 403 Forbidden 상태 코드를 반환하도록 설정
    options.Events.OnRedirectToLogin = ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});


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

// 순서 중요: 인증 미들웨어는 반드시 Authorization 미들웨어보다 먼저 호출되어야 함
// 순서 중요: MapControllers()는 반드시 UseAuthentication()과 UseAuthorization() 이후에 호출되어야 함
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
