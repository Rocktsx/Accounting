using Accounting.Finance.SubjectCategories;
using Accounting.Finance.Subjects;
using Accounting.Models;
using Accounting.Permissions;
using Accounting.Utility;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Core.Models;
using Magicodes.ExporterAndImporter.Csv;
using Magicodes.ExporterAndImporter.Excel;
using Magicodes.IE.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
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
    [Route("api/subjects")]
    public class SubjectsController : AccountingController
    {
        private readonly ISubjectAppService _subjectAppService;

        public SubjectsController(ISubjectAppService subjectAppService)
        {
            _subjectAppService = subjectAppService;
        }

        [HttpPost]
        [Authorize(AccountingPermissions.Subjects.Import)]
        public async Task<IResult> ImportData(IFormFile file)
        {
            var importer = file.GetImporter(L, LazyServiceProvider);
            var result = 0;
            var stream = file.OpenReadStream();
            var importResult = await importer.Import<SubjectImportModel>(stream);
            importResult.HandleErrors(L);
            if (importResult.Data != null)
            {
                var data = ObjectMapper.Map<List<SubjectImportModel>, List<SubjectImportDto>>(importResult.Data.ToList());
                result = await _subjectAppService.ImportDataAsync(data);
            }

            return Results.Json(new { Count = result });
        }
    }
}
