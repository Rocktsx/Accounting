using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.VirtualFileSystem;

namespace Accounting.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/templates")]
    public class TemplatesController : AccountingController
    {
        private readonly IVirtualFileProvider _fileProvider;
        public TemplatesController(IVirtualFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }
        [HttpGet("{template}")]
        [Authorize]
        public async Task<IActionResult> GetTemplate(string template)
        {
            var fileName = template + ".xlsx";

            var filePath = Path.Combine("Templates/", fileName);
            var fileInfo = _fileProvider.GetFileInfo(filePath);
            if (!fileInfo.Exists)
            {
                Logger.LogWarning("Template file not found: {FilePath}", filePath);
                return NotFound();
            }

            var stream = fileInfo.CreateReadStream();
            HttpContext.Response.Headers.Append("Content-Disposition", $"Attachment; filename={fileName}");
            HttpContext.Response.Headers.Append("Content-Length", stream.Length.ToString());
            HttpContext.Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition, Request-Context");
            return new FileStreamResult(stream, "application/octet-stream");
        }
    }
}
