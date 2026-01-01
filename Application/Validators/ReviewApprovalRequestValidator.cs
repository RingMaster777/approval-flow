using ApprovalFlow.Application.Commands;
using FluentValidation;

namespace ApprovalFlow.Application.Validators;

public class ReviewApprovalRequestValidator : AbstractValidator<ReviewApprovalRequestCommand>
{
    public ReviewApprovalRequestValidator()
    {
        RuleFor(x => x.RequestId)
            .NotEmpty().WithMessage("Request ID is required");

        RuleFor(x => x.ReviewerId)
            .NotEmpty().WithMessage("Reviewer ID is required")
            .MaximumLength(100).WithMessage("Reviewer ID cannot exceed 100 characters");

        RuleFor(x => x.ReviewerName)
            .NotEmpty().WithMessage("Reviewer name is required")
            .MaximumLength(100).WithMessage("Reviewer name cannot exceed 100 characters");

        RuleFor(x => x.Comments)
            .MaximumLength(500).WithMessage("Comments cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Comments));
    }
}
