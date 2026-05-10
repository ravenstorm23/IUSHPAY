namespace IUSHPAY.Application.UseCases.Payment.Webhook;

public record ProcessPSEWebhookCommand(
	string Payload,
	string Signature,
	Guid WalletId,
	decimal Amount
);