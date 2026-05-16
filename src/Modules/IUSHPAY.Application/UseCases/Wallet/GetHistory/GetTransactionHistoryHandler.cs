using IUSHPAY.Application.Common.Models;
using IUSHPAY.Application.DTOs;
using IUSHPAY.Domain.Interfaces.Repositories;

namespace IUSHPAY.Application.UseCases.Wallet.GetHistory;

public class GetTransactionHistoryHandler
{
    private readonly ITransactionRepository _txRepo;
    private readonly IWalletRepository _walletRepo;

    public GetTransactionHistoryHandler(ITransactionRepository txRepo, IWalletRepository walletRepo)
    {
        _txRepo = txRepo;
        _walletRepo = walletRepo;
    }

    public async Task<Result<IEnumerable<TransactionDto>>> HandleAsync(GetTransactionHistoryQuery q)
    {
        var wallet = await _walletRepo.GetByUserIdAsync(q.UserId);
        if (wallet == null)
            return Result<IEnumerable<TransactionDto>>.Failure("Wallet no encontrada");

        var transactions = await _txRepo.GetByWalletIdAsync(wallet.Id);

        var dtos = transactions.Select(t => new TransactionDto(
            t.Id,
            t.Amount,
            t.Type.ToString(),
            t.Status.ToString(),
            t.CreatedAt));

        return Result<IEnumerable<TransactionDto>>.Success(dtos);
    }
}
