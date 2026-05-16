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

	public async Task<Wallet?> GetByUserIdAsync(Guid userId)
		=> await _db.Wallets
			.Include(w => w.Transactions)
			.FirstOrDefaultAsync(x => x.UserId == userId);

	public async Task<Wallet?> GetByIdAsync(Guid walletId)
		=> await _db.Wallets
			.FirstOrDefaultAsync(x => x.Id == walletId);

	public async Task<Wallet?> GetByIdWithTransactionsAsync(Guid walletId)
		=> await _db.Wallets
			.Include(w => w.Transactions)
			.FirstOrDefaultAsync(x => x.Id == walletId);

	public async Task UpdateAsync(Wallet wallet)
	{
		// Actualizar el balance de la wallet directamente en la BD
		// sin tocar el grafo de transacciones
		await _db.Wallets
			.Where(w => w.Id == wallet.Id)
			.ExecuteUpdateAsync(s => s
				.SetProperty(w => w.Balance, wallet.Balance)
				.SetProperty(w => w.UpdatedAt, wallet.UpdatedAt));

		// Insertar las transacciones nuevas directamente
		foreach (var tx in wallet.Transactions)
		{
			var exists = await _db.Transactions.AnyAsync(t => t.Id == tx.Id);
			if (!exists)
				await _db.Transactions.AddAsync(tx);
		}

		await _db.SaveChangesAsync();
	}
}