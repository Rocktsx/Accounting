using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly IRepository<Subject, Guid> _subjectRepository;
        private readonly IRepository<AccountingPeriod, Guid> _accountingPeriodRepository;
        private readonly IRepository<AccountType, Guid> _accountTypeRepository;
        public VoucherManagerTests()
        {
            _voucherManager = GetRequiredService<VoucherManager>();
            _subjectRepository = GetRequiredService<IRepository<Subject, Guid>>();
            _accountingPeriodRepository = GetRequiredService<IRepository<AccountingPeriod, Guid>>();
            _accountTypeRepository = GetRequiredService<IRepository<AccountType, Guid>>(); 
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
                new AccountingPeriod(Guid.NewGuid(), "2023", new DateOnly(2024,1,1),new DateOnly(2024,12,31),true),
                new AccountingPeriod(Guid.NewGuid(), "2022", new DateOnly(2023,1,1),new DateOnly(2024,12,31),true),
            ]); 

            var arAccountType = await _accountTypeRepository.GetAsync(item => item.Code == AccountTypeConsts.AccountingReceivableType);
            var apAccounType = await _accountTypeRepository.GetAsync(item => item.Code == AccountTypeConsts.AccountingPayableType);

            await _subjectRepository.InsertManyAsync([
                GetSubject("a11",null,false),
                GetSubject("b22",null,false),
                GetSubject("c33",null,false),
                GetSubject("21",arAccountType.Id,true),
                GetSubject("41",apAccounType.Id,true)
            ]);
        }
        [Fact]
        public async Task Can_Validate_Voucher()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange 
                var voucherDate = DateOnly.FromDateTime(DateTime.Now);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), string.Empty, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC001", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), string.Empty, "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC002", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);

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
                var voucherDate = DateOnly.FromDateTime(DateTime.Now);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), string.Empty, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC001", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), string.Empty, "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 10.0m,
                    10.0m, "DOC002", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);

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
        [InlineData(2024)]
        public async Task Cannot_Validate_Voucher_With_Out_Period_Voucher_Date(int year)
        {
            await WithUnitOfWorkAsync(async () => await InitData());
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange 
                var voucherDate = new DateOnly(year, 1, 12);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), string.Empty, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC001", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), string.Empty, "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC002", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);

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
                //await InitData();
                var arSubject = await _subjectRepository.GetAsync(item => item.Code == "21");
                
                var voucherDate = DateOnly.FromDateTime(DateTime.Now);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), string.Empty, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC001", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);
                voucher.AddDetail(Guid.NewGuid(), arSubject.Id, string.Empty, "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC002", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);

                // Act
                var exception = await Should.ThrowAsync<BusinessException>(async () =>
                {
                    await _voucherManager.ValidateReceivablePayableSubject(voucher,_subjectRepository);
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
                var arSubject = await _subjectRepository.GetAsync(item => item.Code == "21");

                var voucherDate = DateOnly.FromDateTime(DateTime.Now);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), string.Empty, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC001", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);
                voucher.AddDetail(Guid.NewGuid(), arSubject.Id, "demo", "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                    100.0m, string.Empty, null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);

                // Act
                var exception = await Should.ThrowAsync<BusinessException>(async () =>
                {
                    await _voucherManager.ValidateReceivablePayableSubject(voucher, _subjectRepository);
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
                await InitData();
                var arSubject = await _subjectRepository.GetAsync(item => item.Code == "21");

                var voucherDate = DateOnly.FromDateTime(DateTime.Now);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), string.Empty, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC001", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);
                voucher.AddDetail(Guid.NewGuid(), arSubject.Id, "demo", "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC002", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);

                // Act
                var exception = await Should.ThrowAsync<BusinessException>(async () =>
                {
                    await _voucherManager.ValidateReceivablePayableSubject(voucher, _subjectRepository);
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
                await InitData();
                var apSubject = await _subjectRepository.GetAsync(item => item.Code == "41");

                var voucherDate = DateOnly.FromDateTime(DateTime.Now);
                var voucher = CreateVoucher(voucherDate);
                voucher.AddDetail(Guid.NewGuid(), Guid.NewGuid(), string.Empty, "Test Description", DebitorCreditor.Debitor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC001", null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);
                voucher.AddDetail(Guid.NewGuid(), apSubject.Id, "demo", "Test2 Description", DebitorCreditor.Creditor, "USD", 1.0m, 100.0m,
                    100.0m, "DOC002", new DateOnly(2027,1,1), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, true);

                // Act
                await _voucherManager.ValidateReceivablePayableSubject(voucher, _subjectRepository);

                // Assert 
                (true).ShouldBe(true);
            });
        }
    }
}
