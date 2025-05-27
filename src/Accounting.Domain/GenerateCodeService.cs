using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Accounting
{
    public class GenerateCodeService : DomainService
    {
        public async Task GenerateCodeAsync<TEntity,TKey>(IGenerateCode obj, IRepository<TEntity, TKey> repository ) where TEntity : class, IAggregateRoot<TKey>, IGenerateCode
        { 
            Check.NotNullOrWhiteSpace(obj.Prefix, nameof(obj.Prefix));
            var prefix = obj.Prefix;  
            var queryable = await repository.GetQueryableAsync(); 
            var lastNum = queryable.Where(item => item.Prefix == prefix).OrderByDescending(item => item.GenNo).Select(item => item.GenNo).FirstOrDefault();
            var lastGenNo = lastNum + 1;
            var code = $"{prefix}-{string.Format("{0:###0000}", lastGenNo)}";
            obj.SetCode(code, prefix, lastGenNo);
        } 
    }
}
