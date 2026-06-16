using FluentValidation;

namespace Cassandra.Application.Commands.DaftarHargaLeasing.UpdateDaftarHargaLeasing;

public class UpdateDaftarHargaLeasingCommandValidator : AbstractValidator<UpdateDaftarHargaLeasingCommand>
{
    public UpdateDaftarHargaLeasingCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.GlobalLeasingId).NotEmpty();
        RuleFor(x => x.GrupTenorId).NotEmpty();
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
