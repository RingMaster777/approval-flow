using ApprovalFlow.Application.Common.Exceptions;
using ApprovalFlow.Application.DTOs;
using ApprovalFlow.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApprovalFlow.Application.Queries;

public record GetApprovalHistoryQuery(Guid RequestId) : IRequest<List<ApprovalHistoryDto>>;

public class GetApprovalHistoryHandler : IRequestHandler<GetApprovalHistoryQuery, List<ApprovalHistoryDto>>
{
    private readonly AppDbContext _context;

    public GetApprovalHistoryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ApprovalHistoryDto>> Handle(GetApprovalHistoryQuery request, CancellationToken cancellationToken)
    {
        var exists = await _context.ApprovalRequests
            .AnyAsync(a => a.Id == request.RequestId, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException($"Approval request with ID {request.RequestId} not found");
        }

        var history = await _context.ApprovalHistories
            .Where(h => h.ApprovalRequestId == request.RequestId)
            .OrderBy(h => h.Timestamp)
            .Select(h => new ApprovalHistoryDto(
                h.Id,
                h.FromStatus.ToString(),
                h.ToStatus.ToString(),
                h.ActorName,
                h.Comments,
                h.Timestamp
            ))
            .ToListAsync(cancellationToken);

        return history;
    }
}
