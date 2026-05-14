namespace IUSHPAY.Application.DTOs;

public record TransactionDto(
	Guid Id,
	decimal Amount,
	string Type,
	string Status,
	DateTime Date
);
