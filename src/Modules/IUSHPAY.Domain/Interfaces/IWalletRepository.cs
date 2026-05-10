using IUSHPAY.Domain.Entities;

namespace IUSHPAY.Domain.Interfaces.Repositories;

public interface IWalletRepository
{
	Task<Wallet?> GetByUserIdAsync(Guid userId);
	Task<Wallet?> GetByIdAsync(Guid walletId);
	Task UpdateAsync(Wallet wallet);
}