namespace ApprovalFlow.Application.DTOs;

public record ReviewApprovalRequestDto(
    bool IsApproved,
    string ReviewerId,
    string ReviewerName,
    string? Comments
);
