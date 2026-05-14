using IUSHPAY.Domain.Entities;
using IUSHPAY.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IUSHPAY.Infrastructure.Persistence.Repositories;

public class ParkingAccessRepository : IParkingAccessRepository
{
	private readonly AppDbContext _db;

	public ParkingAccessRepository(AppDbContext db)
	{
		_db = db;
	}

	public async Task AddAsync(ParkingAccess access)
	{
		_db.ParkingAccesses.Add(access);
		await _db.SaveChangesAsync();
	}

	public async Task<IReadOnlyList<ParkingAccess>> GetAllAsync(DateTime? from, DateTime? to)
	{
		var query = _db.ParkingAccesses
			.Include(x => x.User)
			.AsQueryable();

		if (from.HasValue)
			query = query.Where(x => x.AccessedAt >= from.Value.ToUniversalTime());

		if (to.HasValue)
			query = query.Where(x => x.AccessedAt <= to.Value.ToUniversalTime());

		return await query
			.OrderByDescending(x => x.AccessedAt)
			.ToListAsync();
	}
}