using FluentValidation;
using IUSHPAY.Application.Behaviors;
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
    private readonly ValidationBehavior<RegisterCommand> _validation;

    public RegisterHandler(
        IUserRepository userRepo,
        IJwtService jwtService,
        IEnumerable<IValidator<RegisterCommand>> validators)
    {
        _userRepo = userRepo;
        _jwtService = jwtService;
        _validation = new ValidationBehavior<RegisterCommand>(validators);
    }

    public Task<Result<AuthResponseDto>> HandleAsync(RegisterCommand cmd)
        => _validation.ValidateAndHandleAsync(cmd, ExecuteAsync);

    private async Task<Result<AuthResponseDto>> ExecuteAsync(RegisterCommand cmd)
    {
        if (await _userRepo.ExistsByEmailAsync(cmd.Email))
            return Result<AuthResponseDto>.Failure("Ya existe un usuario con ese email");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(cmd.Password);

        var user = User.Create(
            cmd.InstitutionalCode,
            cmd.FullName,
            cmd.Email,
            passwordHash);

        await _userRepo.AddAsync(user);

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
