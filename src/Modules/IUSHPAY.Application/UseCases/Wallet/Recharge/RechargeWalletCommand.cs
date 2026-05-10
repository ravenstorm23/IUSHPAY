namespace IUSHPAY.Application.UseCases.Wallet.Recharge;

public record RechargeWalletCommand(Guid UserId, decimal Amount);