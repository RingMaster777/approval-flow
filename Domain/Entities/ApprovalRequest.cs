namespace ApprovalFlow.Domain.Entities;

public class ApprovalRequest
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RequesterId { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
    public ApprovalStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewerId { get; set; }
    public string? ReviewerName { get; set; }
    public string? ReviewComments { get; set; }
    
    public List<ApprovalHistory> History { get; set; } = new();
}

public enum ApprovalStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}
