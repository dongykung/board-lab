using BoardApi.Dtos;
using BoardApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace BoardApi.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest request)
    {
        UserResponse result = await _userService.RegisterAsync(request);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> Login(LoginRequest request)
    {
        UserResponse result = await _userService.LoginAsync(request);
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, result.Id.ToString()),
            new Claim(ClaimTypes.Name, result.LoginId)
        };
        ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        AuthenticationProperties authProperties = new()
        {
            AllowRefresh = true,
            // 인증 세션을 새로 고침(갱신)할 수 있도록 허용할지 여부
            // 

            //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
            // 인증 세션의 만료 시간. null이면 세션 쿠키가 브라우저 종료 시 만료됨

            IsPersistent = true,
            // 인증 세션이 브라우저 종료 후에도 유지되는지 여부. true이면 영속 쿠키, false이면 세션 쿠키

            //IssuedUtc = <DateTimeOffset>,
            // 인증 세션이 발급된 시간. null이면 현재 시간으로 설정됨

            //RedirectUri = <string>
            // 인증 세션이 만료되거나 로그아웃 후 리디렉션할 URI. null이면 기본값으로 설정됨
        };
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            authProperties // 브라우저 종료 후에도 유지되는 세션 쿠키
        );
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }
}
