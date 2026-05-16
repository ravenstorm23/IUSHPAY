namespace IUSHPAY.Domain.Interfaces.Services;

public interface IQRGeneratorService
{
    /// <summary>Genera un token QR para el usuario y lo guarda en caché por 5 minutos.</summary>
    Task<string> GenerateAsync(Guid userId);
}

public interface IQRValidatorService
{
    /// <summary>
    /// Valida el token QR. Si es válido, retorna el userId asociado y
    /// elimina el token de caché (uso único). Retorna null si el token
    /// es inválido o ya expiró.
    /// </summary>
    Task<Guid?> ValidateAsync(string token);
}

// Alias de conveniencia para handlers que necesitan ambas operaciones
public interface IQRService : IQRGeneratorService, IQRValidatorService { }
