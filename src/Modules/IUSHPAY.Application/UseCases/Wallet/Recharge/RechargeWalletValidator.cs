using FluentValidation;

namespace IUSHPAY.Application.UseCases.Wallet.Recharge;

public class RechargeWalletValidator : AbstractValidator<RechargeWalletCommand>
{
    public RechargeWalletValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El identificador de usuario es obligatorio.");

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(1000)
            .WithMessage("El monto mínimo de recarga es $1.000.")
            .LessThanOrEqualTo(5_000_000)
            .WithMessage("El monto máximo de recarga es $5.000.000.");
    }
}
