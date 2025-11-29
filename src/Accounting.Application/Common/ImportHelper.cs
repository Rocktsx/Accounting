using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;

namespace Accounting.Common
{
    internal static class ImportHelper
    {
        public static async Task<IEnumerable<string>> CheckImportDataAsync<T>(this IEnumerable<T> inputs, IStringLocalizer localizer,
            Func<T, string> getCode, Func<IEnumerable<string>, Task<IEnumerable<string>>> getExistItems)
        {
            Check.NotNull(inputs, nameof(inputs));

            var codes = inputs.Select(getCode).Distinct();

            if (codes.Count() < inputs.Count())
            {
                var repeatCodes = inputs.Select(getCode).GroupBy(item => item).Where(item => item.Count() > 1).Select(item => item.Key);
                ThrowBusinessExceptionWithCodes(AccountingDomainErrorCodes.CodeIsDuplicated, repeatCodes, localizer);
            }
            await CheckExistsCodesAsync(codes, localizer, getExistItems);

            return codes;
        }
        public static async Task CheckExistsCodesAsync(IEnumerable<string> codes, IStringLocalizer localizer, Func<IEnumerable<string>, Task<IEnumerable<string>>> getExistItems)
        {
            var existsItems = await getExistItems(codes);
            if (existsItems.Any())
            {
                ThrowBusinessExceptionWithCodes(AccountingDomainErrorCodes.CodeIsInUse, existsItems, localizer);
            }
        }
        public static void ThrowBusinessExceptionWithCodes(string errorCode, IEnumerable<string> codes, IStringLocalizer localizer)
        {
            throw new BusinessException(errorCode).WithData("codes", string.Join(localizer["Comma"], codes));
        }
    }
}
