using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.ResponseCompression;

using IUSHPAY.Application;
using IUSHPAY.Infrastructure;
using IUSHPAY.Infrastructure.Persistence;

using IUSHPAY.WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddControllers();

builder.Services.AddResponseCompression(options =>
{
	options.EnableForHttps = true;
	options.Providers.Add<GzipCompressionProvider>();
	options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/json"]);
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
	options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.AddHealthChecks()
	.AddDbContextCheck<AppDbContext>("database");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "IUSHPAY API", Version = "v1" });

	// FIX: Cambiar de ApiKey a Http con scheme bearer
	// Antes Swagger no enviaba el token en el header Authorization correctamente
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization. Ingresa solo el token, sin 'Bearer '",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
			},
			Array.Empty<string>()
		}
	});
});

var app = builder.Build();

app.UseResponseCompression();
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<WebhookSignatureMiddleware>();

app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
	var retries = 3;
	while (retries > 0)
	{
		try
		{
			logger.LogInformation("Aplicando migraciones... (intentos restantes: {Retries})", retries);
			db.Database.Migrate();
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

app.Run();