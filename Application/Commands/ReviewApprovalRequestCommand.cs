using ApprovalFlow.Application.Common.Exceptions;
using ApprovalFlow.Application.DTOs;
using ApprovalFlow.Domain.Entities;
using ApprovalFlow.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApprovalFlow.Application.Commands;

public record ReviewApprovalRequestCommand(
    Guid RequestId,
    bool IsApproved,
    string ReviewerId,
    string ReviewerName,
    string? Comments
) : IRequest<ApprovalRequestResponseDto>;

public class ReviewApprovalRequestHandler : IRequestHandler<ReviewApprovalRequestCommand, ApprovalRequestResponseDto>
{
    private readonly AppDbContext _context;

    public ReviewApprovalRequestHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApprovalRequestResponseDto> Handle(ReviewApprovalRequestCommand request, CancellationToken cancellationToken)
    {
        var approvalRequest = await _context.ApprovalRequests
            .Include(a => a.History)
            .FirstOrDefaultAsync(a => a.Id == request.RequestId, cancellationToken);

        if (approvalRequest is null)
        {
            throw new NotFoundException($"Approval request with ID {request.RequestId} not found");
        }

        if (approvalRequest.Status != ApprovalStatus.Pending)
        {
            throw new BusinessRuleException($"Cannot review request. Current status: {approvalRequest.Status}");
        }

        var oldStatus = approvalRequest.Status;
        var newStatus = request.IsApproved ? ApprovalStatus.Approved : ApprovalStatus.Rejected;

        approvalRequest.Status = newStatus;
        approvalRequest.ReviewedAt = DateTime.UtcNow;
        approvalRequest.ReviewerId = request.ReviewerId;
        approvalRequest.ReviewerName = request.ReviewerName;
        approvalRequest.ReviewComments = request.Comments;

        var history = new ApprovalHistory
        {
            Id = Guid.NewGuid(),
            ApprovalRequestId = approvalRequest.Id,
            FromStatus = oldStatus,
            ToStatus = newStatus,
            ActorId = request.ReviewerId,
            ActorName = request.ReviewerName,
            Comments = request.Comments,
            Timestamp = DateTime.UtcNow
        };

        approvalRequest.History.Add(history);

        await _context.SaveChangesAsync(cancellationToken);

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
