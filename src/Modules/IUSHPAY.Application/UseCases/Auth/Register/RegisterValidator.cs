using FluentValidation;

namespace IUSHPAY.Application.UseCases.Auth.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    // Dominio institucional de la universidad
    private const string InstitutionalDomain = "@iush.edu.co";

    public RegisterValidator()
    {
        RuleFor(x => x.InstitutionalCode)
            .NotEmpty()
            .WithMessage("El código institucional es obligatorio.")
            .Matches(@"^\d{6,10}$")
            .WithMessage("El código institucional debe contener entre 6 y 10 dígitos.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("El nombre completo es obligatorio.")
            .MinimumLength(3)
            .WithMessage("El nombre debe tener al menos 3 caracteres.")
            .MaximumLength(100)
            .WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress()
            .WithMessage("El correo electrónico no tiene un formato válido.")
            .Must(email => email.EndsWith(InstitutionalDomain, StringComparison.OrdinalIgnoreCase))
            .WithMessage($"Solo se permiten correos institucionales ({InstitutionalDomain}).");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8)
            .WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
            .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número.");
    }
}
