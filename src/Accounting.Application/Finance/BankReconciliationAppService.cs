using Accounting.Finance.BankReconciliations;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance
{
    public class BankReconciliationAppService : AccountingAppService, IBankReconciliationAppService
    {
        private readonly IBankReconciliationRepository _bankReconciliationRepository;

        public BankReconciliationAppService(IBankReconciliationRepository bankReconciliationRepository)
        {
            _bankReconciliationRepository = bankReconciliationRepository;
        }

        [Authorize(AccountingPermissions.BankReconciliations.Create)]
        public async Task<BankReconciliationDto> CreateAsync(BankReconciliationCreateDto input)
        {
            var entity = new BankReconciliation(GuidGenerator.Create(), input.VoucherDetailId, input.IsPresented);
            entity = await _bankReconciliationRepository.InsertAsync(entity);

            return ObjectMapper.Map<BankReconciliation, BankReconciliationDto>(entity);
        }

        [Authorize(AccountingPermissions.BankReconciliations.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            await _bankReconciliationRepository.DeleteAsync(id);
        }

        [Authorize(AccountingPermissions.BankReconciliations.Default)]
        public async Task<BankReconciliationDto> GetAsync(Guid id)
        {
            var entity = await _bankReconciliationRepository.GetAsync(id);

            return ObjectMapper.Map<BankReconciliation, BankReconciliationDto>(entity);
        }

        [Authorize(AccountingPermissions.BankReconciliations.Default)]
        public async Task<PagedResultDto<BankReconciliationPagedResultDto>> GetListAsync(BankReconciliationPagedRequestDto input)
        {
            var request = ObjectMapper.Map<BankReconciliationPagedRequestDto, BankReconciliationFilterRequest>(input);
            var list = await _bankReconciliationRepository.GetPagedListAsync(request,
                input.Sorting, input.MaxResultCount, input.SkipCount);
            var count = await _bankReconciliationRepository.GetCountAsync(request);
            var result = ObjectMapper.Map<IEnumerable<BankReconciliationPagedResult>, List<BankReconciliationPagedResultDto>>(list);
            return new PagedResultDto<BankReconciliationPagedResultDto>(count, result);
        }

        [Authorize(AccountingPermissions.BankReconciliations.Update)]
        public async Task<BankReconciliationDto> UpdateAsync(Guid id, BankReconciliationUpdateDto input)
        {
            var entity = await _bankReconciliationRepository.GetAsync(id);

            entity.SetIsPresented(input.IsPresented);

            entity = await _bankReconciliationRepository.UpdateAsync(entity);

            return ObjectMapper.Map<BankReconciliation, BankReconciliationDto>(entity);
        }
    }
}
