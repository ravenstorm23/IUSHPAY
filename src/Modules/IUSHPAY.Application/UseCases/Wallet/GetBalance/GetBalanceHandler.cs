using IUSHPAY.Application.Common.Interfaces;
using IUSHPAY.Application.Common.Models;
using IUSHPAY.Application.DTOs;
using IUSHPAY.Domain.Interfaces.Repositories;

namespace IUSHPAY.Application.UseCases.Wallet.GetBalance;

public class GetBalanceHandler
{
    private readonly IWalletRepository _repo;
    private readonly ICacheService _cache;

    public GetBalanceHandler(IWalletRepository repo, ICacheService cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<Result<WalletDto>> HandleAsync(GetBalanceQuery q)
    {
        var cacheKey = $"wallet:{q.UserId}";
        var cached = await _cache.GetAsync<WalletDto>(cacheKey);

        if (cached != null)
            return Result<WalletDto>.Success(cached);

        var wallet = await _repo.GetByUserIdAsync(q.UserId);
        if (wallet == null)
            return Result<WalletDto>.Failure("Wallet no encontrada");

        var dto = new WalletDto(wallet.Id, wallet.Balance, DateTime.UtcNow);

        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromSeconds(10));

        return Result<WalletDto>.Success(dto);
    }
}
