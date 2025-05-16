using Microsoft.AspNetCore.Builder;
using Accounting;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("Accounting.Web.csproj"); 
await builder.RunAbpModuleAsync<AccountingWebTestModule>(applicationName: "Accounting.Web");

public partial class Program
{
}
