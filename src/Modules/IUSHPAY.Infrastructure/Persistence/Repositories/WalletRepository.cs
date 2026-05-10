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
		=> await _db.Wallets.FirstOrDefaultAsync(x => x.UserId == userId);

	public async Task<Wallet?> GetByIdAsync(Guid walletId)
		=> await _db.Wallets.FirstOrDefaultAsync(x => x.Id == walletId);

	public async Task UpdateAsync(Wallet wallet)
	{
		_db.Wallets.Update(wallet);
		await _db.SaveChangesAsync();
	}
}