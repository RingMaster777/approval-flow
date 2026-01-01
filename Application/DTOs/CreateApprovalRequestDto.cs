namespace ApprovalFlow.Application.DTOs;

public record CreateApprovalRequestDto(
    string Title,
    string Description,
    string RequesterId,
    string RequesterName
);
