using Accounting.Finance.Vouchers;
using Accounting.Models;
using Accounting.Permissions;
using Accounting.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Controllers
{
    [Route("api/transfer-vouchers")]
    public class TransferVoucherController : AccountingController
    {
        private readonly IImportVoucherAppService _importVoucherAppService;

        public TransferVoucherController(IImportVoucherAppService importVoucherAppService)
        {
            _importVoucherAppService = importVoucherAppService;
        }
        [Authorize(AccountingPermissions.TransferVouchers.Import)]
        public async Task<IResult> ImportData(IFormFile file, VoucherImportType? importType, string? subjectCode)
        {
            var importer = file.GetImporter(L, LazyServiceProvider);
            var result = 0;
            var stream = file.OpenReadStream();
            var importResult = await importer.Import<VoucherImportItemModel>(stream);
            importResult.HandleErrors(L);
            var data = ObjectMapper.Map<List<VoucherImportItemModel>, List<VoucherImportItemDto>>(importResult.Data.ToList());
            result = await _importVoucherAppService.ImportDataAsync(new VoucherImportDto
            {
                ImportType = importType ?? VoucherImportType.DoubleEntry,
                SubjectCode = subjectCode,
                Data = data
            });

            return Results.Json(new { Count = result });
        }
    }
}
