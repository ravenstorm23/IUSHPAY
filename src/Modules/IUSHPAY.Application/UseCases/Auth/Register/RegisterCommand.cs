namespace IUSHPAY.Application.UseCases.Auth.Register;

public record RegisterCommand(
    string InstitutionalCode,
    string FullName,
    string Email,
    string Password);
