using FluentValidation;

namespace Cassandra.Application.Commands.ExpenseType.UpdateExpenseType;

public class UpdateExpenseTypeCommandValidator : AbstractValidator<UpdateExpenseTypeCommand>
{
    public UpdateExpenseTypeCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
