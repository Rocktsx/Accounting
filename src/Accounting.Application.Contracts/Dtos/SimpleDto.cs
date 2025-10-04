using Volo.Abp.Application.Dtos;

namespace Accounting.Dtos
{
    public class SimpleDto<T>: EntityDto<T>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string OtherName { get; set; }
    }
}
