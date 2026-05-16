using IUSHPAY.Application.Common.Models;
using IUSHPAY.Domain.Interfaces.Services;

namespace IUSHPAY.Application.UseCases.Access.GenerateQR;

public class GenerateQRHandler
{
	private readonly IQRGeneratorService _qr;

	public GenerateQRHandler(IQRGeneratorService qr)
	{
		_qr = qr;
	}

	public async Task<Result<string>> HandleAsync(GenerateQRCommand cmd)
	{
		var token = await _qr.GenerateAsync(cmd.UserId);
		return Result<string>.Success(token);
	}
}