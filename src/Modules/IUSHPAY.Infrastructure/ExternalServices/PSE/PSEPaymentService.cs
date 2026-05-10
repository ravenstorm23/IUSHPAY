using IUSHPAY.Domain.Interfaces.Services;

namespace IUSHPAY.Infrastructure.ExternalServices.PSE;

public class PSEPaymentService : IPaymentGatewayService
{
	public Task<string> InitiateAsync(Guid walletId, decimal amount)
	{
		// Simulación URL PSE
		return Task.FromResult($"https://fake-pse.com/pay?walletId={walletId}&amount={amount}");
	}

	public Task<bool> ValidateSignatureAsync(string payload, string signature)
	{
		// Simulación validación webhook
		return Task.FromResult(true);
	}
}