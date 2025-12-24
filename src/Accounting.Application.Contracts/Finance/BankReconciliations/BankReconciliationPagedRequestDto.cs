using System;
using Volo.Abp.Application.Dtos;

namespace Accounting.Finance.BankReconciliations
{
    public class BankReconciliationPagedRequestDto: PagedAndSortedResultRequestDto
    {
        public Guid? SubjectId { get; set; }
        public bool? IsPresented { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Prefix { get; set; }
        public int? StartNo { get; set; }
        public int? EndNo { get; set; }
        public string? ReferenceNo { get; set; }
    }
}
