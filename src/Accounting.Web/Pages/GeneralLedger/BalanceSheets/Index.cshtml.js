$(function () {
    const l = abp.localization.getResource('Accounting');

    function getFormParams() {
        return {
            endDate: $('#endDate').val(),
            periodId: $('#periodId').val(),
        }
    }
    const accountTypeGroups = {
        assets: 1,
        liabilities: 2
    }

    const { createApp, ref, markRaw, computed } = Vue;
    const { defineStore, storeToRefs, createPinia } = Pinia;

    function handleData(items) {
        const groups = [];
        const secondaryGroups = {}
        const total = {
            netAssets: 0,
        }
        items.reduce((prev, current) => {
            const code = current.groupCode;
            let group = prev[code];
            if (!group) {
                group = markRaw({
                    code: code,
                    item: current,
                    items: [],
                    total: 0,
                    displayFactor: current.accountTypeGroup == accountTypeGroups.assets ? 1 : -1
                });
                prev[code] = group;
                groups.push(group)
            }

            const secondaryCode = current.groupCode + '__' + current.secondaryGroupCode;
            let secondaryGroup = secondaryGroups[secondaryCode];
            if (!secondaryGroup) {
                secondaryGroup = {
                    code: current.secondaryGroupCode,
                    item: current,
                    total: 0,
                    items: []
                }
                secondaryGroups[secondaryCode] = secondaryGroup;
                group.items.push(secondaryGroup);
            }

            group.total += current.nativeAmount * group.displayFactor;

            secondaryGroup.items.push(current);
            secondaryGroup.total += current.nativeAmount * group.displayFactor;

            if (current.accountTypeGroup == accountTypeGroups.assets ||
                current.accountTypeGroup == accountTypeGroups.liabilities) {
                total.netAssets += current.nativeAmount
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
            netAssets: 0
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
                    <th class="fw-bold text-end">{{ l('AsAt') }}&nbsp;{{ formatDate(params.endDate) }}</th>
                </tr>
                <tr>
                    <th></th>
                    <th></th>
                    <th class="text-end">{{ nativeCurrency }}</th>
                </tr>
            </thead>
            <tbody>
                <template  v-for="item in items" :key="item.code">
                     <tr class="fw-bold">
                        <td colspan="4">{{ item.item.groupName }}</td>
                     </tr>
                     <template  v-for="secondaryItem in item.items" :key="item.code + 'secondary'">
                        <tr>
                        <td colspan="3">{{ secondaryItem.item.secondaryGroupName }}</td>
                        </tr>
                        <tr v-for="subItem in secondaryItem.items" :key="secondaryItem.code">
                            <td>{{ subItem.subjectCode }}</td>
                            <td>{{ subItem.subjectName }}</td>
                            <td class="text-end">{{ renderAmount(subItem.nativeAmount * item.displayFactor) }}</td>
                        </tr>
                       <tr class="border-top fw-bold" >
                            <td class="text-end" colspan="2">{{ l('SubTotal') }}</td>
                            <td class="text-end">
                                {{ renderAmount(secondaryItem.total) }}
                            </td>
                       </tr>
                    </template>
                    <tr class="fw-bold" >
                        <td class="text-end" colspan="2">{{ l('TotalText') }}&nbsp;{{ item.item.groupName }}</td>
                        <td class="text-end border-bottom">
                            {{ renderAmount(item.total) }}
                        </td>
                    </tr>
                    <tr v-if="item.item.accountTypeGroup == accountTypeGroups.liabilities" class="fw-bold" >
                        <td class="text-end" colspan="2">{{ l('NetAssets') }}</td>
                        <td class="text-end border-bottom">
                            {{ renderAmount(total.netAssets) }}
                        </td>
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
     
    accounting.finance.accountingSetting.getNativeCurrency()
        .then(result => reportStore.setNativeCurrency(result))
        .catch(() => { });


    function search() {
        const params = getFormParams();
        reportStore.setParams(params);
        reportStore.setItems({ items: [] });
        const busyEle = '.body';
        abp.ui.setBusy(busyEle);
        accounting.finance.reports.balanceSheetReport.getYtdList(params).then(function (result) {
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
        const endDate = $option.attr('data-end-date');
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