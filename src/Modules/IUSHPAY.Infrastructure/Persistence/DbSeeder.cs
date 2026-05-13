using IUSHPAY.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IUSHPAY.Infrastructure.Persistence;

/// <summary>
/// Siembra los 3 usuarios administradores por defecto (Área TI).
/// Se ejecuta una sola vez al arrancar la app; si ya existen no hace nada.
/// </summary>
public static class DbSeeder
{
	// ─── Credenciales iniciales de los 3 admins ──────────────────────────────
	// IMPORTANTE: cambia las contraseñas en el primer login o mediante
	//             variables de entorno / secretos de producción.
	private static readonly (string Email, string Password, string FullName)[] Admins =
	[
		("admin.ti1@iush.edu.co",   "123456",  "Administrador TI 1"),
		("admin.ti2@iush.edu.co",   "123456",  "Administrador TI 2"),
		("admin.ti3@iush.edu.co",   "123456",  "Administrador TI 3"),
	];

	public static async Task SeedAsync(AppDbContext db)
	{
		bool anyCreated = false;
		var hasher = new PasswordHasher<string>();

		foreach (var (email, password, fullName) in Admins)
		{
			// Si el admin ya existe en BD, no lo vuelve a crear
			bool exists = await db.Users.AnyAsync(u => u.Email == email);
			if (exists) continue;

			string hash = hasher.HashPassword(email, password);

			// Usa el factory method existente en la entidad User
			var admin = User.CreateAdmin(email, hash);

			db.Users.Add(admin);
			anyCreated = true;
		}

		if (anyCreated)
			await db.SaveChangesAsync();
	}
}