using Accounting.Finance.Subjects;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Utility
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
                throw new BusinessException(AccountingDomainErrorCodes.CodeIsDuplicated).WithData("codes", string.Join(localizer["Comma"], repeatCodes));
            }
            var existsItems = await getExistItems(codes);
            if (existsItems.Any())
            {
                throw new BusinessException(AccountingDomainErrorCodes.CodeIsInUse).WithData("codes", string.Join(localizer["Comma"],
                    existsItems.Where(item => codes.Contains(item)).Select(item => item)));
            }

            return codes;
        }
    }
}
