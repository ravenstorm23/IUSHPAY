using System.Security.Claims;
using IUSHPAY.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace IUSHPAY.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }

    public string InstitutionalCode
        => _httpContextAccessor.HttpContext?.User.FindFirstValue("code") ?? string.Empty;

    public string Role
        => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
}
