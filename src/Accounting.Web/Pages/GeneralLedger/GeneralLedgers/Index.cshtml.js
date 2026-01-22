$(function () {
    const l = abp.localization.getResource('Accounting');

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
            endDate: $('#endDate').val(),
            periodId: $('#periodId').val(),
        }
    }
    const currentPeriodSortOrder = 3;

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
                    currentPeriodBalance: 0,
                    currentPeriodVouchers: 0,
                });
                prev[code] = group;
                groups.push(group);
            }
            group.items.push(current);

            if (current.nativeAmount > 0) {
                group.debitor += current.nativeAmount;
            } else {
                group.creditor += Math.abs(current.nativeAmount);
            }

            if (current.sortOrder == currentPeriodSortOrder) {
                group.currentPeriodBalance += current.nativeAmount;
                group.currentPeriodVouchers += 1;
            }

            group.balance += current.nativeAmount;
            current.balance = group.balance;

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
        <h3 class="text-center">{{l('GeneralLedger')}}</h3>
        <div class="text-center">{{ formatDate(params.startDate) }} - {{ formatDate(params.endDate) }}</div>
    </div>`;

    const Header = {
        template: headerTemplate,
        setup() {
            const reportStore = useReportStore();
            const { params } = storeToRefs(reportStore);
            return {
                params,
                l,
                formatDate
            }
        }
    }

    const bodyTemplate = `
    <div class="body"> 
        <table class="table table-borderless">
            <thead>
                <tr class="border-bottom">
                    <th class="fw-bold">{{ l('VoucherDate') }}</th>
                    <th class="fw-bold">{{ l('VoucherCode') }}</th>
                    <th class="fw-bold">{{ l('Description') }}</th>
                    <th class="fw-bold text-end">{{ l('Debitor') }}</th>
                    <th class="fw-bold text-end">{{ l('Creditor') }}</th>
                    <th class="fw-bold text-end">{{ l('Balance') }}</th>
                    <th> </th>
                </tr>
                <tr>
                    <th></th>
                    <th></th>
                    <th></th>
                    <th class="text-end">{{ nativeCurrency }}</th>
                    <th class="text-end">{{ nativeCurrency }}</th>
                    <th class="text-end">{{ nativeCurrency }}</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                <template  v-for="item in items" :key="item.code">
                    <tr class="fw-bold">
                        <td>{{ l('SubjectCode') }}</td>
                        <td colspan="6">{{ item.item.subjectCode }}</td>
                    </tr>
                     <tr class="fw-bold">
                        <td>{{ l('SubjectName') }}</td>
                        <td colspan="6">{{ item.item.subjectName }}</td>
                    </tr>
                    <tr v-for="subItem in item.items" :key="subItem.voucherCode">
                        <td>{{ formatDate(subItem.voucherDate) }}</td>
                        <td>{{ subItem.voucherCode }}</td>
                        <td>{{ subItem.description }}<div v-if="subItem.docNo">{{ subItem.docNo }}</div></td>
                        <td class="text-end">{{ subItem.nativeAmount >= 0 ? renderAmount(Math.abs(subItem.nativeAmount)): '' }}</td>
                        <td class="text-end">{{ subItem.nativeAmount < 0 ? renderAmount(Math.abs(subItem.nativeAmount)): '' }}</td>
                        <td class="text-end">{{ renderAmount(Math.abs(subItem.balance)) }}</td>
                        <td>{{ subItem.balance >= 0 ? 'DR': 'CR' }}</td>
                    </tr>
                    <tr class="border-top fw-bold">
                        <td class="text-end">{{ l('CurrentVouchers') }}</td>
                        <td>{{ item.currentPeriodVouchers  }}</td>
                        <td class="text-end">{{ l('Total') }}</td>
                        <td class="text-end">{{ renderAmount(item.debitor) }}</td>
                        <td class="text-end">{{ renderAmount(item.creditor) }}</td>
                        <td class="text-end">{{ renderAmount(Math.abs(item.balance)) }}</td>
                        <td>{{ item.balance >= 0 ? 'DR': 'CR' }}</td>
                    </tr>
                    <tr class="fw-bold">
                        <td colspan="3" class="text-end">{{ l('PeriodMovement') }}</td>
                        <td :class="{'border-bottom': item.currentPeriodBalance >= 0 }"  class="text-end">{{ item.currentPeriodBalance >= 0 ? renderAmount(item.currentPeriodBalance): '' }}</td>
                        <td :class="{'border-bottom': item.currentPeriodBalance < 0 }"  class="text-end">{{ item.currentPeriodBalance < 0 ? renderAmount(Math.abs(item.currentPeriodBalance)): '' }}</td>
                        <td></td>
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
        accounting.finance.reports.generalLedgerReport.getSingleCurrencyList(params).then(function (result) {
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
});