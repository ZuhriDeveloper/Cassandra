using FluentValidation;

namespace Cassandra.Application.Commands.Dealers.RenameDealer;

public class RenameDealerCommandValidator : AbstractValidator<RenameDealerCommand>
{
    public RenameDealerCommandValidator()
    {
        RuleFor(x => x.DealerId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.RenamedBy).NotEmpty();
    }
}
