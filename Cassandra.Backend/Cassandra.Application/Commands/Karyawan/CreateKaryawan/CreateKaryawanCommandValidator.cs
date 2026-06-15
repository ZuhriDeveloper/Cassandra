using FluentValidation;

namespace Cassandra.Application.Commands.Karyawan.CreateKaryawan;

public class CreateKaryawanCommandValidator : AbstractValidator<CreateKaryawanCommand>
{
    public CreateKaryawanCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().MaximumLength(200).EmailAddress();
        RuleFor(x => x.KtpNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.PhoneAlt).MaximumLength(20).When(x => x.PhoneAlt is not null);
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.SalesLimit).GreaterThanOrEqualTo(0);
        RuleFor(x => x.JabatanId).NotEmpty();
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
