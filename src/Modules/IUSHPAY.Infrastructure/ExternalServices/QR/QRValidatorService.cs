using IUSHPAY.Application.Common.Interfaces;
using IUSHPAY.Domain.Interfaces.Services;

namespace IUSHPAY.Infrastructure.ExternalServices.QR;

/// <summary>
/// Responsabilidad única: valida tokens QR de uso único.
/// Busca el userId en caché, lo elimina (para que el mismo QR no se pueda usar dos veces)
/// y retorna el userId. Si el token no existe o ya expiró, retorna null.
/// </summary>
public class QRValidatorService : IQRValidatorService
{
    private readonly ICacheService _cache;

    public QRValidatorService(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task<Guid?> ValidateAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var value = await _cache.GetAsync<Guid>($"qr:{token}");

        if (value == Guid.Empty)
            return null;

        // Uso único: eliminar inmediatamente tras validar
        await _cache.RemoveAsync($"qr:{token}");

        return value;
    }
}
