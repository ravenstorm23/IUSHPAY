using IUSHPAY.Application.Common.Interfaces;
using IUSHPAY.Domain.Interfaces.Services;

namespace IUSHPAY.Infrastructure.ExternalServices.QR;

/// <summary>
/// Responsabilidad única: genera tokens QR y los guarda en caché.
/// La validación la hace QRValidatorService.
/// </summary>
public class QRGeneratorService : IQRGeneratorService
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
}
