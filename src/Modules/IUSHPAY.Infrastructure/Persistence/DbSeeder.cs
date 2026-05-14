using System.Security.Cryptography;
using IUSHPAY.Infrastructure.Persistence;
using IUSHPAY.Application.UseCases.Auth.Register;
using Microsoft.EntityFrameworkCore;

namespace IUSHPAY.Infrastructure.Persistence;

/// <summary>
/// Siembra los 3 usuarios administradores por defecto (Área TI).
/// Se ejecuta una sola vez al arrancar la app; si ya existen no hace nada.
/// </summary>
public static class DbSeeder
{
	private static readonly (string Email, string Password, string FullName)[] Admins =
	[
		("admin.ti1@iush.edu.co", "123456", "Administrador TI 1"),
		("admin.ti2@iush.edu.co", "123456", "Administrador TI 2"),
		("admin.ti3@iush.edu.co", "123456", "Administrador TI 3"),
	];

	public static async Task SeedAsync(AppDbContext db)
	{
		bool anyCreated = false;

		foreach (var (email, password, fullName) in Admins)
		{
			bool exists = await db.Users.AnyAsync(u => u.Email == email);
			if (exists) continue;

			string hash = HashPassword(password);

			var admin = User.CreateAdmin(email, hash, fullName);

			db.Users.Add(admin);
			anyCreated = true;
		}

		if (anyCreated)
			await db.SaveChangesAsync();
	}

	private static string HashPassword(string password)
	{
		byte[] salt = RandomNumberGenerator.GetBytes(16);
		var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
		byte[] hash = pbkdf2.GetBytes(32);
		return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
	}
}