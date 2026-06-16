using FluentValidation;

namespace Cassandra.Application.Commands.BiayaBiroJasa.CreateBiayaBiroJasa;

public class CreateBiayaBiroJasaCommandValidator : AbstractValidator<CreateBiayaBiroJasaCommand>
{
    public CreateBiayaBiroJasaCommandValidator()
    {
        RuleFor(x => x.SamsatId).NotEmpty();
        RuleFor(x => x.BiroId).NotEmpty();
    }
}
