using ApprovalFlow.Application.DTOs;
using ApprovalFlow.Domain.Entities;
using ApprovalFlow.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApprovalFlow.Application.Queries;

public record GetAllApprovalRequestsQuery(string? Status = null) : IRequest<List<ApprovalRequestResponseDto>>;

public class GetAllApprovalRequestsHandler : IRequestHandler<GetAllApprovalRequestsQuery, List<ApprovalRequestResponseDto>>
{
    private readonly AppDbContext _context;

    public GetAllApprovalRequestsHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ApprovalRequestResponseDto>> Handle(GetAllApprovalRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ApprovalRequests.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Status) && 
            Enum.TryParse<ApprovalStatus>(request.Status, true, out var status))
        {
            query = query.Where(a => a.Status == status);
        }

        var requests = await query
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new ApprovalRequestResponseDto(
                a.Id,
                a.Title,
                a.Description,
                a.RequesterId,
                a.RequesterName,
                a.Status.ToString(),
                a.CreatedAt,
                a.ReviewedAt,
                a.ReviewerId,
                a.ReviewerName,
                a.ReviewComments
            ))
            .ToListAsync(cancellationToken);

        return requests;
    }
}
