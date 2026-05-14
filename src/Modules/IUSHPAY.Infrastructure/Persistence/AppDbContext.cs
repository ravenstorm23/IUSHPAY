using IUSHPAY.Domain.Entities;

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


		// =====================================================
		// USER
		// =====================================================
		builder.Entity<User>(entity =>
		{
			entity.HasKey(x => x.Id);

			entity.Property(x => x.FullName)
				.IsRequired();

			entity.Property(x => x.Email)
				.IsRequired();

			entity.HasIndex(x => x.Email)
				.IsUnique();
		});


		// =====================================================
		// WALLET
		// =====================================================
		builder.Entity<Wallet>(entity =>
		{
			entity.HasKey(x => x.Id);

			entity.Property(x => x.Balance)
				.HasColumnType("numeric(18,2)");

			entity
				.HasOne(x => x.User)
				.WithOne(x => x.Wallet)
				.HasForeignKey<Wallet>(x => x.UserId);
		});


		// =====================================================
		// TRANSACTION
		// =====================================================
		builder.Entity<Transaction>(entity =>
		{
			entity.HasKey(x => x.Id);

			entity.Property(x => x.Amount)
				.HasColumnType("numeric(18,2)");

			entity
				.HasOne(x => x.Wallet)
				.WithMany(x => x.Transactions)
				.HasForeignKey(x => x.WalletId);
		});


		// =====================================================
		// PARKING ACCESS
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