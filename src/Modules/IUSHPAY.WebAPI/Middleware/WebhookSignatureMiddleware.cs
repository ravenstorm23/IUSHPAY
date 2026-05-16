using Microsoft.AspNetCore.Http;

namespace IUSHPAY.WebAPI.Middleware;

public class WebhookSignatureMiddleware
{
	private readonly RequestDelegate _next;

	public WebhookSignatureMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		if (context.Request.Path.StartsWithSegments("/api/payment/webhook"))
		{
			// Si no viene X-Signature (pruebas desde Swagger o frontend),
			// se inyecta una firma simulada automáticamente.
			// En producción real PSE enviará su propia firma HMAC.
			if (!context.Request.Headers.ContainsKey("X-Signature"))
			{
				context.Request.Headers["X-Signature"] = "simulado";
			}
		}

		await _next(context);
	}
}