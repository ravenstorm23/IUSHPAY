using IUSHPAY.Application.Common.Models;
using IUSHPAY.Domain.Entities;
using IUSHPAY.Domain.Interfaces.Repositories;
using IUSHPAY.Domain.Interfaces.Services;

namespace IUSHPAY.Application.UseCases.Payment.Webhook;

public class ProcessPSEWebhookHandler
{
    private readonly IPaymentGatewayService _pse;
    private readonly IWalletRepository _walletRepo;
    private readonly ITransactionRepository _txRepo;

    public ProcessPSEWebhookHandler(
        IPaymentGatewayService pse,
        IWalletRepository walletRepo,
        ITransactionRepository txRepo)
    {
        _pse = pse;
        _walletRepo = walletRepo;
        _txRepo = txRepo;
    }

    public async Task<Result<bool>> HandleAsync(ProcessPSEWebhookCommand cmd)
    {
        if (!await _pse.ValidateSignatureAsync(cmd.Payload, cmd.Signature))
            return Result<bool>.Failure("Firma inválida");

        var wallet = await _walletRepo.GetByUserIdAsync(cmd.WalletId);
        if (wallet == null)
            return Result<bool>.Failure("Wallet no encontrada");

        wallet.Credit(cmd.Amount, "PSE");
        await _walletRepo.UpdateAsync(wallet);

        await _txRepo.AddAsync(Transaction.Credit(wallet.Id, cmd.Amount, "PSE"));

        return Result<bool>.Success(true);
    }
}
