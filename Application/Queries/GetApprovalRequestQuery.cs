using ApprovalFlow.Application.Common.Exceptions;
using ApprovalFlow.Application.DTOs;
using ApprovalFlow.Domain.Entities;
using ApprovalFlow.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApprovalFlow.Application.Queries;

public record GetApprovalRequestQuery(Guid RequestId) : IRequest<ApprovalRequestResponseDto>;

public class GetApprovalRequestHandler : IRequestHandler<GetApprovalRequestQuery, ApprovalRequestResponseDto>
{
    private readonly AppDbContext _context;

    public GetApprovalRequestHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApprovalRequestResponseDto> Handle(GetApprovalRequestQuery request, CancellationToken cancellationToken)
    {
        var approvalRequest = await _context.ApprovalRequests
            .FirstOrDefaultAsync(a => a.Id == request.RequestId, cancellationToken);

        if (approvalRequest is null)
        {
            throw new NotFoundException($"Approval request with ID {request.RequestId} not found");
        }

        return MapToDto(approvalRequest);
    }

    private static ApprovalRequestResponseDto MapToDto(ApprovalRequest request)
    {
        return new ApprovalRequestResponseDto(
            request.Id,
            request.Title,
            request.Description,
            request.RequesterId,
            request.RequesterName,
            request.Status.ToString(),
            request.CreatedAt,
            request.ReviewedAt,
            request.ReviewerId,
            request.ReviewerName,
            request.ReviewComments
        );
    }
}
