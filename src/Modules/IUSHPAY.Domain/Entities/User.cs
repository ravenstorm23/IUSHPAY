using IUSHPAY.Domain.Entities;

public class User
{
	public Guid Id { get; private set; }

	public string InstitutionalCode { get; private set; } = string.Empty;

	public string FullName { get; private set; } = string.Empty;

	public string Email { get; private set; } = string.Empty;

	public string PasswordHash { get; private set; } = string.Empty;

	public string Role { get; private set; } = "User";

	public Wallet Wallet { get; private set; } = null!;

	public ICollection<ParkingAccess> ParkingAccesses { get; private set; }
		= new List<ParkingAccess>();

	public DateTime CreatedAt { get; private set; }

	private User() { }

	public static User Create(string code, string name, string email, string passwordHash)
	{
		var id = Guid.NewGuid();
		return new User
		{
			Id = id,
			InstitutionalCode = code,
			FullName = name,
			Email = email,
			PasswordHash = passwordHash,
			Role = "User",
			Wallet = Wallet.CreateFor(id),
			CreatedAt = DateTime.UtcNow
		};
	}

	/// <summary>
	/// Crea un usuario administrador (Área TI).
	/// </summary>
	/// <param name="email">Correo institucional del admin.</param>
	/// <param name="passwordHash">Hash BCrypt de la contraseña.</param>
	/// <param name="fullName">Nombre completo; por defecto "Administrador TI".</param>
	public static User CreateAdmin(string email, string passwordHash, string fullName = "Administrador TI")
	{
		var id = Guid.NewGuid();
		return new User
		{
			Id = id,
			InstitutionalCode = "ADMIN",
			FullName = fullName,
			Email = email,
			PasswordHash = passwordHash,
			Role = "Admin",
			Wallet = Wallet.CreateFor(id),
			CreatedAt = DateTime.UtcNow
		};
	}
}