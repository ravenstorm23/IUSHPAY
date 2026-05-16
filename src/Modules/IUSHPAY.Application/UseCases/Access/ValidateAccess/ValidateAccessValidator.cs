using FluentValidation;

namespace IUSHPAY.Application.UseCases.Access.ValidateAccess;

public class ValidateAccessValidator : AbstractValidator<ValidateAccessCommand>
{
    public ValidateAccessValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("El token QR es obligatorio.")
            .Must(t => Guid.TryParse(t, out _))
            .WithMessage("El token QR tiene un formato inválido.");

        RuleFor(x => x.Fee)
            .GreaterThan(0)
            .WithMessage("La tarifa de acceso debe ser mayor a cero.")
            .LessThanOrEqualTo(50_000)
            .WithMessage("La tarifa de acceso supera el máximo permitido.");
    }
}
