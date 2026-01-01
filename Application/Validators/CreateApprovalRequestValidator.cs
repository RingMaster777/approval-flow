using ApprovalFlow.Application.Commands;
using FluentValidation;

namespace ApprovalFlow.Application.Validators;

public class CreateApprovalRequestValidator : AbstractValidator<CreateApprovalRequestCommand>
{
    public CreateApprovalRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.RequesterId)
            .NotEmpty().WithMessage("Requester ID is required")
            .MaximumLength(100).WithMessage("Requester ID cannot exceed 100 characters");

        RuleFor(x => x.RequesterName)
            .NotEmpty().WithMessage("Requester name is required")
            .MaximumLength(100).WithMessage("Requester name cannot exceed 100 characters");
    }
}
