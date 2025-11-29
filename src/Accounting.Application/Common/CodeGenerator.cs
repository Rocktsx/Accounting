using System;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DistributedLocking;

namespace Accounting.Common
{
    public class CodeGenerator
    {
        IAbpDistributedLock _distributedLock;
        IDistributedCache<CodeCacheItem> _cache;
        TimeSpan _waitTime = TimeSpan.FromSeconds(3);
        private const string Format = "{0:###0000}";
        public CodeGenerator(IAbpDistributedLock distributedLock,
            IDistributedCache<CodeCacheItem> cache)
        {
            _distributedLock = distributedLock;
            _cache = cache;
        }
        public async Task LockAsync(string lockName, Func<Task> action)
        {
            await using var handle = await _distributedLock.TryAcquireAsync(
                 lockName, _waitTime);
            if (handle != null)
            {
                await action();
            }
        }
        public async Task GenerateCodeAsync(
            IGenerateCode obj, CodeCacheItem cacheItem, Func<string> getPrefix = null,
            Func<string, Task<int>> getLastNumber = null, string format = null)
        {
            var prefix = getPrefix == null ? obj.Prefix : getPrefix();
            cacheItem.Prefix = prefix;
            var cacheKey = cacheItem.GetKey();
            await LockAsync(cacheKey, async () =>
            {
                var existsCacheItem = await _cache.GetAsync(cacheKey);
                if (existsCacheItem == null)
                {
                    var lastNum = getLastNumber == null ? 0 : await getLastNumber(prefix);
                    cacheItem.LastNumber = lastNum;
                    existsCacheItem = cacheItem;
                }

                existsCacheItem.LastNumber += 1;
                cacheItem.LastNumber = existsCacheItem.LastNumber;
                await _cache.SetAsync(cacheKey, existsCacheItem);
            });

            var lastNo = cacheItem.LastNumber;
            var codeFormat = string.IsNullOrWhiteSpace(format) ? Format : format;
            var code = $"{prefix}-{string.Format(codeFormat, lastNo)}";
            obj.SetCode(code, prefix, lastNo);
        }
    }
}
