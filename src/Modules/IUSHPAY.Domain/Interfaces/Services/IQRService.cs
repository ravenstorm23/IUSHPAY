namespace IUSHPAY.Domain.Interfaces.Services;

public interface IQRService
{
	Task<string> GenerateAsync(Guid userId);
	Task<Guid?> ValidateAsync(string token);
}