namespace Cassandra.Infrastructure.Persistence.Projections;

public class RegistrasiPenjualanReadModel
{
    public Guid     Id                    { get; set; }
    public Guid     DealerId              { get; set; }
    public string   NoPenjualan           { get; set; } = default!;
    public DateOnly SaleDate              { get; set; }
    public Guid     KaryawanId            { get; set; }
    public Guid     KiosId                { get; set; }
    public Guid?    MediatorId            { get; set; }
    public string   MetodePenjualan       { get; set; } = default!;
    public string   TipePenjualan         { get; set; } = default!;
    public string   NoMesin               { get; set; } = default!;
    public string   NoRangka              { get; set; } = default!;
    public string   NamaCustomer          { get; set; } = default!;
    public string   Address               { get; set; } = default!;
    public string   Phone                 { get; set; } = default!;
    public string?  Phone1                { get; set; }
    public string?  Phone2                { get; set; }
    public decimal  OffRoad               { get; set; }
    public decimal  Bbn                   { get; set; }
    public decimal  Discount              { get; set; }
    public decimal  ApprovedDiscount      { get; set; }
    public decimal  OriginalDiscount      { get; set; }
    public decimal  Total                 { get; set; }
    public decimal  AmbilUang             { get; set; }
    public decimal  Dp                    { get; set; }
    public decimal  Angsuran              { get; set; }
    public decimal  Tac                   { get; set; }
    public Guid?    DaftarHargaLeasingId  { get; set; }
    public string?  TenorCode             { get; set; }
    public string   TipeMotorCode         { get; set; } = default!;
    public string   WarnaName             { get; set; } = default!;
    public string   SerahTerimaKendaraanId { get; set; } = default!;
    public string?  TandaTerimaSementaraId { get; set; }
    public string   Kelengkapan           { get; set; } = default!;
    public bool     IsApproved            { get; set; }
    public bool     IsSent                { get; set; }
    public bool     IsVoid                { get; set; }
    public bool     EnableToVoid          { get; set; }
    public string   CreatedBy             { get; set; } = default!;
    public DateTime CreatedAt             { get; set; }
    public string?  UpdatedBy             { get; set; }
    public DateTime? UpdatedAt            { get; set; }
}
