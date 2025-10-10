using Accounting.BasicData.Companies;
using Accounting.Finance.SubjectCategories;
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
    [Route("api/companies")]
    public class CompaniesController : AccountingController
    {
        private ICompanyAppService _companyAppService = null;
       
        [HttpPost]
        [Route("clients/import")]
        [Authorize(AccountingPermissions.Clients.Import)]
        public async Task<IResult> ImportClientData(IFormFile file)
        { 
            _companyAppService = LazyServiceProvider.LazyGetRequiredService<IClientAppService>();
            return await ImportCompanyData(file);
        }
        [HttpPost]
        [Route("vendors/import")]
        [Authorize(AccountingPermissions.Vendors.Import)]
        public async Task<IResult> ImportVendorData(IFormFile file)
        {
            _companyAppService = LazyServiceProvider.LazyGetRequiredService<IVendorAppService>();
            return await ImportCompanyData(file);
        }
        private async Task<IResult> ImportCompanyData(IFormFile file)
        {
            var importer = file.GetImporter(L, LazyServiceProvider);
            var result = 0;
            if (importer != null)
            {
                var stream = file.OpenReadStream();
                var importResult = await importer.Import<CompanyImportModel>(stream);
                importResult.HandleErrors(L);
                if (importResult.Data != null)
                {
                    var data = ObjectMapper.Map<List<CompanyImportModel>, List<CompanyImportDto>>(importResult.Data.ToList());
                    result = await _companyAppService.ImportDataAsync(data);
                }
            }

            return Results.Json(new { Count = result });
        }
    }
}
