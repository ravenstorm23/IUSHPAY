using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using IUSHPAY.Infrastructure;
using IUSHPAY.Infrastructure.Persistence;

using IUSHPAY.WebAPI.Middleware;

using IUSHPAY.Application.UseCases.Wallet.GetBalance;
using IUSHPAY.Application.UseCases.Wallet.Recharge;
using IUSHPAY.Application.UseCases.Wallet.GetHistory;

using IUSHPAY.Application.UseCases.Payment.Webhook;

using IUSHPAY.Application.UseCases.Access.GenerateQR;
using IUSHPAY.Application.UseCases.Access.GetLog;
using IUSHPAY.Application.UseCases.Access.ValidateAccess;

using IUSHPAY.Application.UseCases.Auth.Login;
using IUSHPAY.Application.UseCases.Auth.Register;

var builder = WebApplication.CreateBuilder(args);


// =====================================================
// INFRASTRUCTURE
// =====================================================
builder.Services.AddInfrastructure(builder.Configuration);


// =====================================================
// APPLICATION HANDLERS
// =====================================================
builder.Services.AddScoped<GetBalanceHandler>();

builder.Services.AddScoped<RechargeWalletHandler>();

builder.Services.AddScoped<GetTransactionHistoryHandler>();

builder.Services.AddScoped<ProcessPSEWebhookHandler>();

builder.Services.AddScoped<GenerateQRHandler>();

builder.Services.AddScoped<ValidateAccessHandler>();

builder.Services.AddScoped<GetAccessLogHandler>();

builder.Services.AddScoped<LoginHandler>();

builder.Services.AddScoped<RegisterHandler>();


// =====================================================
// CONTROLLERS
// =====================================================
builder.Services.AddControllers();


// =====================================================
// SWAGGER
// =====================================================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1",
		new OpenApiInfo
		{
			Title = "IUSHPAY API",
			Version = "v1"
		});

	c.AddSecurityDefinition("Bearer",
		new OpenApiSecurityScheme
		{
			Description = "JWT Authorization. Usa: Bearer {token}",

			Name = "Authorization",

			In = ParameterLocation.Header,

			Type = SecuritySchemeType.ApiKey,

			Scheme = "Bearer"
		});

	c.AddSecurityRequirement(
		new OpenApiSecurityRequirement
		{
			{
				new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference
					{
						Type = ReferenceType.SecurityScheme,
						Id = "Bearer"
					}
				},
				Array.Empty<string>()
			}
		});
});


// =====================================================
// BUILD
// =====================================================
var app = builder.Build();


// =====================================================
// MIDDLEWARES
// =====================================================
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseMiddleware<WebhookSignatureMiddleware>();


// =====================================================
// SWAGGER
// =====================================================
app.UseSwagger();

app.UseSwaggerUI();


// =====================================================
// AUTH
// =====================================================
app.UseAuthentication();

app.UseAuthorization();


// =====================================================
// CONTROLLERS
// =====================================================
app.MapControllers();


// =====================================================
// AUTO DATABASE MIGRATION + SEED
// =====================================================
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider
		.GetRequiredService<AppDbContext>();

	// 1. Aplica migraciones pendientes
	// db.Database.Migrate();

	// 2. Siembra los 3 admins de TI si aún no existen
	 await DbSeeder.SeedAsync(db);
}


// =====================================================
// RUN
// =====================================================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");