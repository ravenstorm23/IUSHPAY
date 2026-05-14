using IUSHPAY.Domain.Entities;
using IUSHPAY.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IUSHPAY.Infrastructure.Persistence.Repositories;

public class TransactionRepository : ITransactionRepository
{
	private readonly AppDbContext _db;

	public TransactionRepository(AppDbContext db)
	{
		_db = db;
	}

	public async Task AddAsync(Domain.Entities.Transaction tx)
	{
		_db.Transactions.Add(tx);
		await _db.SaveChangesAsync();
	}

	public async Task<IEnumerable<Domain.Entities.Transaction>> GetByWalletIdAsync(Guid walletId)
	{
		return await _db.Transactions
			.Where(x => x.WalletId == walletId)
			.OrderByDescending(x => x.CreatedAt)
			.ToListAsync();
	}
}