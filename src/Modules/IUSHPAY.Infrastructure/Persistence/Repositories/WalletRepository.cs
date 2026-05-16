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

	// Para consultas de historial/balance donde no se va a modificar
	public async Task<Wallet?> GetByUserIdAsync(Guid userId)
		=> await _db.Wallets
			.Include(w => w.Transactions)
			.FirstOrDefaultAsync(x => x.UserId == userId);

	// Para modificaciones (webhook, recarga): sin Include para evitar
	// que EF trackee las transacciones existentes y las confunda con nuevas
	public async Task<Wallet?> GetByIdAsync(Guid walletId)
		=> await _db.Wallets
			.FirstOrDefaultAsync(x => x.Id == walletId);

	// Para consultas de historial donde se necesita el ID de la wallet
	public async Task<Wallet?> GetByIdWithTransactionsAsync(Guid walletId)
		=> await _db.Wallets
			.Include(w => w.Transactions)
			.FirstOrDefaultAsync(x => x.Id == walletId);

	public async Task UpdateAsync(Wallet wallet)
	{
		_db.Entry(wallet).State = EntityState.Modified;
		await _db.SaveChangesAsync();
	}
}