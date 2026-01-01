using ApprovalFlow.Domain.Entities;

namespace ApprovalFlow.Application.DTOs;

public record ApprovalRequestResponseDto(
    Guid Id,
    string Title,
    string Description,
    string RequesterId,
    string RequesterName,
    string Status,
    DateTime CreatedAt,
    DateTime? ReviewedAt,
    string? ReviewerId,
    string? ReviewerName,
    string? ReviewComments
);

public record ApprovalHistoryDto(
    Guid Id,
    string FromStatus,
    string ToStatus,
    string ActorName,
    string? Comments,
    DateTime Timestamp
);
