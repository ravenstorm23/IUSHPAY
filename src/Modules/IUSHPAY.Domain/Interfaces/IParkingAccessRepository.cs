using IUSHPAY.Domain.Entities;

namespace IUSHPAY.Domain.Interfaces.Repositories;

public interface IParkingAccessRepository
{
	Task AddAsync(ParkingAccess access);

	Task<IReadOnlyList<ParkingAccess>> GetAllAsync(DateTime? from, DateTime? to);
}