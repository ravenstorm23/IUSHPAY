using IUSHPAY.Domain.Interfaces.Services;
using IUSHPAY.Application.Common.Interfaces;

namespace IUSHPAY.Infrastructure.ExternalServices.QR;

public class QRGeneratorService : IQRService
{
	private readonly ICacheService _cache;

	public QRGeneratorService(ICacheService cache)
	{
		_cache = cache;
	}

	public async Task<string> GenerateAsync(Guid userId)
	{
		var token = Guid.NewGuid().ToString();

		await _cache.SetAsync($"qr:{token}", userId, TimeSpan.FromMinutes(5));

		return token;
	}

	public async Task<Guid?> ValidateAsync(string token)
	{
		var value = await _cache.GetAsync<Guid>($"qr:{token}");

		if (value == Guid.Empty)
			return null;

		await _cache.RemoveAsync($"qr:{token}");

		return value;
	}
}