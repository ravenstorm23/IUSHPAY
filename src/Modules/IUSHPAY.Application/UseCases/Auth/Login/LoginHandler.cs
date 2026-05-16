using IUSHPAY.Application.Common.Interfaces;
using IUSHPAY.Application.Common.Models;
using IUSHPAY.Application.DTOs.Auth;
using IUSHPAY.Domain.Interfaces.Repositories;

namespace IUSHPAY.Application.UseCases.Auth.Login;

public class LoginHandler
{
    private readonly IUserRepository _userRepo;
    private readonly IJwtService _jwtService;

    public LoginHandler(IUserRepository userRepo, IJwtService jwtService)
    {
        _userRepo = userRepo;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponseDto>> HandleAsync(LoginCommand cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd.Email) || string.IsNullOrWhiteSpace(cmd.Password))
            return Result<AuthResponseDto>.Failure("Email y contraseña son obligatorios");

        var user = await _userRepo.GetByEmailAsync(cmd.Email);
        if (user == null)
            return Result<AuthResponseDto>.Failure("Usuario no encontrado");

        if (!BCrypt.Net.BCrypt.Verify(cmd.Password, user.PasswordHash))
            return Result<AuthResponseDto>.Failure("Contraseña incorrecta");

        var token = _jwtService.GenerateToken(user);

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            UserId = user.Id
        });
    }
}
