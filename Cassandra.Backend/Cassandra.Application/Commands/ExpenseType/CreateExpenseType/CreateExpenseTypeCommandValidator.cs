using FluentValidation;

namespace Cassandra.Application.Commands.ExpenseType.CreateExpenseType;

public class CreateExpenseTypeCommandValidator : AbstractValidator<CreateExpenseTypeCommand>
{
    public CreateExpenseTypeCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
