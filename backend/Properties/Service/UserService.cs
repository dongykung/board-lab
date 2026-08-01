using BoardApi.Data;
using BoardApi.Dtos;
using BoardApi.Exceptions;
using BoardApi.Models;
using BoardApi.Security;
using Microsoft.EntityFrameworkCore;

namespace BoardApi.Services;

public class UserService : IUserService
{
    private readonly BoardDbContext _db;
    private readonly ILogger<UserService> _logger;

    public UserService(BoardDbContext db, ILogger<UserService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<UserResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.LoginId == request.LoginId))
        {
            throw new DuplicateLoginIdException(request.LoginId);
        }
        User user = new User
        {
            LoginId = request.LoginId,
            Password = PasswordHasher.Hash(request.Password),
            Name = request.Name
        };
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
        _logger.LogInformation("User registered: {LoginId}", user.LoginId);
        return UserResponse.FromEntity(user);
    }

    public async Task<UserResponse> LoginAsync(LoginRequest request)
    {
        User? user = await _db.Users.SingleOrDefaultAsync(u => u.LoginId == request.LoginId);

        if (user is null || !PasswordHasher.Verify(request.Password, user.Password))
        {
            throw new InvalidCredentialsException();
        }
        _logger.LogInformation("User logged in: {LoginId}", user.LoginId);
        return UserResponse.FromEntity(user);
    }
}
