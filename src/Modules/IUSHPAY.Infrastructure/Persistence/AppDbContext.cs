using IUSHPAY.Domain.Entities;
using IUSHPAY.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace IUSHPAY.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options)
	{
	}

	public DbSet<User> Users => Set<User>();

	public DbSet<Wallet> Wallets => Set<Wallet>();

	public DbSet<Transaction> Transactions => Set<Transaction>();

	public DbSet<ParkingAccess> ParkingAccesses => Set<ParkingAccess>();


	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		// Aplica todas las configuraciones de la carpeta Configurations/
		// Esto reemplaza la configuración inline anterior que ignoraba
		// el backing field _transactions de Wallet, causando el error 500.
		builder.ApplyConfiguration(new UserConfiguration());
		builder.ApplyConfiguration(new WalletConfiguration());
		builder.ApplyConfiguration(new TransactionConfiguration());

		// =====================================================
		// PARKING ACCESS (sin clase de configuración propia)
		// =====================================================
		builder.Entity<ParkingAccess>(entity =>
		{
			entity.HasKey(x => x.Id);

			entity
				.HasOne(x => x.User)
				.WithMany(x => x.ParkingAccesses)
				.HasForeignKey(x => x.UserId);
		});
	}
}