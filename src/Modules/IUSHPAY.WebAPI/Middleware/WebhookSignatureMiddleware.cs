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
			if (!context.Request.Headers.ContainsKey("X-Signature"))
			{
				context.Response.StatusCode = 401;
				return;
			}
		}

		await _next(context);
	}
}
