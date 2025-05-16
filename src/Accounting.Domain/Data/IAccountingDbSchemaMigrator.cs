using System.Threading.Tasks;

namespace Accounting.Data;

public interface IAccountingDbSchemaMigrator
{
    Task MigrateAsync();
}
