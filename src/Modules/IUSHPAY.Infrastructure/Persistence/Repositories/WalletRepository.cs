using Microsoft.EntityFrameworkCore;
using IUSHPAY.Domain.Entities;
using IUSHPAY.Domain.Interfaces.Repositories;

namespace IUSHPAY.Infrastructure.Persistence.Repositories;

public class WalletRepository : IWalletRepository
{
	private readonly AppDbContext _db;

	public WalletRepository(AppDbContext db)
	{
		_db = db;
	}

	// FIX: Include Transactions para que EF pueda trackear
	// las nuevas transacciones que agrega Credit() y Debit()
	public async Task<Wallet?> GetByUserIdAsync(Guid userId)
		=> await _db.Wallets
			.Include(w => w.Transactions)
			.FirstOrDefaultAsync(x => x.UserId == userId);

	public async Task<Wallet?> GetByIdAsync(Guid walletId)
		=> await _db.Wallets
			.Include(w => w.Transactions)
			.FirstOrDefaultAsync(x => x.Id == walletId);

	public async Task UpdateAsync(Wallet wallet)
	{
		// FIX: Update() marcaba todo el grafo como Modified, causando que EF
		// intentara hacer UPDATE de transacciones ya existentes en vez de INSERT,
		// lo que resultaba en el error de concurrencia optimista (500).
		// Entry().State solo marca la wallet, las transacciones nuevas las
		// detecta EF automáticamente por el tracking del Include().
		_db.Entry(wallet).State = EntityState.Modified;
		await _db.SaveChangesAsync();
	}
}