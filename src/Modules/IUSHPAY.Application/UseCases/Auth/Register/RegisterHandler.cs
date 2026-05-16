using IUSHPAY.Application.Common.Interfaces;
using IUSHPAY.Application.Common.Models;
using IUSHPAY.Application.DTOs.Auth;
using IUSHPAY.Domain.Entities;
using IUSHPAY.Domain.Interfaces.Repositories;

namespace IUSHPAY.Application.UseCases.Auth.Register;

public class RegisterHandler
{
    private readonly IUserRepository _userRepo;
    private readonly IJwtService _jwtService;

    public RegisterHandler(IUserRepository userRepo, IJwtService jwtService)
    {
        _userRepo = userRepo;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponseDto>> HandleAsync(RegisterCommand cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd.Email) || string.IsNullOrWhiteSpace(cmd.Password))
            return Result<AuthResponseDto>.Failure("Email y contraseña son obligatorios");

        if (await _userRepo.ExistsByEmailAsync(cmd.Email))
            return Result<AuthResponseDto>.Failure("Ya existe un usuario con ese email");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(cmd.Password);

        var user = User.Create(
            cmd.InstitutionalCode,
            cmd.FullName,
            cmd.Email,
            passwordHash);

        await _userRepo.AddAsync(user);

        // Genera el token directamente tras el registro — evita el segundo request de login
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
