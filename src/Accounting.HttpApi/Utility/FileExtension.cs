 using Magicodes.ExporterAndImporter.Core; 
using Magicodes.ExporterAndImporter.Csv;
using Magicodes.ExporterAndImporter.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization; 
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Accounting.Utility
{
    public static class FileExtension
    {
        public const string CsvFileExtension = ".csv";
        public const string XslxFileExtension = ".xlsx";
        public static IImporter GetImporter(this IFormFile file, IStringLocalizer localizer, IAbpLazyServiceProvider lazyServiceProvider)
        {
            IImporter importer = null;
            if (file.FileName.EndsWith(CsvFileExtension))
            {
                importer = (IImporter)lazyServiceProvider.LazyGetRequiredService<ICsvImporter>();
            }
            else if (file.FileName.EndsWith(XslxFileExtension))
            {
                importer = (IImporter)lazyServiceProvider.LazyGetRequiredService<IExcelImporter>();
            }
            else
            {
                throw new UserFriendlyException(localizer["UnsupportFileExtension"]);
            }
            return importer;
        }
    }
}
