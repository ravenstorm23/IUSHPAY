namespace IUSHPAY.Application.DTOs;

public record WalletDto(Guid WalletId, decimal Balance, DateTime LastUpdated);
