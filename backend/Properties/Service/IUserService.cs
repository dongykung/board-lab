using BoardApi.Dtos;

namespace BoardApi.Services;

public interface IUserService
{
    Task<UserResponse> RegisterAsync(RegisterRequest request);
    Task<UserResponse> LoginAsync(LoginRequest request);
}
