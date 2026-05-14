using IUSHPAY.Application.Common.Models;
using IUSHPAY.Application.DTOs;
using IUSHPAY.Domain.Interfaces.Repositories;

namespace IUSHPAY.Application.UseCases.Access.GetLog;

public class GetAccessLogHandler
{
	private readonly IParkingAccessRepository _repo;

	public GetAccessLogHandler(IParkingAccessRepository repo)
	{
		_repo = repo;
	}

	public async Task<Result<IReadOnlyList<AccessLogDto>>> HandleAsync(GetAccessLogQuery query)
	{
		var records = await _repo.GetAllAsync(query.From, query.To);

		var dtos = records.Select(r => new AccessLogDto(
			AccessId: r.Id,
			UserId: r.UserId,
			UserFullName: r.User?.FullName ?? "–",
			UserCarnet: r.User?.CarnetNumber ?? "–",
			Method: r.Method,
			IsAuthorized: r.IsAuthorized,
			AccessedAt: r.AccessedAt
		)).ToList();

		return Result<IReadOnlyList<AccessLogDto>>.Success(dtos);
	}
}