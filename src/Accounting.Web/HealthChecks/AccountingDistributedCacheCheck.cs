using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SixLabors.ImageSharp;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Accounting.Web.HealthChecks
{
    public class AccountingDistributedCacheCheck : IHealthCheck, ITransientDependency
    {
        private IConfiguration _configuration;
        public AccountingDistributedCacheCheck(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var connection = await ConnectionMultiplexer
               .ConnectAsync(_configuration["Redis:Configuration"]);
            if (connection.IsConnected)
            {
                return HealthCheckResult.Healthy("Redis is connected.");
            }
            else
            {
                return HealthCheckResult.Unhealthy("Redis is not connected.");
            }
        }
    }
}
