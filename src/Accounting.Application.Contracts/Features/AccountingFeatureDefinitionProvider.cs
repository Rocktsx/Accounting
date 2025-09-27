using Accounting.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

namespace Accounting.Features
{
    public class AccountingFeatureDefinitionProvider : FeatureDefinitionProvider
    {
        public override void Define(IFeatureDefinitionContext context)
        {
            var myGroup = context.AddGroup(AccountingFeatures.GroupName, L(AccountingFeatures.GroupName));

            myGroup.AddFeature(
                AccountingFeatures.ProjectFunction,
                defaultValue: "false",
                displayName: L(AccountingFeatures.ProjectFunction),
                isVisibleToClients: false,
                valueType: new ToggleStringValueType());

            myGroup.AddFeature(
                AccountingFeatures.RegionFunction,
                defaultValue: "false",
                displayName: L(AccountingFeatures.RegionFunction),
                isVisibleToClients: false,
                valueType: new ToggleStringValueType());

            myGroup.AddFeature(
                AccountingFeatures.DepartmentFunction,
                defaultValue: "false",
                displayName: L(AccountingFeatures.DepartmentFunction),
                isVisibleToClients: false,
                valueType: new ToggleStringValueType());

            myGroup.AddFeature(
                AccountingFeatures.Custom1Function,
                defaultValue: "false",
                displayName: L(AccountingFeatures.Custom1Function),
                isVisibleToClients: false,
                valueType: new ToggleStringValueType());

            myGroup.AddFeature(
                AccountingFeatures.Custom2Function,
                defaultValue: "false",
                displayName: L(AccountingFeatures.Custom2Function),
                isVisibleToClients: false,
                valueType: new ToggleStringValueType());
        }
        private ILocalizableString L(string name)
        {
            return LocalizableString.Create<AccountingResource>(name);
        }
    }
}
