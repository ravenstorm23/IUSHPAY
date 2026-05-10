using IUSHPAY.Application.DTOs.Auth;
using IUSHPAY.Application.UseCases.Auth.Login;
using IUSHPAY.Application.UseCases.Auth.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IUSHPAY.WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly LoginHandler _loginHandler;
    private readonly RegisterHandler _registerHandler;

    public AuthController(LoginHandler loginHandler, RegisterHandler registerHandler)
    {
        _loginHandler = loginHandler;
        _registerHandler = registerHandler;
    }

    /// <summary>Autenticar usuario y obtener JWT</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _loginHandler.HandleAsync(new LoginCommand(dto.Email, dto.Password));
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(new { message = result.Error });
    }

    /// <summary>Registrar nuevo usuario</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _registerHandler.HandleAsync(new RegisterCommand(
            dto.InstitutionalCode,
            dto.FullName,
            dto.Email,
            dto.CarnetNumber,
            dto.Password));

        return result.IsSuccess ? Ok(new { message = result.Value }) : BadRequest(new { message = result.Error });
    }

    /// <summary>Perfil del usuario autenticado</summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var name = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        return Ok(new { userId, email, name, role });
    }
}
