using System.Security.Cryptography;
using IUSHPAY.Application.Common.Models;
using IUSHPAY.Domain.Entities;
using IUSHPAY.Domain.Interfaces.Repositories;

namespace IUSHPAY.Application.UseCases.Auth.Register;

public class RegisterHandler
{
	private readonly IUserRepository _userRepo;

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

		var passwordHash = HashPassword(cmd.Password);

		var user = User.Create(
			cmd.InstitutionalCode,
			cmd.FullName,
			cmd.Email,
			cmd.CarnetNumber,
			passwordHash);

		await _userRepo.AddAsync(user);

		return Result<string>.Success("Usuario registrado correctamente");
	}

	public static string HashPassword(string password)
	{
		byte[] salt = RandomNumberGenerator.GetBytes(16);
		var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
		byte[] hash = pbkdf2.GetBytes(32);
		return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
	}
}