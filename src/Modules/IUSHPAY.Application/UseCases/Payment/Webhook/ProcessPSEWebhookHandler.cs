using IUSHPAY.Application.Common.Models;
using IUSHPAY.Domain.Interfaces.Repositories;
using IUSHPAY.Domain.Interfaces.Services;

namespace IUSHPAY.Application.UseCases.Payment.Webhook;

public class ProcessPSEWebhookHandler
{
    private readonly IPaymentGatewayService _pse;
    private readonly IWalletRepository _walletRepo;

    public ProcessPSEWebhookHandler(
        IPaymentGatewayService pse,
        IWalletRepository walletRepo)
    {
        _pse = pse;
        _walletRepo = walletRepo;
    }

    public async Task<Result<bool>> HandleAsync(ProcessPSEWebhookCommand cmd)
    {
        if (!await _pse.ValidateSignatureAsync(cmd.Payload, cmd.Signature))
            return Result<bool>.Failure("Firma inválida");

        // GetByIdAsync because cmd.WalletId IS the wallet id, not the user id
        var wallet = await _walletRepo.GetByIdAsync(cmd.WalletId);
        if (wallet == null)
            return Result<bool>.Failure("Wallet no encontrada");

        // wallet.Credit already appends a Transaction to its internal collection;
        // UpdateAsync persists both the balance change and the new transaction row
        // via EF change tracking. Do NOT call txRepo.AddAsync separately.
        wallet.Credit(cmd.Amount, "PSE");
        await _walletRepo.UpdateAsync(wallet);

        return Result<bool>.Success(true);
    }
}
