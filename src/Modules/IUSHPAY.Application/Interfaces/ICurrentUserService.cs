namespace IUSHPAY.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string InstitutionalCode { get; }
    string Role { get; }
}
