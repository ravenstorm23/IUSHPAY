using IUSHPAY.Application.Common.Interfaces;
using IUSHPAY.Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace IUSHPAY.Application.Behaviors;

/// <summary>
/// Loggea la entrada y salida de cada handler:
/// - Qué comando/query se ejecutó
/// - El userId del usuario autenticado
/// - Si terminó en éxito o falla (con el mensaje de error)
/// Útil para auditar quién hizo qué en el parqueadero.
/// </summary>
public class LoggingBehavior<TCommand>
{
    private readonly ILogger<LoggingBehavior<TCommand>> _logger;
    private readonly ICurrentUserService _currentUser;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TCommand>> logger,
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<Result<TResult>> LogAndHandleAsync<TResult>(
        TCommand command,
        Func<TCommand, Task<Result<TResult>>> next)
    {
        var commandName = typeof(TCommand).Name;
        var userId = _currentUser.UserId;

        _logger.LogInformation(
            "[IUSHPAY] Ejecutando {Command} | Usuario: {UserId}",
            commandName, userId);

        var result = await next(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation(
                "[IUSHPAY] {Command} completado exitosamente | Usuario: {UserId}",
                commandName, userId);
        }
        else
        {
            _logger.LogWarning(
                "[IUSHPAY] {Command} falló | Usuario: {UserId} | Error: {Error}",
                commandName, userId, result.Error);
        }

        return result;
    }
}
