namespace IUSHPAY.Domain.Exceptions;

public class InsufficientBalanceException : DomainException
{
	public InsufficientBalanceException(decimal current, decimal requested)
		: base($"Saldo insuficiente. Disponible: {current}, requerido: {requested}")
	{
	}
}