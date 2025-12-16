$(function () {
    const l = abp.localization.getResource('Accounting');

    function getFormParams() {
        return {
            voucherType: $('#voucherType').val(),
            startDate: $('#startDate').val(),
            endDate: $('#endDate').val(),
            prefix: $('#prefix').val(),
            startNo: $('#startNo').val(),
            endNo: $('#endNo').val(),
        }
    }
    const debitCredit = {
        debitor: 1,
        creditor: -1
    }
    function handleData(items) {
        const groups = [];
        items.reduce((prev, current) => {
            const code = current.voucherCode;
            let group = prev[code];
            if (!group) {
                group = {
                    code: code,
                    item: current,
                    items: [],
                    debitor: 0,
                    creditor: 0,
                }
                prev[code] = group;
                groups.push(group);
            }
            group.items.push(current);

            if (current.debitorCreditor == debitCredit.debitor) {
                group.debitor += current.nativeAmount;
            } else {
                group.creditor += Math.abs(current.nativeAmount);
            }

            return prev;
        }, {});
        return groups;
    }
    const store = new Vuex.Store({
        state() {
            return {
                items: [],
                params: getFormParams(),
                nativeCurrency: ''
            }
        },
        mutations: {
            setItems(state, payload) {
                const { items } = payload;
                state.items = handleData(items);
            },
            setParams(state, payload) {
                state.params = payload;
            },
            setNativeCurrency(state, payload) {
                state.nativeCurrency = payload;
            },
        },
        getters: {
            items: (state) => state.items,
            params: (state) => state.params,
            nativeCurrency: (state) => state.nativeCurrency,
        }
    })


    accounting.finance.accountingSetting.getNativeCurrency()
        .then(result => store.commit('setNativeCurrency', result))

    $(document).on('click', '#searchBtn', function () {
        const params = getFormParams();
        store.commit('setParams', params);
        store.commit('setItems', { items: [] });
        const busyEle = '.body';
        abp.ui.setBusy(busyEle);
        accounting.finance.generalLedgerReports.journalReport.getSingleCurrencySortByDateList(params).then(function (result) {
            store.commit('setItems', { items: result || [] });
            abp.ui.clearBusy(busyEle);
        }).catch(function () {
            abp.ui.clearBusy(busyEle);
        });
    });

    const headerTemplate = `
    <div class="header">
        <h3 class="text-center">{{l('JournalReport')}}</h3>
        <div class="text-center">{{ formatDate(params.startDate) }} - {{ formatDate(params.endDate) }}</div>
    </div>`;

    const Header = {
        template: headerTemplate,
        computed: {
            ...Vuex.mapGetters(['params'])
        },
        methods: {
            formatDate,
            l
        }
    }

    const bodyTemplate = `
    <div class="body"> 
        <table class="table table-borderless">
            <thead>
                <tr class="border-bottom">
                    <th class="fw-bold">{{ l('VoucherCode') }}</th>
                    <th class="fw-bold">{{ l('VoucherDate') }}</th>
                    <th class="fw-bold">{{ l('Subject') }}</th>
                    <th class="fw-bold">{{ l('Description') }}</th>
                    <th class="fw-bold text-end">{{ l('Debitor') }}</th>
                    <th class="fw-bold text-end">{{ l('Creditor') }}</th>
                </tr>
                <tr>
                    <th colspan="4"></th>
                    <th class="text-end">{{ nativeCurrency }}</th>
                    <th class="text-end">{{ nativeCurrency }}</th>
                </tr>
            </thead>
            <tbody>
                <template  v-for="item in items" :key="item.code"> 
                    <tr v-for="(subItem, index) in item.items" :key="subItem.voucherCode + index">
                        <td>{{ index == 0 ? subItem.voucherCode  : '' }}</td>
                        <td>{{ index == 0 ? formatDate(subItem.voucherDate) : ''}}</td>
                        <td  v-if="index == 0" :rowspan="item.items.length">
                            <div>  {{ subItem.subjectCode}} </div>
                            <div>{{ subItem.subjectName}} </div>
                        </td>
                        <td>{{ subItem.description }}<div v-if="subItem.docNo">{{ subItem.docNo }}</div></td>
                        <td class="text-end">{{ subItem.debitorCreditor == debitCredit.debitor ? renderAmount(subItem.nativeAmount): '' }}</td>
                        <td class="text-end">{{ subItem.debitorCreditor == debitCredit.creditor ? renderAmount(subItem.nativeAmount): '' }}</td>
                    </tr>
                    <tr :key="item.code + 'total'" class="border-top fw-bold"> 
                        <td class="text-end" colspan="4">{{ l('SubTotal') }}</td>
                        <td class="text-end">{{ renderAmount(item.debitor) }}</td>
                        <td class="text-end">{{ renderAmount(item.creditor) }}</td>
                    </tr>
                </template>
            </tbody>
        </table>
    </div>`;

    const Body = {
        template: bodyTemplate,
        data() {
            return {
                debitCredit
            }
        },
        computed: {
            ...Vuex.mapGetters(['items', 'nativeCurrency'])
        },
        methods: {
            formatDate,
            l,
            renderAmount,
        }
    }

    const reportTemplate = `
 <div class="report">
    <Header />
    <Body/>
    <div v-if="items.length == 0" class="text-center">{{ l('NoDataAvailable') }}</div>
 </div>`;

    const Report = {
        components: { Header, Body },
        template: reportTemplate,
        computed: {
            ...Vuex.mapGetters(['items'])
        },
        methods: {
            l
        }
    }

    const app = new Vue({
        components: { Report },
        template: `<Report />`,
        el: '#app',
        store
    });
});