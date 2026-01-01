namespace ApprovalFlow.Domain.Entities;

public class ApprovalHistory
{
    public Guid Id { get; set; }
    public Guid ApprovalRequestId { get; set; }
    public ApprovalStatus FromStatus { get; set; }
    public ApprovalStatus ToStatus { get; set; }
    public string ActorId { get; set; } = string.Empty;
    public string ActorName { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public DateTime Timestamp { get; set; }
    
    public ApprovalRequest ApprovalRequest { get; set; } = null!;
}
