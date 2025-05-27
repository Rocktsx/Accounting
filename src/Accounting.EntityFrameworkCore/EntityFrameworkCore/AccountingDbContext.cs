using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using System.Collections.Generic;
using Accounting.BasicData;
using System;

namespace Accounting.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class AccountingDbContext :
    AbpDbContext<AccountingDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */


    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyAddress> CompanyAddresses { get; set; }
    public DbSet<CompanyContact> CompanyContacts { get; set; }

    public AccountingDbContext(DbContextOptions<AccountingDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();

        /* Configure your own tables/entities inside here */
        
        //currency
        builder.Entity<Currency>(b =>
        {
            b.ToTable(AccountingConsts.DbTablePrefix + "Currencies", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.SourceCurrency).IsRequired().HasMaxLength(CurrencyConsts.MaxCurrencyLength);
            b.Property(x => x.TargetCurrency).IsRequired().HasMaxLength(CurrencyConsts.MaxCurrencyLength);
            b.Property(x => x.SourceAmount).HasColumnType("decimal").HasPrecision(AccountingCommonConsts.AmountPrecision, AccountingCommonConsts.AmountScale);
            b.Property(x => x.TargetAmount).HasColumnType("decimal").HasPrecision(AccountingCommonConsts.AmountPrecision,AccountingCommonConsts.AmountScale);
            b.Property(x => x.ExchangeRate).HasColumnType("decimal").HasPrecision(AccountingCommonConsts.AmountPrecision, AccountingCommonConsts.AmountScale);
            b.Property(x => x.EffectiveDate).HasColumnType("date").HasDefaultValue(new DateOnly(1900,1,1));
            b.HasKey(x => new{ x.TenantId, x.SourceCurrency, x.TargetCurrency});
        });

        //company
        builder.Entity<Company>(b =>
        {
            b.ToTable(AccountingConsts.DbTablePrefix + "Companies", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.Code).IsRequired().HasMaxLength(AccountingCommonConsts.MaxCodeLength);
            b.Property(x =>x.Prefix).IsRequired().HasMaxLength(AccountingCommonConsts.MaxPrefixLength);
            b.Property(x => x.Name).IsRequired().HasMaxLength(CompanyConsts.MaxNameLength);
            b.Property(x => x.OtherName).HasMaxLength(CompanyConsts.MaxNameLength);
            b.Property(x => x.NickName).HasMaxLength(CompanyConsts.MaxNameLength);
            b.Property(x => x.Currency).IsRequired().HasMaxLength(CurrencyConsts.MaxCurrencyLength);
            b.Property(x => x.PaymentTerm).HasMaxLength(CompanyConsts.CommonMaxLength);
            b.Property(x => x.TradeTerm).HasMaxLength(CompanyConsts.CommonMaxLength);
            b.HasMany(x => x.Addresses).WithOne().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(x => x.Contacts).WithOne().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Cascade);
        });

        //company address
        builder.Entity<CompanyAddress>(b =>
        {
            b.ToTable(AccountingConsts.DbTablePrefix + "CompanyAddresses", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.Name).IsRequired().HasMaxLength(CompanyConsts.MaxNameLength);
            b.Property(x => x.Address).HasMaxLength(CompanyAddressConsts.MaxAddressLength);
            b.Property(x => x.ContactPerson).HasMaxLength(CompanyConsts.CommonMaxLength);
            b.Property(x => x.Telephone).HasMaxLength(CompanyConsts.CommonMaxLength);
            b.Property(x => x.Email).HasMaxLength(CompanyConsts.MaxEmailLength);
            b.Property(x => x.Remark).HasMaxLength(CompanyConsts.MaxRemarkLength);
            b.Property(x => x.Country).HasMaxLength(CompanyConsts.CommonMaxLength);
            b.Property(x => x.Region).HasMaxLength(CompanyConsts.CommonMaxLength);
            b.Property(x => x.District).HasMaxLength(CompanyConsts.CommonMaxLength);
            b.Property(x => x.Fax).HasMaxLength(CompanyConsts.CommonMaxLength);
        });

        //company contact
        builder.Entity<CompanyContact>(b =>
        {
            b.ToTable(AccountingConsts.DbTablePrefix + "CompanyContacts", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.ContactName).IsRequired().HasMaxLength(CompanyConsts.CommonMaxLength);
            b.Property(x => x.Department).HasMaxLength(CompanyConsts.CommonMaxLength);
            b.Property(x => x.Position).HasMaxLength(CompanyConsts.CommonMaxLength);
            b.Property(x => x.DirectLine).HasMaxLength(CompanyConsts.CommonMaxLength);
            b.Property(x => x.Telephone).HasMaxLength(CompanyConsts.CommonMaxLength);
            b.Property(x => x.Fax).HasMaxLength(CompanyConsts.CommonMaxLength);
            b.Property(x => x.Email).HasMaxLength(CompanyConsts.MaxEmailLength);
            b.Property(x => x.Remark).HasMaxLength(CompanyConsts.MaxRemarkLength);
        });
    }
}
