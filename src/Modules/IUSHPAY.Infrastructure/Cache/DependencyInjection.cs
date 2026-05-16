using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using IUSHPAY.Infrastructure.Persistence;
using IUSHPAY.Infrastructure.Persistence.Repositories;

using IUSHPAY.Infrastructure.ExternalServices.PSE;
using IUSHPAY.Infrastructure.ExternalServices.QR;

using IUSHPAY.Infrastructure.Cache;
using IUSHPAY.Infrastructure.Services;

using IUSHPAY.Domain.Interfaces.Repositories;
using IUSHPAY.Domain.Interfaces.Services;

using IUSHPAY.Application.Common.Interfaces;

namespace IUSHPAY.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services,
		IConfiguration config)
	{
		// =====================================================
		// DATABASE MYSQL
		// =====================================================
		var connectionString =
			config.GetConnectionString("DefaultConnection")
			?? throw new InvalidOperationException(
				"Connection string 'DefaultConnection' no configurada");

		services.AddDbContext<AppDbContext>(options =>
				options.UseNpgsql(
			connectionString,
			npgsqlOptions =>
			{
				npgsqlOptions.MigrationsAssembly(
					typeof(AppDbContext).Assembly.FullName);

				// Reintentar hasta 3 veces ante fallos transitorios (cold start de Supabase/Render)
				npgsqlOptions.EnableRetryOnFailure(
					maxRetryCount: 3,
					maxRetryDelay: TimeSpan.FromSeconds(5),
					errorCodesToAdd: null);

				// Timeout de comando explícito
				npgsqlOptions.CommandTimeout(15);
			}
		)
		);

		// =====================================================
		// CACHE EN MEMORIA
		// =====================================================
		services.AddMemoryCache();

		services.AddScoped<ICacheService, MemoryCacheService>();


		// =====================================================
		// REPOSITORIES
		// =====================================================
		services.AddScoped<IUserRepository, UserRepository>();

		services.AddScoped<IWalletRepository, WalletRepository>();

		services.AddScoped<ITransactionRepository, TransactionRepository>();

		services.AddScoped<IParkingAccessRepository, ParkingAccessRepository>();


		// =====================================================
		// EXTERNAL SERVICES
		// =====================================================
		services.AddScoped<IPaymentGatewayService, PSEPaymentService>();

		services.AddScoped<IQRService, QRGeneratorService>();


		// =====================================================
		// APPLICATION SERVICES
		// =====================================================
		services.AddScoped<IJwtService, JwtService>();

		services.AddHttpContextAccessor();

		services.AddScoped<ICurrentUserService, CurrentUserService>();


		// =====================================================
		// JWT AUTHENTICATION
		// =====================================================
		var jwtKey =
			config["Jwt:Key"]
			?? throw new InvalidOperationException(
				"JWT Key no configurada");

		services
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters =
					new TokenValidationParameters
					{
						ValidateIssuer = true,

						ValidateAudience = true,

						ValidateLifetime = true,

						ValidateIssuerSigningKey = true,

						ValidIssuer = config["Jwt:Issuer"],

						ValidAudience = config["Jwt:Audience"],

						IssuerSigningKey =
							new SymmetricSecurityKey(
								Encoding.UTF8.GetBytes(jwtKey))
					};
			});


		// =====================================================
		// AUTHORIZATION POLICIES
		// =====================================================
		services
			.AddAuthorizationBuilder()
			.AddPolicy("AdminOnly",
				policy => policy.RequireRole("Admin"))

			.AddPolicy("UserOrAdmin",
				policy => policy.RequireRole("User", "Admin"));


		return services;
	}
}