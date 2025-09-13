using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounting.Finance.Dtos;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace Accounting.Finance;

public abstract class VoucherAppServiceTests<TStartupModule> : AccountingApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IVoucherAppService _voucherAppService;
    private readonly ISubjectAppService _subjectAppService;
    private const string BankSubjectCode = "2801";
    private const string RentAndRatesSubjectCode = "8021";

    public VoucherAppServiceTests()
    {
        _voucherAppService = GetRequiredService<IVoucherAppService>();
        _subjectAppService = GetRequiredService<ISubjectAppService>();
    }

    private async Task<VoucherCreateDto> GetCreateDtoAsync()
    {
        var subjects = await _subjectAppService.GetSimpleListAsync();
        var dto = new VoucherCreateDto()
        {
            Prefix = "JV",
            VoucherType = VoucherType.JournalVoucher,
            VoucherDate = new DateOnly(2025, 1, 12),
            Details =
            [
                new VoucherDetailCreateDto()
                {
                    CurrencyCode = "RMB",
                    CurrencyRate = 1m,
                    ForeignAmount = 1000m,
                    NativeAmount = 1000m,
                    Description = "Rent & Rates 2025 01",
                    SubjectId = subjects.First(item => item.Code == BankSubjectCode).Id,
                    DebitorCreditor = DebitorCreditor.Creditor
                },
                new VoucherDetailCreateDto()
                {
                    CurrencyCode = "RMB",
                    CurrencyRate = 1m,
                    ForeignAmount = 1000m,
                    NativeAmount = 1000m,
                    Description = "Rent & Rates 2025 01",
                    SubjectId = subjects.First(item => item.Code == RentAndRatesSubjectCode).Id,
                    DebitorCreditor = DebitorCreditor.Debitor,
                    ItemQty = 0,
                    IsOriginal = false
                }
            ]
        };

        return dto;
    }

    private VoucherDetailUpdateDto GetDetailUpdateDto(VoucherDetailDto input)
    {
        return new VoucherDetailUpdateDto()
        {
            Id = input.Id,
            CurrencyCode = input.CurrencyCode,
            CurrencyRate = input.CurrencyRate,
            ForeignAmount = input.ForeignAmount,
            NativeAmount = input.NativeAmount,
            Description = input.Description,
            Custom1 = input.Custom1,
            Custom2 = input.Custom2,
            DebitorCreditor = input.DebitorCreditor,
            ItemQty = input.ItemQty,
            IsOriginal = false,
            Region = input.Region,
            Department = input.Department,
            DueDate = input.DueDate,
            SubjectId = input.SubjectId,
            SubSubjectCode = input.SubSubjectCode
        };
    }

    [Fact]
    public async Task Can_Create_Voucher()
    {
        // Arrange
        var createDto = await GetCreateDtoAsync();

        // Act
        var dto = await _voucherAppService.CreateAsync(createDto);

        // Assert
        dto.ShouldNotBeNull();
        dto.Id.ShouldNotBe(Guid.Empty);
        dto.VoucherType.ShouldBe(VoucherType.JournalVoucher);
        dto.VoucherDate.ShouldBe(new DateOnly(2025, 1, 12));
        dto.Status.ShouldBe(VoucherStatus.Draft);
        dto.Details.ShouldNotBeNull();
        dto.Details.Count().ShouldBe(2);
    }

    [Fact]
    public async Task Cannot_Create_Voucher_With_Empty_Prefix()
    {
        // Arrange
        var createDto = await GetCreateDtoAsync();
        createDto.Prefix = string.Empty;

        // Act
        var exception = await Should.ThrowAsync<AbpValidationException>(async () =>
            await _voucherAppService.CreateAsync(createDto));

        // Assert
        exception.ShouldNotBeNull();
    }

    [Fact]
    public async Task Cannot_Create_Voucher_With_Out_Period_VoucherDate()
    {
        // Arrange
        var createDto = await GetCreateDtoAsync();
        createDto.VoucherDate = new DateOnly(2024, 1, 12);

        // Act
        var exception = await Should.ThrowAsync<BusinessException>(async () =>
            await _voucherAppService.CreateAsync(createDto));

        // Assert
        exception.ShouldNotBeNull();
    }

    [Fact]
    public async Task Can_Update_Voucher()
    {
        // Arrange
        var createDto = await GetCreateDtoAsync();
        var dto = await _voucherAppService.CreateAsync(createDto);
        var firstDetailItem = GetDetailUpdateDto(dto.Details.First());
        firstDetailItem.ForeignAmount = 1220m;
        firstDetailItem.NativeAmount = 1220m;
        var secondDetailItem = GetDetailUpdateDto(dto.Details.Last());
        secondDetailItem.ForeignAmount = 1220m;
        secondDetailItem.NativeAmount = 1220m;
        var oldDetailId = secondDetailItem.Id;
        secondDetailItem.Id = Guid.Empty;
        var voucherDate = new DateOnly(2025, 10, 10);
        var updateDto = new VoucherUpdateDto()
        {
            VoucherDate = voucherDate,
            Status = VoucherStatus.Approval,
            Details = [firstDetailItem, secondDetailItem]
        };

        // Act
        var updatedDto = await _voucherAppService.UpdateAsync(dto.Id, updateDto);

        // Assert
        updatedDto.ShouldNotBeNull();
        updatedDto.VoucherDate.ShouldBe(voucherDate);
        updatedDto.Status.ShouldBe(VoucherStatus.Approval);
        updatedDto.Details.ShouldNotBeNull();
        updatedDto.Details.Count().ShouldBe(2);
        updatedDto.Details.ShouldContain(item =>
            item.Id == firstDetailItem.Id && item.ForeignAmount == firstDetailItem.ForeignAmount &&
            item.NativeAmount == firstDetailItem.NativeAmount);
        updatedDto.Details.ShouldNotContain(item => item.Id == oldDetailId);
    }

    [Fact]
    public async Task Can_Get_A_Voucher()
    {
        // Arrange
        var createDto = await GetCreateDtoAsync();
        var dto = await _voucherAppService.CreateAsync(createDto);

        // Act
        var result = await _voucherAppService.GetAsync(dto.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.VoucherType.ShouldBe(VoucherType.JournalVoucher);
        result.VoucherDate.ShouldBe(new DateOnly(2025, 1, 12));
        result.Status.ShouldBe(VoucherStatus.Draft);
        result.Details.ShouldNotBeNull();
        result.Details.Count().ShouldBe(2);
        result.Details.ShouldContain(item => item.Id == dto.Details.First().Id);
        result.Details.ShouldContain(item => item.Id == dto.Details.Last().Id);
    }

    [Fact]
    public async Task Can_Delete_Voucher()
    {
        // Arrange
        var createDto = await GetCreateDtoAsync();
        var dto = await _voucherAppService.CreateAsync(createDto);

        // Act
        await _voucherAppService.DeleteAsync(dto.Id);

        var exception = await Should.ThrowAsync<EntityNotFoundException>(async () =>
        {
            await _voucherAppService.GetAsync(dto.Id);
        });

        // Assert
        exception.ShouldNotBeNull();
    }

    [Fact]
    public async Task Can_Get_Voucher_List_With_Filter()
    {
        // Arrange
        var createDto = await GetCreateDtoAsync();
        var dto = await _voucherAppService.CreateAsync(createDto);

        // Act
        var result = await _voucherAppService.GetListAsync(new VoucherFilterRequestDto()
        {
            MaxResultCount = 10,
            Filter = dto.Code
        });

        // Assert
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(1);
        result.Items.ShouldContain(x => x.Id == dto.Id);
        result.Items.ShouldContain(x => x.Code == dto.Code);
    }

    [Fact]
    public async Task Can_Get_Voucher_List_With_Voucher_Type()
    {
        // Arrange
        var createDto = await GetCreateDtoAsync();
        await _voucherAppService.CreateAsync(createDto);

        // Act
        var result = await _voucherAppService.GetListAsync(new VoucherFilterRequestDto()
        {
            MaxResultCount = 10,
            VoucherType = VoucherType.PayableVoucher
        });

        // Assert
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Can_Get_Voucher_List_With_Voucher_Date()
    {
        // Arrange
        var createDto = await GetCreateDtoAsync();
        await _voucherAppService.CreateAsync(createDto);

        // Act
        var result = await _voucherAppService.GetListAsync(new VoucherFilterRequestDto()
        {
            MaxResultCount = 10,
            StartDate = new DateOnly(2025, 1, 10),
            EndDate = new DateOnly(2025, 1, 12),
        });

        // Assert
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(1);
        result.Items.First().VoucherDate.ShouldBe(new DateOnly(2025, 1, 12));
    }

    [Fact]
    public async Task Can_Get_Voucher_List_With_Status()
    {
        // Arrange
        var createDto = await GetCreateDtoAsync();
        var dto = await _voucherAppService.CreateAsync(createDto);
        var firstDetailItem = GetDetailUpdateDto(dto.Details.First());
        var secondDetailItem = GetDetailUpdateDto(dto.Details.Last());
        var voucherDate = new DateOnly(2025, 10, 10);
        var updateDto = new VoucherUpdateDto()
        {
            VoucherDate = voucherDate,
            Status = VoucherStatus.Approval,
            Details = [firstDetailItem, secondDetailItem]
        };
        await _voucherAppService.UpdateAsync(dto.Id, updateDto);

        //Act
        var result = await _voucherAppService.GetListAsync(new VoucherFilterRequestDto()
        {
            MaxResultCount = 10,
            Status = VoucherStatus.Approval
        });

        // Assert
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(1);
        result.Items.First().VoucherDate.ShouldBe(voucherDate);
    }
    [Fact]
    public async Task Can_Get_Voucher_List_With_No_Void_Status()
    {
        // Arrange
        var createDto = await GetCreateDtoAsync();
        var dto = await _voucherAppService.CreateAsync(createDto);
        var firstDetailItem = GetDetailUpdateDto(dto.Details.First());
        var secondDetailItem = GetDetailUpdateDto(dto.Details.Last()); 
        var updateDto = new VoucherUpdateDto()
        {
            VoucherDate = new DateOnly(2025, 10, 10),
            Status = VoucherStatus.Void,
            Details = [firstDetailItem, secondDetailItem]
        };
        await _voucherAppService.UpdateAsync(dto.Id, updateDto);

        //Act
        var result = await _voucherAppService.GetListAsync(new VoucherFilterRequestDto()
        {
            MaxResultCount = 10
        });

        // Assert
        result.ShouldNotBeNull(); 
        result.Items.Count.ShouldBe(1);
        result.Items.ShouldNotContain(item => item.Status == VoucherStatus.Void);
    }
    [Fact]
    public async Task Can_Get_Voucher_List_With_Prefix()
    {
        // Arrange
        var createDto = await GetCreateDtoAsync();
        createDto.Prefix = "TV";
        var dto = await _voucherAppService.CreateAsync(createDto); 

        //Act
        var result = await _voucherAppService.GetListAsync(new VoucherFilterRequestDto()
        {
            MaxResultCount = 10,
            Prefix = "TV"
        });

        // Assert
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(1); 
        result.Items.First().Prefix.ShouldBe("TV");
    }
    [Fact]
    public async Task Can_Get_Voucher_List_Start_No()
    {
        // Arrange
        var createDto = await GetCreateDtoAsync(); 
        var dto = await _voucherAppService.CreateAsync(createDto);

        //Act
        var result = await _voucherAppService.GetListAsync(new VoucherFilterRequestDto()
        {
            MaxResultCount = 10,
            StartNo = dto.GenNo
        });

        // Assert
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(1);
        result.Items.First().GenNo.ShouldBe(dto.GenNo);
    }
    [Fact]
    public async Task Can_Get_Voucher_List_End_No()
    {
        // Arrange
        var createDto = await GetCreateDtoAsync();
        var dto = await _voucherAppService.CreateAsync(createDto);

        //Act
        var result = await _voucherAppService.GetListAsync(new VoucherFilterRequestDto()
        {
            MaxResultCount = 10,
            EndNo = dto.GenNo
        });

        // Assert
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(2);
        result.Items.ShouldContain(item => item.GenNo == dto.GenNo);
    }
    [Fact]
    public async Task Can_Get_Voucher_List_Doc_No()
    {
        // Arrange
        var createDto = await GetCreateDtoAsync();
        var docNo = "si202401";
        createDto.Details.First().DocNo = docNo;
        var dto = await _voucherAppService.CreateAsync(createDto);

        //Act
        var result = await _voucherAppService.GetListAsync(new VoucherFilterRequestDto()
        {
            MaxResultCount = 10,
            DocNo = docNo
        });

        // Assert
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(1);
        result.Items.First().Id.ShouldBe(dto.Id);
    }
}