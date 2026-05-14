using IUSHPAY.Application.Common.Models;
using IUSHPAY.Application.DTOs;
using IUSHPAY.Domain.Interfaces.Repositories;

namespace IUSHPAY.Application.UseCases.Wallet.GetHistory;

public class GetTransactionHistoryHandler
{
    private readonly ITransactionRepository _txRepo;

    public GetTransactionHistoryHandler(ITransactionRepository txRepo)
    {
        _txRepo = txRepo;
    }

    public async Task<Result<IEnumerable<TransactionDto>>> HandleAsync(GetTransactionHistoryQuery q)
    {
        var transactions = await _txRepo.GetByWalletIdAsync(q.WalletId);

        var dtos = transactions.Select(t => new TransactionDto(
            t.Id,
            t.Amount,
            t.Type.ToString(),
            t.Status.ToString(),
            t.CreatedAt));

        return Result<IEnumerable<TransactionDto>>.Success(dtos);
    }
}
