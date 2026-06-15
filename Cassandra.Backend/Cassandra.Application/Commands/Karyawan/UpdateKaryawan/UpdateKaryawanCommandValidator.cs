using FluentValidation;

namespace Cassandra.Application.Commands.Karyawan.UpdateKaryawan;

public class UpdateKaryawanCommandValidator : AbstractValidator<UpdateKaryawanCommand>
{
    public UpdateKaryawanCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().MaximumLength(200).EmailAddress();
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.PhoneAlt).MaximumLength(20).When(x => x.PhoneAlt is not null);
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.JabatanId).NotEmpty();
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
