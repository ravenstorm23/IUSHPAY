namespace IUSHPAY.Domain.ValueObjects;

public record Money(decimal Amount)
{
	public static Money Zero => new(0);

	public Money Add(Money other) => new(Amount + other.Amount);
	public Money Subtract(Money other) => new(Amount - other.Amount);
}
