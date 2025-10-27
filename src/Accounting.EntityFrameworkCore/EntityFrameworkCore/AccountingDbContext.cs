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
using Accounting.Finance;
using Accounting.Finance.Vouchers;
using Accounting.Finance.AccountTypes;
using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.Subjects;
using Accounting.Finance.SubjectCategories;
using Accounting.BasicData.Companies;
using Accounting.BasicData.Currencies;

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

    public DbSet<AccountingPeriod> AccountingPeriods { get; set; }
    public DbSet<AccountType> AccountTypes { get; set; }
    public DbSet<SubjectCategory> SubjectCategories { get; set; }
    public DbSet<Subject> Subjects { get; set; }

    public DbSet<Voucher> Vouchers { get; set; }
    public DbSet<VoucherDetail> VoucherDetails { get; set; }

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
        ConfigureCurrency(builder);
        ConfigureCompany(builder);
        ConfigureCompanyAddress(builder);
        ConfigureCompanyContact(builder);
        ConfigureAccountingPeriod(builder);
        ConfigureAccountType(builder);
        ConfigureSubjectCategory(builder);
        ConfigureSubject(builder);
        ConfigureVoucher(builder);
        ConfigureVoucherDetail(builder);
    }
    protected static void ConfigureCurrency(ModelBuilder builder)
    {
        builder.Entity<Currency>(b =>
        {
            b.ToTable(AccountingConsts.DbTablePrefix + "Currencies", AccountingConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.SourceCurrency).IsRequired().HasMaxLength(CurrencyConsts.MaxCurrencyLength);
            b.Property(x => x.TargetCurrency).IsRequired().HasMaxLength(CurrencyConsts.MaxCurrencyLength);
            b.Property(x => x.SourceAmount).HasColumnType("decimal").HasPrecision(AccountingCommonConsts.AmountPrecision, AccountingCommonConsts.AmountScale);
            b.Property(x => x.TargetAmount).HasColumnType("decimal").HasPrecision(AccountingCommonConsts.AmountPrecision, AccountingCommonConsts.AmountScale);
            b.Property(x => x.ExchangeRate).HasColumnType("decimal").HasPrecision(AccountingCommonConsts.AmountPrecision, AccountingCommonConsts.AmountScale);
            b.Property(x => x.EffectiveDate).HasColumnType("date").HasDefaultValue(new DateOnly(1900, 1, 1));
            b.HasIndex(x => new { x.TenantId, x.SourceCurrency, x.TargetCurrency }).IsUnique();
        });
    }
    protected static void ConfigureCompany(ModelBuilder builder)
    {
        builder.Entity<Company>(b =>
        {
            b.ToTable(AccountingConsts.DbTablePrefix + "Companies", AccountingConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Code).IsRequired().HasMaxLength(AccountingCommonConsts.MaxCodeLength);
            b.Property(x => x.Prefix).IsRequired().HasMaxLength(AccountingCommonConsts.MaxPrefixLength);
            b.Property(x => x.Name).IsRequired().HasMaxLength(CompanyConsts.MaxNameLength);
            b.Property(x => x.OtherName).HasMaxLength(CompanyConsts.MaxNameLength);
            b.Property(x => x.NickName).HasMaxLength(CompanyConsts.MaxNameLength);
            b.Property(x => x.Currency).IsRequired().HasMaxLength(CurrencyConsts.MaxCurrencyLength);
            b.Property(x => x.PaymentTerm).HasMaxLength(CompanyConsts.CommonMaxLength);
            b.Property(x => x.TradeTerm).HasMaxLength(CompanyConsts.CommonMaxLength);
            b.HasMany(x => x.Addresses).WithOne().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(x => x.Contacts).WithOne().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Cascade);
            b.Property(x => x.CreditLimit).HasColumnType("decimal").HasPrecision(AccountingCommonConsts.AmountPrecision, AccountingCommonConsts.AmountScale);
        });
    }
    protected static void ConfigureCompanyAddress(ModelBuilder builder) { 
        builder.Entity<CompanyAddress>(b =>
        {
            b.ToTable(AccountingConsts.DbTablePrefix + "CompanyAddresses", AccountingConsts.DbSchema);
            b.ConfigureByConvention();
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
    }
    protected static void ConfigureCompanyContact(ModelBuilder builder)
    {
        builder.Entity<CompanyContact>(b =>
        {
            b.ToTable(AccountingConsts.DbTablePrefix + "CompanyContacts", AccountingConsts.DbSchema);
            b.ConfigureByConvention();
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
    protected static void ConfigureAccountingPeriod(ModelBuilder builder)
    {
        //company contact
        builder.Entity<AccountingPeriod>(b =>
        {
            b.ToTable(AccountingConsts.DbTablePrefix + "AccountingPeriods", AccountingConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Code).IsRequired().HasMaxLength(AccountingCommonConsts.MaxCodeLength);
            b.Property(x => x.StartDate).IsRequired().HasColumnType("date").HasDefaultValue(new DateOnly(2025, 1, 1));
            b.Property(x => x.EndDate).IsRequired().HasColumnType("date").HasDefaultValue(new DateOnly(2025, 12, 31));
            b.Property(x => x.IsCurrentPeriod).IsRequired().HasDefaultValue(false);
        });
    }
    protected static void ConfigureAccountType(ModelBuilder builder)
    {
        builder.Entity<AccountType>(b =>
        {
            b.ToTable(AccountingConsts.DbTablePrefix + "AccountTypes", AccountingConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Code).IsRequired().HasMaxLength(AccountingCommonConsts.MaxCodeLength);
            b.Property(x => x.Id).IsRequired().HasMaxLength(AccountingCommonConsts.MaxCodeLength);
            b.Property(x => x.Name).IsRequired().HasMaxLength(AccountingCommonConsts.MaxNameLength);
            b.Property(x => x.OtherName).IsRequired().HasMaxLength(AccountingCommonConsts.MaxNameLength); 
            b.Property(x => x.Category).IsRequired().HasDefaultValue(AccountTypeTypes.Normal);
            b.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        });
    }
    protected static void ConfigureSubjectCategory(ModelBuilder builder)
    {
        builder.Entity<SubjectCategory>(b =>
        {
            b.ToTable(AccountingConsts.DbTablePrefix + "SubjectCategories", AccountingConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Code).IsRequired().HasMaxLength(AccountingCommonConsts.MaxCodeLength);
            b.Property(x => x.Name).IsRequired().HasMaxLength(AccountingCommonConsts.MaxNameLength);
            b.Property(x => x.OtherName).HasMaxLength(AccountingCommonConsts.MaxNameLength); 
            b.Property(x => x.Description).HasMaxLength(AccountingCommonConsts.MaxDescriptionLength);
            b.HasOne(x => x.AccountType).WithMany().HasForeignKey(x => x.AccountTypeId).OnDelete(DeleteBehavior.Restrict);
            b.HasMany(x => x.Subjects).WithOne(x => x.SubjectCategory).HasForeignKey(x => x.SubjectCategoryId).OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        });
    }
    protected static void ConfigureSubject(ModelBuilder builder)
    {
        builder.Entity<Subject>(b =>
        {
            b.ToTable(AccountingConsts.DbTablePrefix + "Subjects", AccountingConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Code).IsRequired().HasMaxLength(AccountingCommonConsts.MaxCodeLength);
            b.Property(x => x.Name).IsRequired().HasMaxLength(AccountingCommonConsts.MaxNameLength);
            b.Property(x => x.OtherName).HasMaxLength(AccountingCommonConsts.MaxNameLength); 
            b.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(CurrencyConsts.MaxCurrencyLength);
            b.Property(x => x.Description).HasMaxLength(AccountingCommonConsts.MaxDescriptionLength);
            b.HasOne(x => x.AccountType).WithMany().HasForeignKey(x => x.AccountTypeId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.SubjectCategory).WithMany(x => x.Subjects).HasForeignKey(x => x.SubjectCategoryId);
            b.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        });
    }
    protected static void ConfigureVoucher(ModelBuilder builder)
    {
        builder.Entity<Voucher>(b =>
        {
            b.ToTable(AccountingConsts.DbTablePrefix + "Vouchers", AccountingConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Code).IsRequired().HasMaxLength(AccountingCommonConsts.MaxCodeLength);
            b.Property(x => x.Prefix).IsRequired().HasMaxLength(AccountingCommonConsts.MaxPrefixLength);
            b.Property(x => x.VoucherDate).IsRequired().HasColumnType("date");
            b.Property(x => x.VoucherType).IsRequired().HasDefaultValue(VoucherType.JournalVoucher); 
            b.HasMany(x => x.Details).WithOne(x => x.Voucher).HasForeignKey(x => x.VoucherId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        });
    }
    protected static void ConfigureVoucherDetail(ModelBuilder builder)
    {
        builder.Entity<VoucherDetail>(b =>
        {
            b.ToTable(AccountingConsts.DbTablePrefix + "VoucherDetails", AccountingConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(AccountingCommonConsts.MaxCodeLength);
            b.Property(x => x.CurrencyRate).HasColumnType("decimal").HasPrecision(AccountingCommonConsts.AmountPrecision, AccountingCommonConsts.AmountScale);
            b.Property(x => x.ForeignAmount).HasColumnType("decimal").HasPrecision(AccountingCommonConsts.AmountPrecision, AccountingCommonConsts.AmountScale);
            b.Property(x => x.NativeAmount).HasColumnType("decimal").HasPrecision(AccountingCommonConsts.AmountPrecision, AccountingCommonConsts.AmountScale);
            b.Property(x => x.Description).HasMaxLength(AccountingCommonConsts.MaxDescriptionLength);
            b.Property(x => x.DocNo).HasMaxLength(AccountingCommonConsts.MaxCodeLength);
            b.Property(x => x.Project).HasMaxLength(AccountingCommonConsts.MaxCodeLength);
            b.Property(x => x.Department).HasMaxLength(AccountingCommonConsts.MaxCodeLength);
            b.Property(x => x.Region).HasMaxLength(AccountingCommonConsts.MaxCodeLength);
            b.Property(x => x.Custom1).HasMaxLength(AccountingCommonConsts.MaxCodeLength);
            b.Property(x => x.Custom2).HasMaxLength(AccountingCommonConsts.MaxCodeLength);
            b.Property(x => x.DueDate).HasColumnType("date");
            b.HasOne(x => x.Subject).WithMany().HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.SubSubjectCode).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
