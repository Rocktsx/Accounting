using CsvHelper.TypeConversion;
using Magicodes.ExporterAndImporter.Core.Models;
using Microsoft.Extensions.Localization;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace Accounting.Utility
{
    public static class ImportResultExtension
    {
        public static void HandleErrors<T>(this ImportResult<T> importResult, IStringLocalizer localizer) where T : class
        {
            var comma = localizer["Comma"].Value;
            if (importResult.Exception != null)
            {
                var msg = string.Empty;
                if (importResult.Exception.GetType() == typeof(TypeConverterException))
                {
                    var ex = (TypeConverterException)importResult.Exception;
                    msg = localizer.GetString("DataConvertFail", ex.Text);
                }
                throw new UserFriendlyException(msg);
            }
            if (importResult.HasError)
            {
                string msg = string.Empty;
                if (importResult.TemplateErrors != null && importResult.TemplateErrors.Count > 0)
                {
                    msg = localizer.GetString("TemplateErrorsMsg", string.Join(comma, importResult.TemplateErrors.Select(item => item.RequireColumnName)));
                }
                if (importResult.RowErrors != null && importResult.RowErrors.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(localizer["DataConvertFail2"]);
                    importResult.RowErrors.ForEach(item =>
                    {
                        sb.AppendFormat(localizer["RowColumnMsg"].Value, item.RowIndex.ToString(), string.Join(comma, item.FieldErrors.Keys));
                    });
                    msg = sb.ToString();
                }
                throw new UserFriendlyException(msg);
            }
        }
    }
}
