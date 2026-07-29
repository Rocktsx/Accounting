$(function () {
    const l = abp.localization.getResource('Accounting');

    function getFormParams() {
        return {
            startDate: $('#startDate').val(),
            endDate: $('#endDate').val(),
            periodId: $('#periodId').val(),
        }
    }

    const category = {
        income: 4,
        expenses: 5
    }

    const { createApp, ref, markRaw } = Vue;
    const { defineStore, storeToRefs, createPinia } = Pinia;

    function handleData(items) {
        const groups = [];
        const total = {
            total: 0,
            mtdTotal: 0,
            count: items.length,
        }
        const secondaryGroups = {}
        items.reduce((prev, current) => {
            const code = current.group;
            let group = prev[code];
            if (!group) {
                group = markRaw({
                    code: code,
                    item: current,
                    items: [],
                    total: 0,
                    mtdTotal: 0,
                });
                prev[code] = group;
                groups.push(group);
            }

            const secondaryCode = current.group + '__' + current.secondaryGroupCode;
            let secondaryGroup = secondaryGroups[secondaryCode];
            if (!secondaryGroup) {
                secondaryGroup = {
                    code: current.secondaryGroupCode,
                    item: current,
                    items: [],
                    total: 0,
                    mtdTotal: 0,
                    incomeExpenses: current.category == category.income ? 1 : -1
                }
                secondaryGroups[secondaryCode] = secondaryGroup;
                group.items.push(secondaryGroup);
            }
            secondaryGroup.items.push(current);

            group.total += current.nativeAmount;
            secondaryGroup.total += current.nativeAmount;
            total.total += current.nativeAmount;

            group.mtdTotal += current.monthToDateNativeAmount;
            secondaryGroup.mtdTotal += current.monthToDateNativeAmount;
            total.mtdTotal += current.monthToDateNativeAmount;

            return prev;
        }, {});

        return {
            items: groups, total
        };
    }
    const useReportStore = defineStore('report', () => {
        const params = ref({})
        const items = ref([])
        const total = ref({
            total: 0,
            count: 0,
            mtdTotal: 0
        })
        const nativeCurrency = ref('')

        const setItems = (payload) => {
            const result = handleData(payload.items);
            items.value = result.items;
            total.value = result.total;
        }
        const setParams = (payload) => {
            params.value = payload;
        }
        const setNativeCurrency = (payload) => {
            nativeCurrency.value = payload;
        }

        return { items, params, total, nativeCurrency, setItems, setParams, setNativeCurrency }
    });


    const headerTemplate = `
    <div class="header">
        <h3 class="text-center">{{l('ProfitAndLoss')}}</h3>
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
                     <template  v-for="secondaryItem in item.items" :key="secondaryItem.code">
                        <tr class="fw-bold">
                            <td colspan="4">{{ secondaryItem.item.secondaryGroupName }}</td>
                        </tr>
                        <tr v-for="subItem in secondaryItem.items" :key="subItem.subjectCode">
                            <td>{{ subItem.subjectCode }}</td>
                            <td>{{ subItem.subjectName }}</td> 
                            <td class="text-end">{{ renderAmount(Math.abs(subItem.monthToDateNativeAmount)) }}</td>
                            <td class="text-end">{{ renderAmount(Math.abs(subItem.nativeAmount)) }}</td>
                        </tr>
                         <tr class="border-top fw-bold" >
                            <td class="text-end" colspan="2">{{ l('SubTotal') }}</td>
                            <td class="text-end">
                                {{ renderAmount(secondaryItem.mtdTotal * secondaryItem.incomeExpenses) }}
                            </td>
                            <td class="text-end">
                                {{ renderAmount(secondaryItem.total * secondaryItem.incomeExpenses) }}
                            </td>
                       </tr>
                    </template>
                    <tr v-if="item.code == 1" class="fw-bold" >
                            <td class="text-end" colspan="2">{{ l('GrossProfit') }}</td>
                            <td class="text-end border-bottom">
                                {{ renderAmount(item.mtdTotal) }}
                            </td>
                            <td class="text-end border-bottom">
                                {{ renderAmount(item.total) }}
                            </td>
                    </tr>
                </template>
            </tbody>
            <tfoot v-if="items.length > 0">
                  <tr class="border-top fw-bold" >
                        <td class="text-end">{{ l('Items') }}</td>
                        <td>
                            <div class="row">
                                <div class="col">{{ total.count  }}</div>
                                <div class="col text-end">{{ l('NetProfit') }}</div>
                            </div>
                        </td>
                         <td class="text-end border-bottom">{{ renderAmount(total.mtdTotal) }}</td>
                        <td class="text-end border-bottom">{{ renderAmount(total.total) }}</td>
                  </tr >
            </tfoot>
        </table>
    </div>`;


    const Body = {
        template: bodyTemplate,
        setup() {
            const reportStore = useReportStore();
            const { items, nativeCurrency, total, params } = storeToRefs(reportStore);
            return {
                items,
                total,
                params,
                nativeCurrency,
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
    <div v-if="total.count == 0" class="text-center">{{ l('NoDataAvailable') }}</div>
 </div>`;

    const Report = {
        components: { Header, Body },
        template: reportTemplate,
        setup() {
            const reportStore = useReportStore();
            const { total } = storeToRefs(reportStore);
            return {
                total,
                l
            }
        }
    }

    const app = createApp(Report);
    app.use(createPinia());
    app.mount('#app');

    const reportStore = useReportStore(); 

    accounting.finance.accountingSetting.getNativeCurrency()
        .then(result => reportStore.setNativeCurrency(result))
        .catch(() => { });

    function search() {
        const params = getFormParams();
        reportStore.setParams(params);
        reportStore.setItems({ items: [] });
        const busyEle = '.body';
        abp.ui.setBusy(busyEle);
        accounting.finance.reports.profitAndLossReport.getMtdYtdList(params).then(function (result) {
            reportStore.setItems({ items: result || [] });
            abp.ui.clearBusy(busyEle);
        }).catch(function () {
            abp.ui.clearBusy(busyEle);
        });
    }
    search();

    $(document).on('click', '#searchBtn', function () {
        search();
    });
    $(document).on('change', '#periodId', function (e) {
        const $this = $(this);
        const $option = $this.find('option:selected');
        const startDate = $option.attr('data-start-date');
        const endDate = $option.attr('data-end-date');
        $('#startDate').val(startDate);
        $('#endDate').val(endDate);
    });

    $("#printBtn").on("click", function () { 
        printJS({
            printable: 'app', // 要打印的元素的ID
            type: 'html', // 打印类型，这里是HTML
            scanStyles: false,
            style: '', // 打印样式表
            css: [$('[href*="bootstrap-dim.css"]').attr('href'), $('[href*="global-print.css"]').attr('href')]
        });
    });
});