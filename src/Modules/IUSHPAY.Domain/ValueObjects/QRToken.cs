namespace IUSHPAY.Domain.ValueObjects;

public record QRToken(string Value, DateTime Expiration)
{
	public bool IsValid() => DateTime.UtcNow <= Expiration;
}
