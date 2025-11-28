using Accounting.BasicData.Companies;
using Accounting.Finance.AccountingPeriods;
using Accounting.Finance.AccountTypes;
using Accounting.Finance.Subjects;
using Accounting.Finance.Vouchers;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace Accounting.Finance
{
    public abstract class VoucherManagerTests<TStartupModule> : AccountingDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly VoucherManager _voucherManager;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IAccountingPeriodRepository _accountingPeriodRepository;
        private readonly IAccountTypeRepository _accountTypeRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IVoucherRepository _voucherRepository;
        private Guid _arSubjectId;
        private Guid _apSubjectId;
        private Guid _a11SubjectId;
        private Guid _companyId;
        public VoucherManagerTests()
        {
            _voucherManager = GetRequiredService<VoucherManager>();
            _subjectRepository = GetRequiredService<ISubjectRepository>();
            _accountingPeriodRepository = GetRequiredService<IAccountingPeriodRepository>();
            _accountTypeRepository = GetRequiredService<IAccountTypeRepository>();
            _companyRepository = GetRequiredService<ICompanyRepository>();
            _voucherRepository = GetRequiredService<IVoucherRepository>();
        }
        private Voucher CreateVoucher(DateOnly voucherDate)
        {
            var voucher = new Voucher(
                Guid.NewGuid(),
                voucherDate,
                VoucherType.JournalVoucher,
                VoucherStatus.Draft
            );
            return voucher;
        }
        private Subject GetSubject(string code, Guid? accountTypeId, bool isSubSubjectType)
        {
            var subject = new Subject(Guid.NewGuid(), code, code, code, null, accountTypeId, DebitorCreditor.Debitor, "USD", code, isSubSubjectType, true, true, 0);
            return subject;
        }
        private async Task InitData()
        {
            await _accountingPeriodRepository.InsertManyAsync([
                new AccountingPeriod(Guid.NewGuid(), "2023", new DateOnly(2023,1,1),new DateOnly(2023,12,31),true),
                new AccountingPeriod(Guid.NewGuid(), "2022", new DateOnly(2022,1,1),new DateOnly(2022,12,31),true),
            ]);

            var arAccountType = (await _accountTypeRepository.GetPagedListAsync(AccountTypeConsts.AccountingReceivableType)).FirstOrDefault();
            var apAccounType = (await _accountTypeRepository.GetPagedListAsync(AccountTypeConsts.AccountingPayableType)).FirstOrDefault();
            var arSubject = GetSubject("21", arAccountType.Id, true);
            var apSubject = GetSubject("41", apAccounType.Id, true);
            _arSubjectId = arSubject.Id;
            _apSubjectId = apSubject.Id;
            var aSubject = GetSubject("a11", null, false);
            _a11SubjectId = aSubject.Id;
            await _subjectRepository.InsertManyAsync([
                aSubject,
                GetSubject("b22",null,false),
                GetSubject("c33",null,false),
                arSubject,
                apSubject
            ]);
        }
        private async Task InitVoucherData()
        {
            var company = (await _companyRepository.GetPagedListAsync()).FirstOrDefault();
            _companyId = company.Id;
            var voucherDate = new DateOnly(2025, 1, 12);
            var arVoucher = CreateVoucher(voucherDate);
            arVoucher.SetCode("test0001", "test", 1);
            arVoucher.AddDetail(Guid.NewGuid(), _a11SubjectId, null, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                100.0m, "SI0001", null, 0, true, string.Empty);
            arVoucher.AddDetail(Guid.NewGuid(), _arSubjectId, company.Id, "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                100.0m, "SI0001", new DateOnly(2027, 1, 1), 0, true, string.Empty);

            var apVoucher = CreateVoucher(voucherDate);
            apVoucher.SetCode("test0002", "test", 2);
            apVoucher.AddDetail(Guid.NewGuid(), _a11SubjectId, null, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                100.0m, "PI0001", null, 0, true, string.Empty);
            apVoucher.AddDetail(Guid.NewGuid(), _apSubjectId, company.Id, "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                100.0m, "PI0001", new DateOnly(2027, 1, 1), 0, true, string.Empty);

            await _voucherRepository.InsertManyAsync([arVoucher, apVoucher]);
        }
        [Fact]
        public async Task Can_Validate_Voucher()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange 
                var voucherDate = new DateOnly(2025, 10, 13);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC001", null, 0, true, string.Empty);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC002", null, 0, true, string.Empty);

                // Act
                await _voucherManager.ValidateAsync(voucher);
                // Assert 
                (true).ShouldBe(true);
            });
        }
        [Fact]
        public async Task Cannot_Validate_Voucher_With_Not_Balance()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange 
                var voucherDate = new DateOnly(2025, 1, 12);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC001", null, 0, true, string.Empty);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 10.0m,
                    10.0m, "DOC002", null, 0, true, string.Empty);

                // Act
                var exception = await Should.ThrowAsync<BusinessException>(async () =>
                {
                    await _voucherManager.ValidateAsync(voucher);
                });
                // Assert 
                exception.ShouldNotBeNull();
                exception.Code.ShouldBe(AccountingDomainErrorCodes.VoucherDoesNotBalance);
            });
        }
        [Theory]
        [InlineData(2020)]
        [InlineData(2026)]
        public async Task Cannot_Validate_Voucher_With_Out_Period_Voucher_Date(int year)
        {
            await WithUnitOfWorkAsync(async () => await InitData());
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange 
                var voucherDate = new DateOnly(year, 1, 12);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC001", null, 0, true, string.Empty);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC002", null, 0, true, string.Empty);

                // Act
                var exception = await Should.ThrowAsync<BusinessException>(async () =>
                {
                    await _voucherManager.ValidateAsync(voucher);
                });
                // Assert 
                exception.ShouldNotBeNull();
                exception.Code.ShouldBe(AccountingDomainErrorCodes.VoucherDateIsNotInCurrentPeriodRange);
            });
        }
        [Fact]
        public async Task Cannot_Validate_Voucher_With_Empty_SubSubjectCode()
        {
            await WithUnitOfWorkAsync(async () => await InitData());
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange   
                var voucherDate = new DateOnly(2025, 1, 12);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC001", null, 0, true, string.Empty);
                voucher.AddDetail(Guid.NewGuid(), _arSubjectId, Guid.Empty, "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC002", null, 0, true, string.Empty);

                // Act
                var exception = await Should.ThrowAsync<BusinessException>(async () =>
                {
                    await _voucherManager.ValidateAsync(voucher);
                });
                // Assert 
                exception.ShouldNotBeNull();
                exception.Code.ShouldBe(AccountingDomainErrorCodes.SubSubjectCodeCanNotBeEmpty);
            });
        }
        [Fact]
        public async Task Cannot_Validate_Voucher_With_Empty_DocNo()
        {
            await WithUnitOfWorkAsync(async () => await InitData());
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange   
                var voucherDate = new DateOnly(2025, 1, 12);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC001", null, 0, true, string.Empty);
                voucher.AddDetail(Guid.NewGuid(), _arSubjectId, Guid.NewGuid(), "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                    100.0m, string.Empty, null, 0, true, string.Empty);

                // Act
                var exception = await Should.ThrowAsync<BusinessException>(async () =>
                {
                    await _voucherManager.ValidateAsync(voucher);
                });
                // Assert 
                exception.ShouldNotBeNull();
                exception.Code.ShouldBe(AccountingDomainErrorCodes.DocNoCanNotBeEmpty);
            });
        }
        [Fact]
        public async Task Cannot_Validate_Voucher_With_Empty_DueDate()
        {
            await WithUnitOfWorkAsync(async () => await InitData());
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange   
                var voucherDate = new DateOnly(2025, 1, 12);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC001", null, 0, true, string.Empty);
                voucher.AddDetail(Guid.NewGuid(), _arSubjectId, Guid.NewGuid(), "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC002", null, 0, true, string.Empty);

                // Act
                var exception = await Should.ThrowAsync<BusinessException>(async () =>
                {
                    await _voucherManager.ValidateAsync(voucher);
                });
                // Assert 
                exception.ShouldNotBeNull();
                exception.Code.ShouldBe(AccountingDomainErrorCodes.DueDateCanNotBeEmpty);
            });
        }
        [Fact]
        public async Task Can_Validate_Voucher_With_Receivable_Payable_Subject()
        {
            await WithUnitOfWorkAsync(async () => await InitData());
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange  
                var voucherDate = new DateOnly(2025, 1, 12);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC001", null, 0, true, string.Empty);
                voucher.AddDetail(Guid.NewGuid(), _apSubjectId, Guid.NewGuid(), "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC002", new DateOnly(2027, 1, 1), 0, true, string.Empty);

                // Act & Assert
                await Should.NotThrowAsync(async () =>
                {
                    await _voucherManager.ValidateAsync(voucher);
                });
            });
        }
        [Fact]
        public async Task Cannot_Validate_Voucher_With_Duplicated_Doc_No()
        {
            await WithUnitOfWorkAsync(async () => await InitData());
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange   
                var voucherDate = new DateOnly(2025, 1, 12);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 200.0m,
                    200.0m, "DOC001", null, 0, true, string.Empty);
                voucher.AddDetail(Guid.NewGuid(), _arSubjectId, Guid.NewGuid(), "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC002", new DateOnly(2027, 1, 1), 0, true, string.Empty);
                voucher.AddDetail(Guid.NewGuid(), _arSubjectId, Guid.NewGuid(), "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                   100.0m, "DOC002", new DateOnly(2027, 1, 1), 0, true, string.Empty);

                // Act
                var exception = await Should.ThrowAsync<BusinessException>(async () =>
                {
                    await _voucherManager.ValidateAsync(voucher);
                });
                // Assert 
                exception.ShouldNotBeNull();
                exception.Code.ShouldBe(AccountingDomainErrorCodes.DocNoIsDuplicated);
            });
        }
        [Fact]
        public async Task Cannot_Validate_Voucher_With_Duplicated_Doc_No_AP()
        {
            await WithUnitOfWorkAsync(async () => await InitData());
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange   
                var voucherDate = new DateOnly(2025, 1, 12);
                var voucher = CreateVoucher(voucherDate);
                var subSubjectCode = Guid.NewGuid();
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Creditor, "USD", 1.0m, 200.0m,
                    200.0m, "DOC001", null, 0, true, string.Empty);
                voucher.AddDetail(Guid.NewGuid(), _apSubjectId, subSubjectCode, "Test2 Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC002", new DateOnly(2027, 1, 1), 0, true, string.Empty);
                voucher.AddDetail(Guid.NewGuid(), _apSubjectId, subSubjectCode, "Test2 Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                   100.0m, "DOC002", new DateOnly(2027, 1, 1), 0, true, string.Empty);

                // Act
                var exception = await Should.ThrowAsync<BusinessException>(async () =>
                {
                    await _voucherManager.ValidateAsync(voucher);
                });
                // Assert 
                exception.ShouldNotBeNull();
                exception.Code.ShouldBe(AccountingDomainErrorCodes.DocNoIsDuplicated);
            });
        }
        [Fact]
        public async Task Cannot_Validate_Voucher_With_Repeat_Doc_No()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                await InitData();
                await InitVoucherData();
            });
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange   
                var voucherDate = new DateOnly(2025, 1, 12);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC001", null, 0, true, string.Empty);
                voucher.AddDetail(Guid.NewGuid(), _arSubjectId, Guid.NewGuid(), "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                    100.0m, "SI0001", new DateOnly(2027, 1, 1), 0, true, string.Empty);

                // Act
                var exception = await Should.ThrowAsync<BusinessException>(async () =>
                {
                    await _voucherManager.ValidateAsync(voucher);
                });
                // Assert 
                exception.ShouldNotBeNull();
                exception.Code.ShouldBe(AccountingDomainErrorCodes.DocNoHasBeenUsed);
            });
        }
        [Fact]
        public async Task Cannot_Validate_Voucher_With_Repeat_Doc_No_AP()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                await InitData();
                await InitVoucherData();
            });

            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange   
                var voucherDate = new DateOnly(2025, 1, 12);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC001", null, 0, true, string.Empty);
                voucher.AddDetail(Guid.NewGuid(), _apSubjectId, _companyId, "Test2 Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "PI0001", new DateOnly(2027, 1, 1), 0, true, string.Empty);

                // Act
                var exception = await Should.ThrowAsync<BusinessException>(async () =>
                {
                    await _voucherManager.ValidateAsync(voucher);
                });
                // Assert 
                exception.ShouldNotBeNull();
                exception.Code.ShouldBe(AccountingDomainErrorCodes.DocNoHasBeenUsed);
            });
        }

        [Fact]
        public void Can_Set_Enable_Function_Fields()
        {
            // Arrange   
            var voucherDate = new DateOnly(2025, 1, 12);
            var voucher = CreateVoucher(voucherDate);
            var detail = voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                100.0m, "DOC001", null, 0, true, string.Empty);

            // Act
            VoucherManager.SetFunctionalFields(detail, true, true, true, true, true, "project", "region", "department", "custom1", "custom2");

            // Assert 
            detail.Region.ShouldBe("region");
            detail.Project.ShouldBe("project");
            detail.Department.ShouldBe("department");
            detail.Custom1.ShouldBe("custom1");
            detail.Custom2.ShouldBe("custom2");
        }
        [Fact]
        public void Cannot_Set_Enable_Function_Fields()
        {
            // Arrange   
            var voucherDate = new DateOnly(2025, 1, 12);
            var voucher = CreateVoucher(voucherDate);
            var detail = voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), null, "Test Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                100.0m, "DOC001", null, 0, true, string.Empty);

            // Act
            VoucherManager.SetFunctionalFields(detail, false, false, false, false, false, "project", "region", "department", "custom1", "custom2");

            // Assert 
            detail.Region.ShouldBe(string.Empty);
            detail.Project.ShouldBe(string.Empty);
            detail.Department.ShouldBe(string.Empty);
            detail.Custom1.ShouldBe(string.Empty);
            detail.Custom2.ShouldBe(string.Empty);
        }
    }
}
