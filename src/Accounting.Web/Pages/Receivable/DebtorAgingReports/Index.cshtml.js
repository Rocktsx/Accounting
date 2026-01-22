$(function () {
    const l = abp.localization.getResource('Accounting');
    const { createApp, ref, computed } = Vue;
    const { defineStore, storeToRefs, createPinia } = Pinia;

    function initCompanySelect() {
        const $target = $('#clientId');
        const url = '/api/app/client';
        const language = getSelect2Language(); 
        $target.attr('data-language', language);
        $target.select2({
            ajax: {
                url: url,
                delay: 250,
                dataType: "json",
                data: function (params) {
                    return { filter: params.term || '', maxResultCount: 10 };
                },
                processResults: function (data) {
                    const items = data.items;
                    const results = items.map(function (item) {
                        const { name, id, code } = item;
                        const text = code + ' - ' + name;
                        return {
                            id,
                            text: text,
                            displayName: text
                        }
                    }); 
                    return { results: results };
                }
            },
            width: '100%',
            dropdownParent: null,
            placeholder: '',
            allowClear: true,
            language: language
        });
    }
    function getFormParams() {
        return {
            subSubjectCode: $('#clientId').val(), 
            endDate: $('#endDate').val(),
            agingDays: Number($('#agingDays').val()),
        }
    }
    function getTotal() {
        return {
            netOsBalance: 0,
            osBalance: 0,
            prepaidAmount: 0,
            overdueAmount1: 0,
            overdueAmount2: 0,
            overdueAmount3: 0,
            overdueAmount4: 0,
            overdueAmount5: 0
        }
    }
    function handleData(items) {
        const total = getTotal();
        items.forEach(item => {
            total.netOsBalance += item.outstandingAmount - item.prepaidDeposit;
            total.osBalance += item.outstandingAmount;
            total.prepaidAmount += item.prepaidDeposit;
            total.overdueAmount1 += item.overdueAmount1;
            total.overdueAmount2 += item.overdueAmount2;
            total.overdueAmount3 += item.overdueAmount3;
            total.overdueAmount4 += item.overdueAmount4;
            total.overdueAmount5 += item.outstandingAmount - item.overdueAmount4 -
                item.overdueAmount3 - item.overdueAmount2 - item.overdueAmount1;
        });

        return total
    }
    const useReportStore = defineStore('report', () => {
        const params = ref({})
        const items = ref([])
        const total = ref(getTotal())
        const nativeCurrency = ref('')
        const setItems = (payload) => {
            items.value = payload.items;
            total.value = handleData(payload.items);
        }
        const setParams = (payload) => {
            params.value = payload;
        }
        const setNativeCurrency = (payload) => {
            nativeCurrency.value = payload;
        }
        const count = computed(() => items.value.length);

        return { items, params, total, nativeCurrency, setItems, setParams, setNativeCurrency, count }
    });
     
    const headerTemplate = `
    <div class="header">
        <h3 class="text-center">{{l('ReceivableAgingReport')}}</h3>
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
                <tr>
                    <th colspan="3"></th>
                   <th colspan="5" class="fw-bold border-bottom text-center">{{ l('OverdueDays') }}</th>
                   <th colspan="3"></th>
                </tr>
                <tr class="border-bottom">
                    <th class="fw-bold">{{ l('ClientCode') }}</th>
                    <th class="fw-bold">{{ l('ClientName') }}</th>
                    <th class="fw-bold text-end">{{ l('MaxOverdueDay') }}</th>
                    <th class="fw-bold text-end">{{ params.agingDays * 3 + 1 }}+</th>
                    <th class="fw-bold text-end">{{ params.agingDays * 2 + 1}} - {{ params.agingDays * 3 }}</th>
                    <th class="fw-bold text-end">{{ params.agingDays + 1 }} - {{ params.agingDays * 2 }}</th>
                    <th class="fw-bold text-end">1 - {{ params.agingDays }}</th>
                    <th class="fw-bold text-end">{{ 0 }}</th>
                    <th class="fw-bold text-end">{{ l('OsBalance') }}</th>
                    <th class="fw-bold text-end">{{ l('PrepaidAmount') }}</th>
                    <th class="fw-bold text-end">{{ l('NetOsBalance') }}</th>
                </tr>
                <tr>
                    <th colspan="8"></th>
                    <th class="text-end">{{ nativeCurrency }}</th>
                    <th class="text-end">{{ nativeCurrency }}</th>
                    <th class="text-end">{{ nativeCurrency }}</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                <template  v-for="item in items" :key="item.code">
                    <tr>
                        <td>{{ item.subSubjectCode }}</td>
                        <td>{{ item.companyName }}</td>
                        <td class="text-end">{{ item.overDays }}</td>
                        <td class="text-end">
                            {{ renderAmount(item.outstandingAmount - item.overdueAmount4 -
                                item.overdueAmount3 - item.overdueAmount2 - item.overdueAmount1)
                            }}
                        </td>
                        <td class="text-end">{{ renderAmount(item.overdueAmount4) }}</td>
                        <td class="text-end">{{ renderAmount(item.overdueAmount3) }}</td>
                        <td class="text-end">{{ renderAmount(item.overdueAmount2) }}</td>
                        <td class="text-end">{{ renderAmount(item.overdueAmount1) }}</td>
                        <td class="text-end">{{ renderAmount(item.outstandingAmount) }}</td>
                        <td class="text-end">{{ renderAmount(item.prepaidDeposit) }}</td>
                        <td class="text-end">{{ renderAmount(item.outstandingAmount - item.prepaidDeposit) }}</td>
                    </tr>
                </template>
            </tbody>
            <tfoot v-if="items.length > 0">
                <tr class="border-top text-end fw-bold">
                    <th colspan="3">{{ l('Total') }}</th>
                    <td>{{ renderAmount(total.overdueAmount5 )}}</td>
                    <td>{{ renderAmount(total.overdueAmount4) }}</td>
                    <td>{{ renderAmount(total.overdueAmount3) }}</td>
                    <td>{{ renderAmount(total.overdueAmount2) }}</td>
                    <td>{{ renderAmount(total.overdueAmount1) }}</td>
                    <td>{{ renderAmount(total.osBalance) }}</td>
                    <td>{{ renderAmount(total.prepaidAmount) }}</td>
                    <td>{{ renderAmount(total.netOsBalance) }}</td>
                 </tr>
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
                renderAmount,
                l,
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
                l,
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
         
    initCompanySelect(); 

    function search() {
        const params = getFormParams();
        reportStore.setParams(params);
        reportStore.setItems({ items: [] });
        const busyEle = '.body';
        abp.ui.setBusy(busyEle);
        accounting.finance.reports.receivableAgingReport.getAgingSummarySingleCurrencyList(params).then(function (result) {
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
});