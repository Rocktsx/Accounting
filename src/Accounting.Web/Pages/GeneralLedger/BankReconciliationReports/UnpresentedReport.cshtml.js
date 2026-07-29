$(function () {
    const l = abp.localization.getResource('Accounting');
    const accountTypeTypes = {
        bank: 1
    };
    const debitorCreditor = {
        debitor: 1,
        creditor: -1
    }
    function initSubjectSelect() {
        const $subjectId = $('#subjectId');
        const language = getSelect2Language();
        $subjectId.attr('data-language', language);
        $subjectId.select2({
            ajax: {
                url: '/api/app/subject',
                delay: 250,
                dataType: "json",
                data: function (params) {
                    return {
                        filter: params.term || '',
                        maxResultCount: 10,
                        accountTypeCategory: accountTypeTypes.bank
                    };
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
            placeholder: '',
            allowClear: true,
            language: language
        });
    }
    function getFormParams() {
        return {
            subjectId: $('#subjectId').val(),
            startDate: $('#startDate').val(),
            endDate: $('#endDate').val()
        }
    }
    const { createApp, ref, markRaw, computed } = Vue;
    const { defineStore, storeToRefs, createPinia } = Pinia;

    function handleData(items) {
        const groups = [];
        items.reduce((prev, current) => {
            const code = current.subjectCode;
            let group = prev[code];
            if (!group) {
                group = markRaw({
                    code: code,
                    item: current,
                    items: [],
                    balance: 0,
                    debitor: 0,
                    creditor: 0,
                    count: 0
                });
                prev[code] = group;
                groups.push(group);
            }
            group.items.push(current);

            if (current.debitorCreditor == debitorCreditor.debitor) {
                group.debitor += current.nativeAmount;
            } else {
                group.creditor += Math.abs(current.nativeAmount);
            }

            group.count++;

            return prev;
        }, {});
        return groups;
    }

    const useReportStore = defineStore('report', () => {
        const params = ref({})
        const items = ref([])
        const nativeCurrency = ref('')

        const count = computed(() => items.value.length);

        const setItems = (payload) => {
            items.value = handleData(payload.items);
        }
        const setParams = (payload) => {
            params.value = payload;
        }
        const setNativeCurrency = (payload) => {
            nativeCurrency.value = payload;
        }

        return { items, params, nativeCurrency, count, setItems, setParams, setNativeCurrency }
    });

    const headerTemplate = `
    <div class="header">
        <h3 class="text-center">{{l('BankReconciliation')}}</h3>
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
                    <th colspan="2" class="fw-bold text-center border-bottom">{{ l('Amount') }}</th>
                </tr>
                <tr class="border-bottom">
                    <th class="fw-bold">{{ l('VoucherDate') }}</th>
                    <th class="fw-bold">{{ l('VoucherCode') }}</th>
                    <th class="fw-bold">{{ l('Description') }}</th>
                    <th class="fw-bold text-end">{{ l('Debitor') }}</th>
                    <th class="fw-bold text-end">{{ l('Creditor') }}</th>
                </tr>
                <tr>
                    <th colspan="3"></th>
                    <th class="text-end">{{ nativeCurrency }}</th>
                    <th class="text-end">{{ nativeCurrency }}</th>
                </tr>
            </thead>
            <tbody>
                <template  v-for="item in items" :key="item.code">
                    <tr :key="item.code + 'code'" class="fw-bold">
                        <td>{{ l('SubjectName') }}</td>
                        <td colspan="4">
                            {{ item.item.subjectName + l('RoundBracketLeft')+ item.item.subjectCode + l('RoundBracketRight') }}
                        </td>
                    </tr> 
                    <tr v-for="subItem in item.items" :key="subItem.voucherCode">
                        <td>{{ formatDate(subItem.voucherDate) }}</td>
                        <td>{{ subItem.voucherCode }}</td>
                        <td>{{ subItem.description }}</td>
                        <td class="text-end">{{ subItem.debitorCreditor == debitorCreditor.debitor ? renderAmount(Math.abs(subItem.nativeAmount)): '' }}</td>
                        <td class="text-end">{{ subItem.debitorCreditor == debitorCreditor.creditor ? renderAmount(Math.abs(subItem.nativeAmount)): '' }}</td>
                    </tr>
                    <tr :key="item.code + 'total'" class="border-top fw-bold">
                        <td class="text-end">{{ l('Items') }}</td>
                        <td>{{ item.count  }}</td>
                        <td class="text-end">{{ l('Total') }}</td>
                        <td class="text-end border-bottom">{{ renderAmount(item.debitor) }}</td>
                        <td class="text-end border-bottom">{{ renderAmount(item.creditor) }}</td>
                    </tr>
                </template>
            </tbody>
        </table>
    </div>`;

    const Body = {
        template: bodyTemplate,
        setup() {
            const reportStore = useReportStore();
            const { items, nativeCurrency } = storeToRefs(reportStore);
            return {
                items,
                nativeCurrency,
                formatDate,
                l,
                renderAmount,
                debitorCreditor
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
      
    initSubjectSelect();

    accounting.finance.accountingSetting.getNativeCurrency()
        .then(result => reportStore.setNativeCurrency(result))
        .catch(() => { });

    function search() {
        const params = getFormParams();
        reportStore.setParams(params);
        reportStore.setItems({ items: [] });
        const busyEle = '.body';
        abp.ui.setBusy(busyEle);
        accounting.finance.reports.bankReconciliationReport.getUnpresentedList(params).then(function (result) {
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