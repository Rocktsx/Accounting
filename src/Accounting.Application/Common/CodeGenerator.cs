using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace Accounting.Common
{
    public class CodeGenerator
    {
        IAbpDistributedLock _distributedLock;
        IDistributedCache<CodeCacheItem> _cache;
        IAsyncQueryableExecuter AsyncExecuter { get; set; }
        TimeSpan _waitTime = TimeSpan.FromSeconds(3);
        private const string Format = "{0:###0000}";
        public CodeGenerator(IAbpDistributedLock distributedLock,
            IDistributedCache<CodeCacheItem> cache,
            IAsyncQueryableExecuter asyncExecuter)
        {
            _distributedLock = distributedLock;
            _cache = cache;
            AsyncExecuter = asyncExecuter;
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
        public async Task GenerateCodeAsync<TEntity, TKey>(
            IGenerateCode obj, IRepository<TEntity, TKey> repository,
            CodeCacheItem cacheItem, Func<string> getPrefix = null,
            string format = "")
            where TEntity : class, IAggregateRoot<TKey>, IGenerateCode
        {
            var prefix = getPrefix == null ? obj.Prefix : getPrefix();
            cacheItem.Prefix = prefix;
            var cacheKey = cacheItem.GetKey();
            await LockAsync(cacheKey, async () =>
            {
                var existsCacheItem = await _cache.GetAsync(cacheKey);
                if (existsCacheItem == null)
                {
                    var queryable = await repository.GetQueryableAsync();
                    var lastNum = await AsyncExecuter.FirstOrDefaultAsync(
                        queryable.Where(item => item.Prefix == prefix).
                        OrderByDescending(item => item.GenNo).Select(
                            item => item.GenNo));
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
