$(function () {
    const l = abp.localization.getResource('Accounting');
    const { createApp, ref, markRaw, computed } = Vue;
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
    function setTotal(item, total) { 
        total.netOsBalance += item.outstandingAmount - item.prepaidDeposit;
        total.osBalance += item.outstandingAmount;
        total.prepaidAmount += item.prepaidDeposit;
        total.overdueAmount1 += item.overdueAmount1;
        total.overdueAmount2 += item.overdueAmount2;
        total.overdueAmount3 += item.overdueAmount3
        total.overdueAmount4 += item.overdueAmount4;
        total.overdueAmount5 += item.outstandingAmount - item.overdueAmount4 -
            item.overdueAmount3 - item.overdueAmount2 - item.overdueAmount1;
    }
    function handleData(items, params) {
        const total = getTotal();

        const groups = [];
        items.reduce((prev, current) => {
            const code = current.subSubjectCode;
            let group = prev[code];
            if (!group) {
                group = markRaw({
                    code: code,
                    item: current,
                    items: [],
                    ...getTotal()
                });
                prev[code] = group;
                groups.push(group);
            }
            group.items.push(current);
            current.overDays = !current.dueDate ? 0 : Math.max(0, Math.floor((new Date(params.endDate) - new Date(current.dueDate)) / (1000 * 60 * 60 * 24)));
            current.overdueAmount4 = current.overDays > params.agingDays * 2 && current.overDays <= params.agingDays * 3 ? current.outstandingAmount : 0;
            current.overdueAmount3 = current.overDays > params.agingDays && current.overDays <= params.agingDays * 2 ? current.outstandingAmount : 0;
            current.overdueAmount2 = current.overDays > 0 && current.overDays <= params.agingDays ? current.outstandingAmount : 0;
            current.overdueAmount1 = current.overDays <= 0 ? current.outstandingAmount : 0;

            setTotal(current, group);
            setTotal(current, total);

            return prev;
        }, {});
        return { groups, total };
    }

    const useReportStore = defineStore('report', () => {
        const params = ref({})
        const items = ref([])
        const total = ref(getTotal())
        const nativeCurrency = ref('')
        const setItems = (payload) => {
            const result = handleData(payload.items, params.value);
            items.value = result.groups;
            total.value = result.total;
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
                    <th colspan="6"></th>
                   <th colspan="5" class="fw-bold border-bottom text-center">{{ l('OverdueDays') }}</th>
                   <th colspan="2"></th>
                </tr>
                <tr class="border-bottom">
                    <th class="fw-bold">{{ l('VoucherDate') }}</th>
                    <th class="fw-bold">{{ l('VoucherCode') }}</th>
                    <th class="fw-bold">{{ l('DocNo') }}</th>
                    <th class="fw-bold text-end">{{ l('DueDate') }}</th>
                    <th class="fw-bold">{{ l('Currency') }}</th>
                    <th class="fw-bold text-end">{{ l('ForeignAmount') }}</th>
                    <th class="fw-bold text-end">{{ params.agingDays * 3 + 1 }}+</th>
                    <th class="fw-bold text-end">{{ params.agingDays * 2 + 1}} - {{ params.agingDays * 3 }}</th>
                    <th class="fw-bold text-end">{{ params.agingDays + 1 }} - {{ params.agingDays * 2 }}</th>
                    <th class="fw-bold text-end">1 - {{ params.agingDays }}</th>
                    <th class="fw-bold text-end">{{ 0 }}</th>
                    <th class="fw-bold text-end">{{ l('PrepaidAmount') }}</th>
                    <th class="fw-bold text-end">{{ l('NetOsBalance') }}</th>
                </tr>
               <tr>
                    <th colspan="6"></th>
                    <th class="fw-bold text-end">{{ nativeCurrency }}</th>
                    <th class="fw-bold text-end">{{ nativeCurrency}}</th>
                    <th class="fw-bold text-end">{{ nativeCurrency }}</th>
                    <th class="fw-bold text-end">{{ nativeCurrency }}</th>
                    <th class="fw-bold text-end">{{ nativeCurrency }}</th>
                    <th class="fw-bold text-end">{{ nativeCurrency }}</th>
                    <th class="fw-bold text-end">{{ nativeCurrency }}</th>
                </tr>
            </thead>
            <tbody>
                <template  v-for="item in items" :key="item.code">
                       <tr class="fw-bold">
                            <td colspan="12">{{ item.item.subSubjectCode }} {{ item.item.companyName }}</td>
                      </tr>
                     <template  v-for="(subItem, index) in item.items" :key="item.docNo">
                        <tr>
                            <td>{{ subItem.voucherDate }}</td>
                            <td>{{ subItem.voucherCode }}</td>
                            <td>{{ subItem.docNo }}</td>
                            <td class="text-end">{{ subItem.dueDate ? formatDate(subItem.dueDate) : '' }}</td>
                            <td>{{ subItem.currencyCode }}</td>
                            <td class="text-end">{{ renderAmount(subItem.foreignAmount) }}</td>
                            <td class="text-end">
                                {{ renderAmount(subItem.foreignOutstandingAmount - subItem.foreignOverdueAmount4 -
                                    subItem.foreignOverdueAmount3 - subItem.foreignOverdueAmount2 - subItem.foreignOverdueAmount1)
                                }}
                            </td>
                            <td class="text-end">{{ renderAmount(subItem.foreignOverdueAmount4) }}</td>
                            <td class="text-end">{{ renderAmount(subItem.foreignOverdueAmount3) }}</td>
                            <td class="text-end">{{ renderAmount(subItem.foreignOverdueAmount2) }}</td>
                            <td class="text-end">{{ renderAmount(subItem.foreignOverdueAmount1) }}</td>
                            <td class="text-end">{{ renderAmount(subItem.prepaidDeposit) }}</td>
                            <td class="text-end">{{ renderAmount(subItem.outstandingAmount - subItem.prepaidDeposit) }}</td>
                        </tr>
                     </template>
                        <tr class="border-top text-end fw-bold">
                            <td colspan="6">{{ l('SubTotal') }}</td>
                            <td>{{ renderAmount(item.overdueAmount5 )}}</td>
                            <td>{{ renderAmount(item.overdueAmount4) }}</td>
                            <td>{{ renderAmount(item.overdueAmount3) }}</td>
                            <td>{{ renderAmount(item.overdueAmount2) }}</td>
                            <td>{{ renderAmount(item.overdueAmount1) }}</td>
                            <td>{{ l('OsBalance') }}</td>
                            <td>{{ renderAmount(item.osBalance) }}</td>
                       </tr>
                       <tr class="text-end fw-bold"> 
                            <td colspan="12">{{ l('PrepaidAmount') }}</td>
                            <td>{{ renderAmount(item.prepaidAmount) }}</td>
                       </tr>
                       <tr class="text-end fw-bold">
                            <td colspan="12">{{ l('NetOsBalance') }}</td>
                            <td>{{ renderAmount(item.netOsBalance) }}</td>
                       </tr>
                </template>
            </tbody>
            <tfoot v-if="items.length > 0">
                <tr class="border-top text-end fw-bold">
                    <td colspan="6">{{ l('Total') }}</td>
                    <td>{{ renderAmount(total.overdueAmount5 )}}</td>
                    <td>{{ renderAmount(total.overdueAmount4) }}</td>
                    <td>{{ renderAmount(total.overdueAmount3) }}</td>
                    <td>{{ renderAmount(total.overdueAmount2) }}</td>
                    <td>{{ renderAmount(total.overdueAmount1) }}</td>
                    <td>{{ l('OsBalance') }}</td>
                    <td>{{ renderAmount(total.osBalance) }}</td>
                </tr>
                <tr class="text-end fw-bold">
                    <td colspan="12">{{ l('PrepaidAmount') }}</td>
                    <td>{{ renderAmount(total.prepaidAmount) }}</td>
                </tr>
                <tr class="text-end fw-bold">
                    <td colspan="12">{{ l('NetOsBalance') }}</td>
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
                formatDate,
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

    function search() {
        const params = getFormParams();
        reportStore.setParams(params);
        reportStore.setItems({ items: [] });
        const busyEle = '.body';
        abp.ui.setBusy(busyEle);
        accounting.finance.reports.receivableAgingReport.getAgingDetailList(params).then(function (result) {
            reportStore.setItems({ items: result || [] });
            abp.ui.clearBusy(busyEle);
        }).catch(function () {
            abp.ui.clearBusy(busyEle);
        });
    }

    search();
    initCompanySelect();

    $(document).on('click', '#searchBtn', function () {
        search();
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