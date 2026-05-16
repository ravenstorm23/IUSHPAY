using System.Text;
using FluentValidation;

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
		// DATABASE POSTGRESQL
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

					npgsqlOptions.CommandTimeout(30);
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

		services.AddScoped<IQRGeneratorService, QRGeneratorService>();

		services.AddScoped<IQRValidatorService, QRValidatorService>();

		// =====================================================
		// FLUENT VALIDATION
		// =====================================================
		services.AddValidatorsFromAssembly(
			typeof(IUSHPAY.Application.DependencyInjection).Assembly);

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

		// DEBUG: imprime la key y config al arrancar para verificar
		Console.WriteLine($">>> JWT KEY LENGTH: {jwtKey.Length}");
		Console.WriteLine($">>> JWT ISSUER: {config["Jwt:Issuer"]}");
		Console.WriteLine($">>> JWT AUDIENCE: {config["Jwt:Audience"]}");

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

				// DEBUG: logs de JWT para ver exactamente por qué falla
				options.Events = new JwtBearerEvents
				{
					OnAuthenticationFailed = context =>
					{
						Console.WriteLine($">>> JWT FAILED: {context.Exception.GetType().Name}: {context.Exception.Message}");
						return Task.CompletedTask;
					},
					OnChallenge = context =>
					{
						Console.WriteLine($">>> JWT CHALLENGE: {context.Error} - {context.ErrorDescription}");
						return Task.CompletedTask;
					},
					OnTokenValidated = context =>
					{
						Console.WriteLine(">>> JWT OK - Token validado correctamente");
						return Task.CompletedTask;
					}
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