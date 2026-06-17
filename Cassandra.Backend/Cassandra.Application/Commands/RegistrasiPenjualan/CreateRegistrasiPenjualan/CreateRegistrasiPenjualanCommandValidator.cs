using Cassandra.Domain.RegistrasiPenjualan;
using FluentValidation;

namespace Cassandra.Application.Commands.RegistrasiPenjualan.CreateRegistrasiPenjualan;

public class CreateRegistrasiPenjualanCommandValidator : AbstractValidator<CreateRegistrasiPenjualanCommand>
{
    public CreateRegistrasiPenjualanCommandValidator()
    {
        RuleFor(x => x.NoPenjualan).NotEmpty().WithMessage("Nomor penjualan tidak boleh kosong.");
        RuleFor(x => x.KaryawanId).NotEmpty().WithMessage("Karyawan harus dipilih.");
        RuleFor(x => x.KiosId).NotEmpty().WithMessage("Kios harus dipilih.");
        RuleFor(x => x.MetodePenjualan)
            .Must(m => m == MetodePenjualanConstants.CASH || m == MetodePenjualanConstants.CREDIT)
            .WithMessage("Metode penjualan harus CASH atau CREDIT.");
        RuleFor(x => x.TipePenjualan)
            .Must(t => t == TipePenjualanConstants.DIRECT || t == TipePenjualanConstants.KIOS || t == TipePenjualanConstants.MEDIATOR)
            .WithMessage("Tipe penjualan harus DIRECT, KIOS, atau MEDIATOR.");
        RuleFor(x => x.NoMesin).NotEmpty().WithMessage("Nomor mesin tidak boleh kosong.");
        RuleFor(x => x.NoRangka).NotEmpty().WithMessage("Nomor rangka tidak boleh kosong.");
        RuleFor(x => x.NamaCustomer).NotEmpty().WithMessage("Nama customer tidak boleh kosong.");
        RuleFor(x => x.Phone).NotEmpty().WithMessage("Nomor telepon tidak boleh kosong.");
        RuleFor(x => x.Total).GreaterThan(0).WithMessage("Total harga harus lebih dari nol.");
        RuleFor(x => x.SerahTerimaKendaraanId).NotEmpty().WithMessage("Nomor serah terima kendaraan tidak boleh kosong.");
        RuleFor(x => x.MediatorId)
            .NotEmpty()
            .When(x => x.TipePenjualan == TipePenjualanConstants.MEDIATOR)
            .WithMessage("Mediator harus dipilih untuk tipe penjualan MEDIATOR.");
        RuleFor(x => x.DaftarHargaLeasingId)
            .NotEmpty()
            .When(x => x.MetodePenjualan == MetodePenjualanConstants.CREDIT)
            .WithMessage("Daftar harga leasing harus dipilih untuk penjualan kredit.");
    }
}
