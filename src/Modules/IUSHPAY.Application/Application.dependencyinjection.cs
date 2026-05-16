using Microsoft.Extensions.DependencyInjection;

using IUSHPAY.Application.UseCases.Auth.Login;
using IUSHPAY.Application.UseCases.Auth.Register;

using IUSHPAY.Application.UseCases.Wallet.GetBalance;
using IUSHPAY.Application.UseCases.Wallet.Recharge;
using IUSHPAY.Application.UseCases.Wallet.GetHistory;

using IUSHPAY.Application.UseCases.Payment.Webhook;

using IUSHPAY.Application.UseCases.Access.GenerateQR;
using IUSHPAY.Application.UseCases.Access.ValidateAccess;
using IUSHPAY.Application.UseCases.Access.GetLog;

namespace IUSHPAY.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(
		this IServiceCollection services)
	{
		// =====================================================
		// AUTH HANDLERS
		// =====================================================
		services.AddScoped<LoginHandler>();

		services.AddScoped<RegisterHandler>();


		// =====================================================
		// WALLET HANDLERS
		// =====================================================
		services.AddScoped<GetBalanceHandler>();

		services.AddScoped<RechargeWalletHandler>();

		services.AddScoped<GetTransactionHistoryHandler>();


		// =====================================================
		// PAYMENT HANDLERS
		// =====================================================
		services.AddScoped<ProcessPSEWebhookHandler>();


		// =====================================================
		// ACCESS HANDLERS
		// =====================================================
		services.AddScoped<GenerateQRHandler>();

		services.AddScoped<ValidateAccessHandler>();

		services.AddScoped<GetAccessLogHandler>();


		return services;
	}
}