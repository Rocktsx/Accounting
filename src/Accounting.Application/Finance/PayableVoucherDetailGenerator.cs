using Accounting.Common;
using Accounting.Finance.PayableVouchers; 
using Accounting.Finance.Settings;
using Accounting.Finance.Vouchers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Accounting.Finance
{
    public class PayableVoucherDetailGenerator : IDisposable
    {
        private static string PaymentDescriptFormat = "received native amount {0:#,##0.00}";
        private static string DepositText = "Deposit received from";
        private GeneratePayableDetailRequestDto Input { get; set; }
        IAbpLazyServiceProvider LazyServiceProvider { get; set; }

        public PayableVoucherDetailGenerator(
            IAbpLazyServiceProvider lazyServiceProvider,
             GeneratePayableDetailRequestDto input)
        {
            LazyServiceProvider = lazyServiceProvider;
            Input = input;
        }
        /// <summary>
        /// 生成传票明细
        /// </summary>
        /// <param name="input">input</param>
        /// <returns></returns>
        public async Task<IEnumerable<VoucherDetailDto>> GenerateAsync(
           )
        {
            Check.NotNull(Input, nameof(Input));
            Check.NotDefaultOrNull(Input.DebitorId, nameof(Input.DebitorId));

            var input = Input;
            var payments = input.Payments.Where(item => item.NativeAmount > 0);
            if (!payments.Any())
            {
                return [];
            }
            var balance = ValidatePaymentBalance();
            var result = new List<VoucherDetailDto>();

            HandlePayments(result, payments);
            var receipts = HandleReceipts(result);

            //没有选收了哪张单的钱 || 收到的钱大于应收的钱
            if (receipts == 0 || balance > 0 && receipts > 0)
            { 
                Guid subjectId = await GetApSubjectIdAsync();
                //没有选收了哪张单的钱
                if (receipts == 0)
                {
                    HandleNoReceipts(result, payments, subjectId);
                }
                else
                {
                    //收到的钱大于应收的钱
                    HandleDeposit(result, subjectId, payments, balance);
                }
            }
            return result;
        }
        private async Task<Guid> GetApSubjectIdAsync()
        {
            var appSettingService = LazyServiceProvider.GetRequiredService<IAccountingSettingAppService>();
            var arSubjectIdText = await appSettingService.GetAccountPayableSubjectCodeAsync();
            Guid subjectId;
            if (string.IsNullOrEmpty(arSubjectIdText) ||
                !Guid.TryParse(arSubjectIdText, out subjectId))
            {
                throw new BusinessException(VoucherErrorCodes.PleaseEnterApSubjectInSetting);
            }
            return subjectId;
        }
        private void HandlePayments(List<VoucherDetailDto> result,
          IEnumerable<PaymentDetailDto> payments)
        {
            foreach (var item in payments)
            {
                result.Add(new VoucherDetailDto
                {
                    SubjectId = item.SubjectId,
                    DebitorCreditor = item.DebitorCreditor.Reverse(),
                    CurrencyCode = item.CurrencyCode,
                    CurrencyRate = item.CurrencyRate,
                    ForeignAmount = item.ForeignAmount,
                    NativeAmount = item.NativeAmount,
                    PaymentReference = item.PaymentReference,
                    Description = string.Format(PaymentDescriptFormat, item.NativeAmount),
                });
            }
        }
        private int HandleReceipts(List<VoucherDetailDto> result)
        {
            var sb = new StringBuilder();
            var count = 0;
            var list = Input.Receipts.Where(item => item.NativeCurrentPaid > 0)
                .Select(item =>
                {
                    var obj = new VoucherDetailDto
                    {
                        SubjectId = item.SubjectId,
                        DocNo = item.DocNo,
                        DebitorCreditor = item.DebitorCreditor.Reverse(),
                        CurrencyCode = item.CurrencyCode,
                        CurrencyRate = item.CurrencyRate,
                        ForeignAmount = item.CurrentPaid,
                        NativeAmount = item.NativeCurrentPaid,
                        SubSubjectCode = Input.DebitorId,
                        IsOriginal = false,
                        Description = $"{item.DebitorCreditor.GetShortName()} {item.DocNo}"
                    };
                    if(count != 0) {
                        sb.Append(' ');
                    }
                    count++;
                    sb.Append(obj.Description);
                    return obj;
                });
            if (list.Any())
            {
                var description = sb.ToString();
                foreach (var item in result)
                {
                    item.Description = description;
                }
                result.InsertRange(result.Count, list);
            }
            return list.Count();
        }
        private void HandleNoReceipts(List<VoucherDetailDto> result,
           IEnumerable<PaymentDetailDto> payments, Guid subjectId)
        {
            foreach (var item in payments)
            {
                CheckPaymentReference(item.PaymentReference);

                result.Add(new VoucherDetailDto
                {
                    SubjectId = subjectId,
                    DocNo = item.PaymentReference,
                    DebitorCreditor = item.DebitorCreditor,
                    CurrencyCode = item.CurrencyCode,
                    CurrencyRate = item.CurrencyRate,
                    ForeignAmount = item.ForeignAmount,
                    NativeAmount = item.NativeAmount,
                    SubSubjectCode = Input.DebitorId,
                    PaymentReference = item.PaymentReference,
                    Description = $"{DepositText} {item.PaymentReference}",
                    IsOriginal = true
                });
            }
        }
        private void CheckPaymentReference(string? reference)
        {
            if (string.IsNullOrEmpty(reference))
            {
                throw new BusinessException(VoucherErrorCodes.PaymentReferenceCannotBeEmpty);
            }
        }
        private void HandleDeposit(List<VoucherDetailDto> result, Guid subjectId,
           IEnumerable<PaymentDetailDto> payments, decimal balance)
        {
            var paymentReference = payments.FirstOrDefault(item =>
                !string.IsNullOrEmpty(item.PaymentReference))?.PaymentReference;

            CheckPaymentReference(paymentReference);

            var item = payments.First();
            var roundScale = AccountingCommonConsts.AmountRoundScale;
            result.Add(new VoucherDetailDto
            {
                SubjectId = subjectId,
                DocNo = paymentReference,
                DebitorCreditor = item.DebitorCreditor,
                CurrencyCode = item.CurrencyCode,
                CurrencyRate = item.CurrencyRate,
                ForeignAmount = Math.Round(balance / item.CurrencyRate, roundScale),
                NativeAmount = balance,
                SubSubjectCode = Input.DebitorId,
                PaymentReference = paymentReference,
                Description = $"{DepositText} {paymentReference}",
                IsOriginal = true
            });
        }
        private decimal ValidatePaymentBalance()
        {
            var paymentAmount = Input.Payments.Sum(item => item.NativeAmount * (int)item.DebitorCreditor);
            var receiptAmount = Input.Receipts.Sum(item => item.NativeCurrentPaid);

            if (paymentAmount == 0 && receiptAmount < 0 || (paymentAmount < 0
                && receiptAmount < 0 && paymentAmount != receiptAmount))
            {
                throw new BusinessException(AccountingDomainErrorCodes.VoucherDoesNotBalance);
            }
            if (receiptAmount > paymentAmount)
            {
                throw new BusinessException(VoucherErrorCodes.ReceiptAmtGreaterThanSettlementAmt);
            }
            return paymentAmount - receiptAmount;
        }

        public void Dispose()
        {
            LazyServiceProvider = null;
            Input = null;
        }
    }
}
