using Microsoft.EntityFrameworkCore;
using IUSHPAY.Domain.Entities;
using IUSHPAY.Domain.Interfaces.Repositories;
using IUSHPAY.Infrastructure.Persistence;

namespace IUSHPAY.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByIdAsync(Guid id)
        => await _db.Users.Include(x => x.Wallet)
                          .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<User?> GetByEmailAsync(string email)
        => await _db.Users.Include(x => x.Wallet)
                          .FirstOrDefaultAsync(x => x.Email == email);

    public async Task AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
        => await _db.Users.AnyAsync(x => x.Email == email);
}
