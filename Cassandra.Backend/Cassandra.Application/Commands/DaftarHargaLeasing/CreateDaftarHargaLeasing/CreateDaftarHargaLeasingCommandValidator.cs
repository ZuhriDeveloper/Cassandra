using FluentValidation;

namespace Cassandra.Application.Commands.DaftarHargaLeasing.CreateDaftarHargaLeasing;

public class CreateDaftarHargaLeasingCommandValidator : AbstractValidator<CreateDaftarHargaLeasingCommand>
{
    public CreateDaftarHargaLeasingCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.GlobalLeasingId).NotEmpty();
        RuleFor(x => x.GrupTenorId).NotEmpty();
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
