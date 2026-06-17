using FluentValidation;

namespace Cassandra.Application.Commands.Stnk.CreateStnk;

public class CreateStnkCommandValidator : AbstractValidator<CreateStnkCommand>
{
    public CreateStnkCommandValidator()
    {
        RuleFor(x => x.RegistrasiPenjualanId).NotEmpty();
        RuleFor(x => x.FakturName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.FakturAddress).NotEmpty().MaximumLength(500);
    }
}
