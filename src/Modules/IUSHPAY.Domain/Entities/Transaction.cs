using IUSHPAY.Domain.Enums;

namespace IUSHPAY.Domain.Entities;

public class Transaction
{
	public Guid Id { get; private set; }

	public Guid WalletId { get; private set; }

	public Wallet Wallet { get; private set; } = null!;

	public decimal Amount { get; private set; }

	public TransactionStatus Status { get; private set; }

	public TransactionType Type { get; private set; }

	public string Reference { get; private set; } = string.Empty;

	public DateTime CreatedAt { get; private set; }


	private Transaction() { }


	public static Transaction Credit(
		Guid walletId,
		decimal amount,
		string reference)
		=> new()
		{
			Id = Guid.NewGuid(),
			WalletId = walletId,
			Amount = amount,
			Type = TransactionType.Credit,
			Status = TransactionStatus.Completed,
			Reference = reference,
			CreatedAt = DateTime.UtcNow
		};


	public static Transaction Debit(
		Guid walletId,
		decimal amount,
		string reference)
		=> new()
		{
			Id = Guid.NewGuid(),
			WalletId = walletId,
			Amount = amount,
			Type = TransactionType.Debit,
			Status = TransactionStatus.Completed,
			Reference = reference,
			CreatedAt = DateTime.UtcNow
		};
}