using IUSHPAY.Domain.Entities;

namespace IUSHPAY.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
