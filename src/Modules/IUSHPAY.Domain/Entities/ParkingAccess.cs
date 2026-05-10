using IUSHPAY.Domain.Enums;

namespace IUSHPAY.Domain.Entities;

public class ParkingAccess
{
	public Guid Id { get; private set; }

	public Guid UserId { get; private set; }

	public User User { get; private set; } = null!;

	public AccessMethod Method { get; private set; }

	public bool IsAuthorized { get; private set; }

	public DateTime AccessedAt { get; private set; }


	private ParkingAccess() { }


	public static ParkingAccess Record(
		Guid userId,
		AccessMethod method,
		bool authorized)
		=> new()
		{
			Id = Guid.NewGuid(),
			UserId = userId,
			Method = method,
			IsAuthorized = authorized,
			AccessedAt = DateTime.UtcNow
		};
}