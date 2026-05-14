using IUSHPAY.Domain.Enums;

namespace IUSHPAY.Application.UseCases.Access.ValidateAccess;

public record ValidateAccessCommand(string Token, AccessMethod Method, decimal Fee);