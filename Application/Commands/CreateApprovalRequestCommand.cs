using ApprovalFlow.Application.DTOs;
using ApprovalFlow.Domain.Entities;
using ApprovalFlow.Infrastructure.Data;
using MediatR;

namespace ApprovalFlow.Application.Commands;

public record CreateApprovalRequestCommand(
    string Title,
    string Description,
    string RequesterId,
    string RequesterName
) : IRequest<ApprovalRequestResponseDto>;

public class CreateApprovalRequestHandler : IRequestHandler<CreateApprovalRequestCommand, ApprovalRequestResponseDto>
{
    private readonly AppDbContext _context;

    public CreateApprovalRequestHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApprovalRequestResponseDto> Handle(CreateApprovalRequestCommand request, CancellationToken cancellationToken)
    {
        var approvalRequest = new ApprovalRequest
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            RequesterId = request.RequesterId,
            RequesterName = request.RequesterName,
            Status = ApprovalStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var history = new ApprovalHistory
        {
            Id = Guid.NewGuid(),
            ApprovalRequestId = approvalRequest.Id,
            FromStatus = ApprovalStatus.Pending,
            ToStatus = ApprovalStatus.Pending,
            ActorId = request.RequesterId,
            ActorName = request.RequesterName,
            Comments = "Request created",
            Timestamp = DateTime.UtcNow
        };

        approvalRequest.History.Add(history);

        _context.ApprovalRequests.Add(approvalRequest);
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
