using Accounting.Finance;
using Accounting.Finance.Dtos;
using Accounting.Models;
using Accounting.Permissions;
using Accounting.Utility;
using CsvHelper.TypeConversion;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Core.Models;
using Magicodes.ExporterAndImporter.Csv;
using Magicodes.ExporterAndImporter.Excel;
using Magicodes.IE.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;

namespace Accounting.Controllers
{
    [Route("api/subject-category")]
    public class SubjectCategoryController : AccountingController
    {
        private readonly ISubjectCategoryAppService _subjectCategoryAppService;

        public SubjectCategoryController(ISubjectCategoryAppService subjectCategoryAppService)
        {
            _subjectCategoryAppService = subjectCategoryAppService;
        }

        [HttpPost]
        [Authorize(AccountingPermissions.GeneralAccounts.Import)]
        public async Task<IResult> ImportData(IFormFile file)
        {
            IImporter importer = null;
            ImportResult<SubjectCategoryImportModel> importResult = null;
            var result = 0;
            var stream = file.OpenReadStream();
            if (file.FileName.EndsWith(".csv"))
            {
                importer = new CsvImporter();
            }
            else if (file.FileName.EndsWith(".xlsx"))
            {
                importer = new ExcelImporter();
            }
            else
            {
                throw new UserFriendlyException(L["UnsupportFileExtension"]);
            }
            if (importer != null)
            {
                importResult = await importer.Import<SubjectCategoryImportModel>(stream);
                importResult.HandleErrors(L);
                if (importResult.Data != null)
                {
                    var data = ObjectMapper.Map<List<SubjectCategoryImportModel>, List<SubjectCategoryImportDto>>(importResult.Data.ToList());
                    result = await _subjectCategoryAppService.ImportData(data);
                }
            }

            return Results.Json(new { Count = result });
        }
    }
}
