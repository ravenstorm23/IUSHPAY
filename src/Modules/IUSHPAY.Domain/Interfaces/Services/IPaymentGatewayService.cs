namespace IUSHPAY.Domain.Interfaces.Services;

public interface IPaymentGatewayService
{
	Task<string> InitiateAsync(Guid walletId, decimal amount);
	Task<bool> ValidateSignatureAsync(string payload, string signature);
}