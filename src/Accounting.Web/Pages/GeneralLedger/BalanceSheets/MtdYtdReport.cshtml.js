$(function () {
    const l = abp.localization.getResource('Accounting');

    function getFormParams() {
        return {
            startDate: $('#startDate').val(),
            endDate: $('#endDate').val(),
            periodId: $('#periodId').val(),
        }
    }

    const accountTypeGroups = {
        assets: 1,
        liabilities: 2
    }
    const getTotalObject = () => ({
        total: 0,
        mtdTotal: 0,
        lastPeriodTotal: 0,
    })

    function setTotal(total, item, group) {
        total.total += item.nativeAmount * group.displayFactor;
        total.mtdTotal += item.monthToDateNativeAmount * group.displayFactor;
        total.lastPeriodTotal += item.lastPeriodNativeAmount * group.displayFactor;
    }
    const { createApp, ref, markRaw, computed } = Vue;
    const { defineStore, storeToRefs, createPinia } = Pinia;

    function handleData(items) {
        const groups = [];
        const total = {
            netAssets: 0,
            mtdNetAssets: 0,
            lastPeriodNetAssets: 0,
        }
        const secondaryGroups = {}
        items.reduce((prev, current) => {
            const code = current.groupCode;
            let group = prev[code];
            if (!group) {
                group = markRaw({
                    code: code,
                    item: current,
                    items: [],
                    ...getTotalObject(),
                    displayFactor: current.accountTypeGroup == accountTypeGroups.assets ? 1 : -1
                });
                prev[code] = group;
                groups.push(group);
            }

            const secondaryCode = current.groupCode + '__' + current.secondaryGroupCode;
            let secondaryGroup = secondaryGroups[secondaryCode];
            if (!secondaryGroup) {
                secondaryGroup = {
                    code: current.secondaryGroupCode,
                    item: current,
                    items: [],
                    ...getTotalObject()
                }
                secondaryGroups[secondaryCode] = secondaryGroup;
                group.items.push(secondaryGroup);
            } 
            
            secondaryGroup.items.push(current);

            setTotal(group, current, group) 
            setTotal(secondaryGroup, current, group) 

            if (current.accountTypeGroup == accountTypeGroups.assets ||
                current.accountTypeGroup == accountTypeGroups.liabilities) {
                total.netAssets += current.nativeAmount;
                total.mtdNetAssets += current.monthToDateNativeAmount;
                total.lastPeriodNetAssets += current.lastPeriodNativeAmount;
            }

            return prev;
        }, {});

        return {
            items: groups, total
        };
    }

    const useReportStore = defineStore('report', () => {
        const params = ref({})
        const items = ref([])
        const nativeCurrency = ref('')
        const total = ref({
            debitor: 0,
            creditor: 0,
            count: 0
        })

        const count = computed(() => items.value.length);

        const setItems = (payload) => {
            const { items: values, total: tempTotal } = handleData(payload.items);
            items.value = values;
            total.value = tempTotal;
        }
        const setParams = (payload) => {
            params.value = payload;
        }
        const setNativeCurrency = (payload) => {
            nativeCurrency.value = payload;
        }

        return { items, params, nativeCurrency, total, count, setItems, setParams, setNativeCurrency }
    });
       
    const headerTemplate = `
    <div class="header">
        <h3 class="text-center">{{l('BalanceSheet')}}</h3>
    </div>`;

    const Header = {
        template: headerTemplate,
        setup() {
            return {
                l
            }
        }
    }

    const bodyTemplate = `
    <div class="body"> 
        <table class="table table-borderless">
            <thead> 
                <tr class="border-bottom">
                    <th class="fw-bold">{{ l('SubjectCode') }}</th>
                    <th class="fw-bold">{{ l('SubjectName') }}</th>
                     <th class="text-end">{{ formatDate(params.startDate) }}&nbsp;{{ l('Before') }}</th>
                    <th class="text-end">{{ formatDate(params.startDate) }} - {{ formatDate(params.endDate) }}</th>
                    <th class="text-end">{{ l('AsAt') }}&nbsp;{{ formatDate(params.endDate) }}</th>
                </tr>
                <tr>
                    <th colspan="2"></th>
                    <th class="text-end">{{ nativeCurrency }}</th>
                    <th class="text-end">{{ nativeCurrency }}</th>
                </tr>
            </thead>
            <tbody>
                <template  v-for="item in items" :key="item.code">  
                      <tr class="fw-bold">
                        <td colspan="5">{{ item.item.groupName }}</td>
                     </tr>
                     <template  v-for="secondaryItem in item.items" :key="item.code + 'secondary'">
                        <tr>
                            <td colspan="5">{{ secondaryItem.item.secondaryGroupName }}</td>
                        </tr>
                        <tr v-for="subItem in secondaryItem.items" :key="secondaryItem.code">
                            <td>{{ subItem.subjectCode }}</td>
                            <td>{{ subItem.subjectName }}</td>
                            <td class="text-end">{{ renderAmount(subItem.lastPeriodNativeAmount * item.displayFactor) }}</td>
                            <td class="text-end">{{ renderAmount(subItem.monthToDateNativeAmount * item.displayFactor) }}</td>
                            <td class="text-end">{{ renderAmount(subItem.nativeAmount * item.displayFactor) }}</td>
                        </tr>
                         <tr :key="item.code + 'subTotal'" class="border-top fw-bold" >
                            <td class="text-end" colspan="2">{{ l('SubTotal') }}</td>
                             <td class="text-end">{{ renderAmount(secondaryItem.lastPeriodTotal) }} </td>
                            <td class="text-end">{{ renderAmount(secondaryItem.mtdTotal) }} </td>
                            <td class="text-end">{{ renderAmount(secondaryItem.total) }}  </td>
                       </tr>
                    </template>
                    <tr class="fw-bold" >
                            <td class="text-end" colspan="2">{{ l('TotalText') }}&nbsp;{{ item.item.groupName }}</td>
                            <td class="text-end border-bottom">{{ renderAmount(item.lastPeriodTotal) }}</td>
                            <td class="text-end border-bottom">{{ renderAmount(item.mtdTotal) }}</td>
                            <td class="text-end border-bottom">{{ renderAmount(item.total) }}</td>
                    </tr>
                     <tr v-if="item.item.accountTypeGroup == accountTypeGroups.liabilities" class="fw-bold" >
                        <td class="text-end" colspan="2">{{ l('NetAssets') }}</td>
                        <td class="text-end border-bottom"> {{ renderAmount(total.lastPeriodNetAssets) }} </td>
                        <td class="text-end border-bottom"> {{ renderAmount(total.mtdNetAssets) }} </td>
                        <td class="text-end border-bottom"> {{ renderAmount(total.netAssets) }} </td>
                    </tr>
                </template>
            </tbody>
        </table>
    </div>`;

    const Body = {
        template: bodyTemplate,
        setup() {
            const reportStore = useReportStore();
            const { items, nativeCurrency, total, params } = storeToRefs(reportStore);
            return {
                accountTypeGroups,
                items,
                nativeCurrency,
                total,
                params,
                formatDate,
                l,
                renderAmount,
            }
        }
    }

    const reportTemplate = `
 <div class="report">
    <Header />
    <Body/>
    <div v-if="count == 0" class="text-center">{{ l('NoDataAvailable') }}</div>
 </div>`;

    const Report = {
        components: { Header, Body },
        template: reportTemplate,
        setup() {
            const reportStore = useReportStore();
            const { count } = storeToRefs(reportStore);
            return {
                count,
                l
            }
        }
    }

    const app = createApp(Report);
    app.use(createPinia());
    app.mount('#app');

    const reportStore = useReportStore();

    reportStore.setParams(getFormParams());

    accounting.finance.accountingSetting.getNativeCurrency()
        .then(result => reportStore.setNativeCurrency(result))
        .catch(() => { });

    $(document).on('click', '#searchBtn', function () {
        const params = getFormParams();
        reportStore.setParams( params);
        reportStore.setItems({ items: [] });
        const busyEle = '.body';
        abp.ui.setBusy(busyEle);
        accounting.finance.reports.balanceSheetReport.getMtdYtdList(params).then(function (result) {
            reportStore.setItems({ items: result || [] });
            abp.ui.clearBusy(busyEle);
        }).catch(function () {
            abp.ui.clearBusy(busyEle);
        });
    });
    $(document).on('change', '#periodId', function (e) {
        const $this = $(this);
        const $option = $this.find('option:selected');
        const startDate = $option.attr('data-start-date');
        const endDate = $option.attr('data-end-date');
        $('#startDate').val(startDate);
        $('#endDate').val(endDate);
    });
});