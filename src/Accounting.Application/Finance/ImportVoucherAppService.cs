using Accounting.Features;
using Accounting.Finance.Vouchers;
using Accounting.Permissions;
using Accounting.Common;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Accounting.Finance.Subjects;
using Accounting.BasicData.Companies;
using Accounting.BasicData.Currencies;

namespace Accounting.Finance
{
    public class ImportVoucherAppService : AccountingAppService, IImportVoucherAppService, IDisposable
    {
        protected IVoucherRepository Repository { get; set; }
        protected ISubjectRepository SubjectRepository { get; set; }
        protected ICompanyRepository CompanyRepository { get; set; }
        protected VoucherManager VoucherManager { get; set; }

        protected bool EnableProjectFunction { get; set; }
        protected bool EnableRegionFunction { get; set; }
        protected bool EnableDepartmentFunction { get; set; }
        protected bool EnableCustom1Function { get; set; }
        protected bool EnableCustom2Function { get; set; }

        protected Dictionary<string, Subject> _subjects = null;

        protected Dictionary<string, Company> _companies = null;

        protected const string GroupText = "Group";

        public ImportVoucherAppService(IVoucherRepository repository, ISubjectRepository subjectRepository,
            ICompanyRepository companyRepository, VoucherManager voucherManager)
        {
            Repository = repository;
            SubjectRepository = subjectRepository;
            CompanyRepository = companyRepository;
            VoucherManager = voucherManager;
        }
        private async Task InitEnableFunctionAsync()
        {
            EnableProjectFunction = await FeatureChecker.IsEnabledAsync(AccountingFeatures.ProjectFunction);
            EnableRegionFunction = await FeatureChecker.IsEnabledAsync(AccountingFeatures.RegionFunction);
            EnableDepartmentFunction = await FeatureChecker.IsEnabledAsync(AccountingFeatures.DepartmentFunction);
            EnableCustom1Function = await FeatureChecker.IsEnabledAsync(AccountingFeatures.Custom1Function);
            EnableCustom2Function = await FeatureChecker.IsEnabledAsync(AccountingFeatures.Custom2Function);
        }

        private async Task<(IEnumerable<IGrouping<int, VoucherImportItemDto>>, Currency?)>
             CheckImportDataAsync(VoucherImportDto input)
        {
            var count = input.Data.Count();
            var subjectInputDic = new Dictionary<string, bool>(count);
            var subjectCodes = new List<string>(count);
            var companyInputDic = new Dictionary<string, bool>(count);
            var companyCodes = new List<string>(count);
            var groupedData = input.Data.GroupBy(x => x.GroupNo, (item) =>
            {
                if (string.IsNullOrWhiteSpace(item.SubjectCode))
                {
                    throw new BusinessException(VoucherErrorCodes.SubjectCodeCanNotBeEmpty).WithData(GroupText, item.GroupNo);
                }
                if (item.Debit > 0 && item.Credit > 0)
                {
                    throw new BusinessException(VoucherErrorCodes.DebitAndCreditCanNotBothBeGreaterThanZero).WithData(GroupText, item.GroupNo);
                }
                if (subjectInputDic.TryAdd(item.SubjectCode, true))
                {
                    subjectCodes.Add(item.SubjectCode);
                }
                if (!string.IsNullOrWhiteSpace(item.SubSubjectCode) && companyInputDic.TryAdd(item.SubSubjectCode, true))
                {
                    companyCodes.Add(item.SubSubjectCode);
                }

                return item;
            });
            var codeList = groupedData.Select(x => x.First().VoucherCode).Where(item => !string.IsNullOrWhiteSpace(item)).ToList();
            await ImportHelper.CheckExistsCodesAsync(codeList, L, async codes => (await Repository.GetPagedListAsync(new VoucherFilterRequest { Codes = codes })).Select(item => item.Code));

            if (input.ImportType == VoucherImportType.SingleEntry)
            {
                if (string.IsNullOrWhiteSpace(input.SubjectCode))
                {
                    throw new BusinessException(VoucherErrorCodes.InSingleEntrySubjectCodeCannotBeEmpty);
                }
                if (!subjectInputDic.ContainsKey(input.SubjectCode))
                {
                    subjectCodes.Add(input.SubjectCode);
                }
            }
            var subjects = await SubjectRepository.GetPagedListAsync(new SubjectFilterRequest { Codes = subjectCodes });
            var companies = await CompanyRepository.GetPagedListAsync(codes: companyCodes);
            CheckExistsCodes([.. subjects], subjectCodes, [.. companies], companyCodes);

            _subjects = subjects.ToDictionary(item => item.Code, item => item);
            _companies = companies.ToDictionary(item => item.Code, item => item);
            var currency = await GetSubjectCurrencyAsync(input, _subjects);

            return (groupedData, currency);
        }
        private async Task<Currency?> GetSubjectCurrencyAsync(VoucherImportDto input, Dictionary<string, Subject> subjects)
        {
            if (input.ImportType != VoucherImportType.SingleEntry)
            {
                return null;
            }
            var singleSubject = subjects[input.SubjectCode];
            Currency? singleCurrency = null;
            if (singleSubject == null)
            {
                throw new BusinessException(VoucherErrorCodes.SubjectCodeNotExists).WithData(nameof(input.SubjectCode), input.SubjectCode);
            }
            if (!string.IsNullOrWhiteSpace(singleSubject.CurrencyCode))
            {
                var currencyRepository = LazyServiceProvider.LazyGetRequiredService<ICurrencyRepository>();
                singleCurrency = (await currencyRepository.GetPagedListAsync(singleSubject.CurrencyCode)).FirstOrDefault();
            }
            if (singleCurrency == null)
            {
                throw new BusinessException(VoucherErrorCodes.CurrencyNotSetUpInSubject).WithData("code", singleSubject.Code);
            }
            return singleCurrency;
        }
        private void CheckExistsCodes(List<Subject> subjects, List<string> subjectCodes, List<Company> companies, List<string> companyCodes)
        {
            if (subjects.Count < subjectCodes.Count)
            {
                var existsCodes = subjects.Select(x => x.Code);
                var notExistsCodes = subjectCodes.Except(existsCodes);
                ImportHelper.ThrowBusinessExceptionWithCodes(VoucherErrorCodes.SubjectCodeNotExists, notExistsCodes, L);
            }
            if (companies.Count < companyCodes.Count)
            {
                var existsCodes = companies.Select(x => x.Code);
                var notExistsCodes = companyCodes.Except(existsCodes);
                ImportHelper.ThrowBusinessExceptionWithCodes(VoucherErrorCodes.SubSubjectCodeNotExists, notExistsCodes, L);
            }
        }
        private int GetGenNo(string voucherCode, string prefix)
        {
            var genNo = 0;
            if (!string.IsNullOrWhiteSpace(voucherCode) && voucherCode.StartsWith(prefix))
            {
                var s = voucherCode.Replace(prefix, string.Empty);
                if (!string.IsNullOrEmpty(s) && s.Length > 1)
                {
                    int.TryParse(s.Substring(1), out genNo);
                }
            }
            return genNo;
        }

        private void GenerateSingleEntryItem(Voucher voucher, VoucherImportItemDto item, Subject? singleSubject, Currency? singleCurrency)
        {
            var totalNativeAmount = voucher.Details.Sum(obj => obj.NativeAmount * (int)obj.DebitorCreditor);

            if (singleSubject != null)
            {
                item.SubSubjectCode = singleSubject.Code;
            }
            var currencyRate = item.CurrencyRate;
            var currency = item.Currency;
            var foriegnAmount = Math.Abs(totalNativeAmount) / currencyRate;
            if (singleCurrency != null)
            {
                currencyRate = singleCurrency.ExchangeRate;
                currency = singleCurrency.TargetCurrency;
                foriegnAmount = Math.Abs(totalNativeAmount) / item.CurrencyRate;
            }

            if (totalNativeAmount > 0)
            {
                item.Debit = 0;
                item.Credit = foriegnAmount;
            }
            else
            {
                item.Credit = 0;
                item.Debit = foriegnAmount;
            }

            GenerateVoucherDetail(voucher, item);
        }
        private void GenerateVoucherDetail(Voucher voucher, VoucherImportItemDto item)
        {
            var subjectId = _subjects[item.SubjectCode].Id;
            Guid? subSubjectId = !string.IsNullOrWhiteSpace(item.SubjectCode) && _companies.TryGetValue(item.SubjectCode, out Company? value) ? value.Id : null;
            var debitorCreditor = DebitorCreditor.Debitor;
            var foriegnAmount = item.Debit;
            if (item.Credit > 0)
            {
                debitorCreditor = DebitorCreditor.Creditor;
                foriegnAmount = item.Credit;
            }
            var nativeAmount = foriegnAmount * item.CurrencyRate;
            DateOnly? dueDate = item.DueDate != null ? DateOnly.FromDateTime(item.DueDate.Value) : null;

            var detailId = GuidGenerator.Create();
            var detail = voucher.AddDetail(detailId, subjectId, subSubjectId, item.Description, debitorCreditor, item.Currency,
                item.CurrencyRate, foriegnAmount, nativeAmount, item.DocNo, dueDate, item.ItemQty, true, item.PaymentReference,
                item.Project, item.Region, item.Department, item.Custom1, item.Custom2);  
        }
        [Authorize(AccountingPermissions.TransferVouchers.Import)]
        public async Task<int> ImportDataAsync(VoucherImportDto input)
        {
            Check.NotNull(input, nameof(input));
            if (input.Data == null || !input.Data.Any())
            {
                return 0;
            }
            var (groupedData, singleCurrency) = await CheckImportDataAsync(input);
            Subject? singleSubject = !string.IsNullOrWhiteSpace(input.SubjectCode) ? _subjects[input.SubjectCode] : null;

            await InitEnableFunctionAsync();

            var entityTasks = groupedData.Select(async item =>
            {
                var firstItem = item.First();
                var voucher = new Voucher(GuidGenerator.Create(), DateOnly.FromDateTime(firstItem.VoucherDate),
                    firstItem.VoucherType ?? VoucherType.JournalVoucher, CurrentTenant.Id);

                var genNo = GetGenNo(firstItem.VoucherCode, firstItem.Prefix);
                voucher.SetCode(firstItem.VoucherCode, firstItem.Prefix, genNo);

                voucher.SetFunctionEnable(EnableProjectFunction, EnableRegionFunction, EnableDepartmentFunction, EnableCustom1Function,
                    EnableCustom2Function);

                foreach (var subItem in item)
                {
                    GenerateVoucherDetail(voucher, subItem);
                }
                if (input.ImportType == VoucherImportType.SingleEntry)
                {
                    GenerateSingleEntryItem(voucher, firstItem, singleSubject, singleCurrency);
                }
                await VoucherManager.ValidateAsync(voucher);

                return voucher;
            });

            await Repository.InsertManyAsync(await Task.WhenAll(entityTasks));

            return entityTasks.Count();
        }

        public void Dispose()
        {
            _companies = null;
            _subjects = null;
            VoucherManager = null;
        }
    }
}
