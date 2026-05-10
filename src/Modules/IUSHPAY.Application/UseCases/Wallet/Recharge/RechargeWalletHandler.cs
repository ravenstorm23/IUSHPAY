using IUSHPAY.Application.Common.Models;
using IUSHPAY.Domain.Interfaces.Repositories;
using IUSHPAY.Domain.Interfaces.Services;

namespace IUSHPAY.Application.UseCases.Wallet.Recharge;

public class RechargeWalletHandler
{
    private readonly IWalletRepository _repo;
    private readonly IPaymentGatewayService _pse;

    public RechargeWalletHandler(IWalletRepository repo, IPaymentGatewayService pse)
    {
        _repo = repo;
        _pse = pse;
    }

    public async Task<Result<string>> HandleAsync(RechargeWalletCommand cmd)
    {
        if (cmd.Amount < 1000)
            return Result<string>.Failure("Monto mínimo 1000");

        var wallet = await _repo.GetByUserIdAsync(cmd.UserId);
        if (wallet == null)
            return Result<string>.Failure("Wallet no encontrada");

        var url = await _pse.InitiateAsync(wallet.Id, cmd.Amount);
        return Result<string>.Success(url);
    }
}
