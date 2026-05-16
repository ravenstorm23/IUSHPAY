using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.ResponseCompression;

using IUSHPAY.Application;
using IUSHPAY.Infrastructure;
using IUSHPAY.Infrastructure.Persistence;

using IUSHPAY.WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);


// =====================================================
// INFRASTRUCTURE
// =====================================================
builder.Services.AddInfrastructure(builder.Configuration);


// =====================================================
// APPLICATION HANDLERS
// =====================================================
builder.Services.AddApplication();


// =====================================================
// CONTROLLERS
// =====================================================
builder.Services.AddControllers();


// =====================================================
// RESPONSE COMPRESSION (reduce tamaño payload ~70%)
// =====================================================
builder.Services.AddResponseCompression(options =>
{
	options.EnableForHttps = true;
	options.Providers.Add<GzipCompressionProvider>();
	options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
		["application/json"]);
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
	options.Level = System.IO.Compression.CompressionLevel.Fastest;
});


// =====================================================
// HEALTH CHECK (para keep-alive y monitoreo)
// =====================================================
builder.Services.AddHealthChecks()
	.AddDbContextCheck<AppDbContext>("database");


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
app.UseResponseCompression(); // debe ir primero para comprimir todas las respuestas

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
// HEALTH CHECK ENDPOINT
// Úsalo desde un cron externo (UptimeRobot, cron-job.org)
// cada 10 min para evitar el cold start de Render
// GET /health → 200 OK {"status":"Healthy"}
// =====================================================
app.MapHealthChecks("/health");


// =====================================================
// AUTO DATABASE MIGRATION + SEED
// FIX: EnableRetryOnFailure no es compatible con Migrate(),
//      se implementa reintento manual con 3 intentos y 5s de espera.
// =====================================================
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	var logger = scope.ServiceProvider
		.GetRequiredService<ILogger<AppDbContext>>();

	var retries = 3;
	while (retries > 0)
	{
		try
		{
			logger.LogInformation("Aplicando migraciones... (intentos restantes: {Retries})", retries);

			// 1. Aplica migraciones pendientes
			db.Database.Migrate();

			// 2. Siembra los 3 admins de TI si aún no existen
			await DbSeeder.SeedAsync(db);

			logger.LogInformation("Migraciones aplicadas correctamente.");
			break;
		}
		catch (Exception ex)
		{
			retries--;
			logger.LogError(ex, "Error al aplicar migraciones. Intentos restantes: {Retries}", retries);

			if (retries == 0)
			{
				logger.LogCritical("No se pudo conectar a la base de datos después de 3 intentos.");
				throw;
			}

			await Task.Delay(TimeSpan.FromSeconds(5));
		}
	}
}


// =====================================================
// RUN
// =====================================================
app.Run();
