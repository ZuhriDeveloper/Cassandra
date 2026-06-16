using System.Text;
using Cassandra.Application.Contracts.Auth;
using Cassandra.Infrastructure.Auth;
using Cassandra.Infrastructure.Identity;
using Cassandra.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Cassandra.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("cassandradb")));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        var jwtSection = configuration.GetSection("Jwt");
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSection["Key"]!)),
                };
            });

        services.AddScoped<IUserAuthRepository, UserAuthRepository>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IUserProvisioningService, UserProvisioningService>();
        services.AddScoped<DatabaseInitializer>();

        // ── Multi-tenant (dealer scoping) ───────────────────────────────────────
        services.AddHttpContextAccessor();
        services.AddScoped<Application.Contracts.Dealers.ICurrentDealer, Dealers.CurrentDealer>();
        services.AddScoped<Application.Contracts.Dealers.IDealerRepository, Dealers.DealerRepository>();
        services.AddScoped<Application.Contracts.Dealers.IDealerQueryRepository, Dealers.DealerQueryRepository>();

        // ── Jabatan ──────────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.Jabatan.IJabatanRepository, Jabatan.JabatanRepository>();
        services.AddScoped<Application.Contracts.Jabatan.IJabatanQueryRepository, Jabatan.JabatanQueryRepository>();

        // ── Karyawan ─────────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.Karyawan.IKaryawanRepository, Karyawan.KaryawanRepository>();
        services.AddScoped<Application.Contracts.Karyawan.IKaryawanQueryRepository, Karyawan.KaryawanQueryRepository>();

        // ── Kios ─────────────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.Kios.IKiosRepository, Kios.KiosRepository>();
        services.AddScoped<Application.Contracts.Kios.IKiosQueryRepository, Kios.KiosQueryRepository>();

        // ── Mediator ──────────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.Mediator.IMediatorRepository, Mediator.MediatorRepository>();
        services.AddScoped<Application.Contracts.Mediator.IMediatorQueryRepository, Mediator.MediatorQueryRepository>();

        // ── Warna ─────────────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.Warna.IWarnaRepository, Warna.WarnaRepository>();
        services.AddScoped<Application.Contracts.Warna.IWarnaQueryRepository, Warna.WarnaQueryRepository>();

        // ── GrupTipeMotor ─────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.GrupTipeMotor.IGrupTipeMotorRepository, GrupTipeMotor.GrupTipeMotorRepository>();
        services.AddScoped<Application.Contracts.GrupTipeMotor.IGrupTipeMotorQueryRepository, GrupTipeMotor.GrupTipeMotorQueryRepository>();

        // ── TipeMotor ─────────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.TipeMotor.ITipeMotorRepository, TipeMotor.TipeMotorRepository>();
        services.AddScoped<Application.Contracts.TipeMotor.ITipeMotorQueryRepository, TipeMotor.TipeMotorQueryRepository>();

        // ── Kelengkapan ───────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.Kelengkapan.IKelengkapanRepository, Kelengkapan.KelengkapanRepository>();
        services.AddScoped<Application.Contracts.Kelengkapan.IKelengkapanQueryRepository, Kelengkapan.KelengkapanQueryRepository>();

        // ── MetodeKeuangan ────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.MetodeKeuangan.IMetodeKeuanganRepository, MetodeKeuangan.MetodeKeuanganRepository>();
        services.AddScoped<Application.Contracts.MetodeKeuangan.IMetodeKeuanganQueryRepository, MetodeKeuangan.MetodeKeuanganQueryRepository>();

        // ── GlobalLeasing ─────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.GlobalLeasing.IGlobalLeasingRepository, GlobalLeasing.GlobalLeasingRepository>();
        services.AddScoped<Application.Contracts.GlobalLeasing.IGlobalLeasingQueryRepository, GlobalLeasing.GlobalLeasingQueryRepository>();

        // ── CabangLeasing ─────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.CabangLeasing.ICabangLeasingRepository, CabangLeasing.CabangLeasingRepository>();
        services.AddScoped<Application.Contracts.CabangLeasing.ICabangLeasingQueryRepository, CabangLeasing.CabangLeasingQueryRepository>();

        // ── GrupTenor ─────────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.GrupTenor.IGrupTenorRepository, GrupTenor.GrupTenorRepository>();
        services.AddScoped<Application.Contracts.GrupTenor.IGrupTenorQueryRepository, GrupTenor.GrupTenorQueryRepository>();

        // ── Tenor ─────────────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.Tenor.ITenorRepository, Tenor.TenorRepository>();
        services.AddScoped<Application.Contracts.Tenor.ITenorQueryRepository, Tenor.TenorQueryRepository>();

        // ── Df ────────────────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.Df.IDfRepository, Df.DfRepository>();
        services.AddScoped<Application.Contracts.Df.IDfQueryRepository, Df.DfQueryRepository>();

        // ── DaftarHargaLeasing ────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.DaftarHargaLeasing.IDaftarHargaLeasingRepository, DaftarHargaLeasing.DaftarHargaLeasingRepository>();
        services.AddScoped<Application.Contracts.DaftarHargaLeasing.IDaftarHargaLeasingQueryRepository, DaftarHargaLeasing.DaftarHargaLeasingQueryRepository>();

        // ── Discount ──────────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.Discount.IDiscountRepository, Discount.DiscountRepository>();
        services.AddScoped<Application.Contracts.Discount.IDiscountQueryRepository, Discount.DiscountQueryRepository>();

        // ── DiscountCash ──────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.DiscountCash.IDiscountCashRepository, DiscountCash.DiscountCashRepository>();
        services.AddScoped<Application.Contracts.DiscountCash.IDiscountCashQueryRepository, DiscountCash.DiscountCashQueryRepository>();

        // ── AlokasiDiskon ─────────────────────────────────────────────────────────
        services.AddScoped<Application.Contracts.AlokasiDiskon.IAlokasiDiskonRepository, AlokasiDiskon.AlokasiDiskonRepository>();
        services.AddScoped<Application.Contracts.AlokasiDiskon.IAlokasiDiskonQueryRepository, AlokasiDiskon.AlokasiDiskonQueryRepository>();

        return services;
    }
}
