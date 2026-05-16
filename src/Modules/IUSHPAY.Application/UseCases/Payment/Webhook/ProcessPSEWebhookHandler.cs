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
		// Si no viene firma (pruebas desde Swagger), se genera una simulada
		// En producción real PSE enviará su propia firma HMAC
		var signature = string.IsNullOrWhiteSpace(cmd.Signature)
			? "simulado"
			: cmd.Signature;

		if (!await _pse.ValidateSignatureAsync(cmd.Payload, signature))
			return Result<bool>.Failure("Firma inválida");

		var wallet = await _walletRepo.GetByIdAsync(cmd.WalletId);
		if (wallet == null)
			return Result<bool>.Failure("Wallet no encontrada");

		wallet.Credit(cmd.Amount, "PSE");
		await _walletRepo.UpdateAsync(wallet);

		return Result<bool>.Success(true);
	}
}