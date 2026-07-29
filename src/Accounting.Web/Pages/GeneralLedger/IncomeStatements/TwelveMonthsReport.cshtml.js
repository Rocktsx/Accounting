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
    const getTotalObject = () => ({
        januaryTotal: 0,
        februaryTotal: 0,
        marchTotal: 0,
        aprilTotal: 0,
        mayTotal: 0,
        juneTotal: 0,
        julyTotal: 0,
        augustTotal: 0,
        septemberTotal: 0,
        octoberTotal: 0,
        novemberTotal: 0,
        decemberTotal: 0,
        total: 0,
    })

    function setTotal(total, item) {
        total.total += item.nativeAmount;
        total.januaryTotal += item.januaryNativeAmount;
        total.februaryTotal += item.februaryNativeAmount;
        total.marchTotal += item.marchNativeAmount;
        total.aprilTotal += item.aprilNativeAmount;
        total.mayTotal += item.mayNativeAmount;
        total.juneTotal += item.juneNativeAmount;
        total.julyTotal += item.julyNativeAmount;
        total.augustTotal += item.augustNativeAmount;
        total.septemberTotal += item.septemberNativeAmount;
        total.octoberTotal += item.octoberNativeAmount;
        total.novemberTotal += item.novemberNativeAmount;
        total.decemberTotal += item.decemberNativeAmount;
    }

    const { createApp, ref, markRaw } = Vue;
    const { defineStore, storeToRefs, createPinia } = Pinia;

    function handleData(items) {
        const groups = [];
        const total = {
            ...getTotalObject(),
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
                    ...getTotalObject(),
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
                    ...getTotalObject(),
                    incomeExpenses: current.category == category.income ? 1 : -1
                }
                secondaryGroups[secondaryCode] = secondaryGroup;
                group.items.push(secondaryGroup);
            }

            secondaryGroup.items.push(current);

            setTotal(group, current);
            setTotal(secondaryGroup, current);
            setTotal(total, current);

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
            ...getTotalObject(),
            count: 0
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
                <tr class="border-bottom fw-bold">
                    <th>{{ l('SubjectCode') }}</th>
                    <th>{{ l('SubjectName') }}</th> 
                    <th class="text-end">{{ l('January') }}</th>
                    <th class="text-end">{{ l('February') }}</th>
                    <th class="text-end">{{ l('March') }}</th>
                    <th class="text-end">{{ l('April') }}</th>
                    <th class="text-end">{{ l('May') }}</th>
                    <th class="text-end">{{ l('June') }}</th>
                    <th class="text-end">{{ l('July') }}</th>
                    <th class="text-end">{{ l('August') }}</th>
                    <th class="text-end">{{ l('September') }}</th>
                    <th class="text-end">{{ l('October') }}</th>
                    <th class="text-end">{{ l('November') }}</th>
                    <th class="text-end">{{ l('December') }}</th>
                    <th class="text-end">{{ l('AsAt') }}&nbsp;{{ formatDate(params.endDate) }}</th>
                </tr>
                <tr>
                    <th colspan="2"></th>
                    <td v-for="field in fields" :key="field" class="text-end">{{ nativeCurrency }} </td>
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
                            <td class="text-end">{{ renderAmount(Math.abs(subItem.januaryNativeAmount)) }}</td>
                            <td class="text-end">{{ renderAmount(Math.abs(subItem.februaryNativeAmount)) }}</td>
                            <td class="text-end">{{ renderAmount(Math.abs(subItem.marchNativeAmount)) }}</td>
                            <td class="text-end">{{ renderAmount(Math.abs(subItem.aprilNativeAmount)) }}</td>
                            <td class="text-end">{{ renderAmount(Math.abs(subItem.mayNativeAmount)) }}</td>
                            <td class="text-end">{{ renderAmount(Math.abs(subItem.juneNativeAmount)) }}</td>
                            <td class="text-end">{{ renderAmount(Math.abs(subItem.julyNativeAmount)) }}</td>
                            <td class="text-end">{{ renderAmount(Math.abs(subItem.augustNativeAmount)) }}</td>
                            <td class="text-end">{{ renderAmount(Math.abs(subItem.septemberNativeAmount)) }}</td>
                            <td class="text-end">{{ renderAmount(Math.abs(subItem.octoberNativeAmount)) }}</td>
                            <td class="text-end">{{ renderAmount(Math.abs(subItem.novemberNativeAmount)) }}</td>
                            <td class="text-end">{{ renderAmount(Math.abs(subItem.decemberNativeAmount)) }}</td>
                            <td class="text-end">{{ renderAmount(Math.abs(subItem.nativeAmount)) }}</td>
                        </tr>
                         <tr class="border-top fw-bold" >
                            <td class="text-end" colspan="2">{{ l('SubTotal') }}</td>
                            <td v-for="field in fields" :key="field" class="text-end">
                                {{ renderAmount(secondaryItem[field] * secondaryItem.incomeExpenses) }}
                            </td>
                       </tr>
                    </template>
                    <tr v-if="item.code == 1" class="fw-bold" >
                        <td class="text-end" colspan="2">{{ l('GrossProfit') }}</td>
                        <td v-for="field in fields" :key="field" class="text-end border-bottom">{{ renderAmount(item[field]) }} </td>
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
                        <td v-for="field in fields" :key="field" class="text-end border-bottom">{{ renderAmount(total[field]) }} </td>
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
                fields: Object.keys(getTotalObject()),
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
        accounting.finance.reports.profitAndLossReport.getTwelveMonthsList(params).then(function (result) {
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
            css: [$('[href*="bootstrap-dim.css"]').attr('href'), $('[href*="global-printa4landscape.css"]').attr('href')]
        });
    });
});