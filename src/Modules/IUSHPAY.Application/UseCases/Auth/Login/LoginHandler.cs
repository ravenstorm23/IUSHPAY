using System.Security.Cryptography;
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

		if (!VerifyPassword(cmd.Password, user.PasswordHash))
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

	private static bool VerifyPassword(string password, string stored)
	{
		try
		{
			var parts = stored.Split('.');
			if (parts.Length != 2) return false;
			byte[] salt = Convert.FromBase64String(parts[0]);
			byte[] expectedHash = Convert.FromBase64String(parts[1]);
			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
			byte[] actualHash = pbkdf2.GetBytes(32);
			return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
		}
		catch
		{
			return false;
		}
	}
}