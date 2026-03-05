namespace Accounting.Finance.Settings;

public class AccountingSettingCompanyDto
{
    public string CompanyName { get; set; }
    public string CompanyOtherName { get; set; }
    public string CompanyAddress { get; set; }
    public string CompanyOtherAddress { get; set; }
    public string CompanyContact { get; set; }

    public AccountingSettingCompanyDto()
    {
        CompanyAddress = string.Empty;
        CompanyOtherAddress = string.Empty;
        CompanyContact = string.Empty;
        CompanyName = string.Empty;
        CompanyOtherName = string.Empty;
    }
}