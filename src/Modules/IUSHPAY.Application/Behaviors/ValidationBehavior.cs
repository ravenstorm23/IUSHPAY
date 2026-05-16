using FluentValidation;
using IUSHPAY.Application.Common.Models;

namespace IUSHPAY.Application.Behaviors;

/// <summary>
/// Ejecuta todos los validadores de FluentValidation registrados para un comando
/// antes de que llegue al handler. Si hay errores, retorna Result.Failure
/// sin llegar a ejecutar la lógica de negocio.
/// </summary>
public class ValidationBehavior<TCommand>
{
    private readonly IEnumerable<IValidator<TCommand>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TCommand>> validators)
    {
        _validators = validators;
    }

    public async Task<Result<TResult>> ValidateAndHandleAsync<TResult>(
        TCommand command,
        Func<TCommand, Task<Result<TResult>>> next)
    {
        if (!_validators.Any())
            return await next(command);

        var context = new ValidationContext<TCommand>(command);

        var failures = (await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context))))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errorMessage = string.Join(" | ", failures.Select(f => f.ErrorMessage));
            return Result<TResult>.Failure(errorMessage);
        }

        return await next(command);
    }
}
