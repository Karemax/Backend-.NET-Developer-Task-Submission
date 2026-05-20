using FluentValidation;

namespace Project.Application.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Priority).IsInEnum();
    }
}
