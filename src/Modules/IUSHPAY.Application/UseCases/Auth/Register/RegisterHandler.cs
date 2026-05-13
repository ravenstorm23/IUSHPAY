using IUSHPAY.Application.Common.Models;
using IUSHPAY.Domain.Entities;
using IUSHPAY.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

namespace IUSHPAY.Application.UseCases.Auth.Register;

public class RegisterHandler
{
	private readonly IUserRepository _userRepo;
	private static readonly PasswordHasher<string> _hasher = new();

	public RegisterHandler(IUserRepository userRepo)
	{
		_userRepo = userRepo;
	}

	public async Task<Result<string>> HandleAsync(RegisterCommand cmd)
	{
		if (string.IsNullOrWhiteSpace(cmd.Email) || string.IsNullOrWhiteSpace(cmd.Password))
			return Result<string>.Failure("Email y contraseña son obligatorios");

		if (await _userRepo.ExistsByEmailAsync(cmd.Email))
			return Result<string>.Failure("Ya existe un usuario con ese email");

		var passwordHash = _hasher.HashPassword(cmd.Email, cmd.Password);

		var user = User.Create(
			cmd.InstitutionalCode,
			cmd.FullName,
			cmd.Email,
			cmd.CarnetNumber,
			passwordHash);

		await _userRepo.AddAsync(user);

		return Result<string>.Success("Usuario registrado correctamente");
	}
}