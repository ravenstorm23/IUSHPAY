using IUSHPAY.Domain.Entities;

namespace IUSHPAY.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task<bool> ExistsByEmailAsync(string email);
}
