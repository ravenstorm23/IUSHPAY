using IUSHPAY.Domain.Exceptions;

namespace IUSHPAY.Domain.Entities;

public class Wallet
{
	public Guid Id { get; private set; }

	public Guid UserId { get; private set; }

	public User User { get; private set; } = null!;

	public decimal Balance { get; private set; }

	public DateTime UpdatedAt { get; private set; }


	private readonly List<Transaction> _transactions = new();

	public IReadOnlyCollection<Transaction> Transactions
		=> _transactions.AsReadOnly();


	private Wallet() { }


	internal static Wallet CreateFor(Guid userId)
		=> new()
		{
			Id = Guid.NewGuid(),
			UserId = userId,
			Balance = 0,
			UpdatedAt = DateTime.UtcNow
		};


	public void Credit(decimal amount, string reference = "")
	{
		if (amount <= 0)
			throw new DomainException("Monto inválido");

		Balance += amount;

		UpdatedAt = DateTime.UtcNow;

		_transactions.Add(
			Transaction.Credit(Id, amount, reference));
	}


	public void Debit(decimal amount, string reference = "")
	{
		if (amount <= 0)
			throw new DomainException("Monto inválido");

		if (amount > Balance)
			throw new InsufficientBalanceException(Balance, amount);

		Balance -= amount;

		UpdatedAt = DateTime.UtcNow;

		_transactions.Add(
			Transaction.Debit(Id, amount, reference));
	}
}