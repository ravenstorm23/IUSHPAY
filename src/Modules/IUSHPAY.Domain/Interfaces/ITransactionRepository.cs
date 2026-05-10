using IUSHPAY.Domain.Entities;

namespace IUSHPAY.Domain.Interfaces.Repositories;

public interface ITransactionRepository
{
	Task AddAsync(Transaction tx);
	Task<IEnumerable<Transaction>> GetByWalletIdAsync(Guid walletId);
}