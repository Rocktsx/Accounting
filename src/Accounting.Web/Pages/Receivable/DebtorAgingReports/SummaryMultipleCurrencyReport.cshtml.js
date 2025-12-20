$(function () {
    const l = abp.localization.getResource('Accounting');
    
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
        total.overdueAmount3 += item.overdueAmount3;
        total.overdueAmount4 += item.overdueAmount4;
        total.overdueAmount5 += item.outstandingAmount - item.overdueAmount4 -
            item.overdueAmount3 - item.overdueAmount2 - item.overdueAmount1;
    }
    function handleData(items) {
        const total = getTotal();
        
        const groups = [];
        items.reduce((prev, current) => {
            const code = current.subSubjectCode;
            let group = prev[code];
            if (!group) {
                group = {
                    code: code,
                    item: current,
                    items: [],
                    ...getTotal()
                }
                prev[code] = group;
                groups.push(group);
            }
            group.items.push(current);
            setTotal(current, group);
            setTotal(current, total);

            return prev;
        }, {});
        return { groups, total };
    }

    const store = new Vuex.Store({
        state() {
            return {
                items: [],
                params: getFormParams(),
                nativeCurrency: '',
                total: getTotal()
            }
        },
        mutations: {
            setItems(state, payload) {
                const { items } = payload; 
                const { groups, total } = handleData(items);
                state.items = groups;
                state.total = total;
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
            total: (state) => state.total,
        }
    })

    initCompanySelect();
    accounting.finance.accountingSetting.getNativeCurrency()
        .then(result => store.commit('setNativeCurrency', result))
        .catch(() => { });

    $(document).on('click', '#searchBtn', function () {
        const params = getFormParams();
        store.commit('setParams', params);
        store.commit('setItems', { items: [] });
        const busyEle = '.body';
        abp.ui.setBusy(busyEle);
        accounting.finance.reports.receivableAgingReport.getAgingSummaryMultipleCurrencyList(params).then(function (result) {
            store.commit('setItems', { items: result || [] });
            abp.ui.clearBusy(busyEle);
        }).catch(function () {
            abp.ui.clearBusy(busyEle);
        });
    });

    const headerTemplate = `
    <div class="header">
        <h3 class="text-center">{{l('ReceivableAgingReport')}}</h3>
    </div>`;

    const Header = {
        template: headerTemplate,
        methods: { 
            l
        }
    }

    const bodyTemplate = `
    <div class="body"> 
        <table class="table table-borderless">
            <thead>
                <tr>
                    <th colspan="4"></th>
                   <th colspan="5" class="fw-bold border-bottom text-center">{{ l('OverdueDays') }}</th>
                   <th colspan="3"></th>
                </tr>
                <tr class="border-bottom">
                    <th class="fw-bold">{{ l('ClientCode') }}</th>
                    <th class="fw-bold">{{ l('ClientName') }}</th>
                    <th class="fw-bold text-end">{{ l('MaxOverdueDay') }}</th>
                    <th class="fw-bold">{{ l('Currency') }}</th>
                    <th class="fw-bold text-end">{{ params.agingDays * 3 + 1 }}+</th>
                    <th class="fw-bold text-end">{{ params.agingDays * 2 + 1}} - {{ params.agingDays * 3 }}</th>
                    <th class="fw-bold text-end">{{ params.agingDays + 1 }} - {{ params.agingDays * 2 }}</th>
                    <th class="fw-bold text-end">1 - {{ params.agingDays }}</th>
                    <th class="fw-bold text-end">{{ 0 }}</th>
                    <th class="fw-bold text-end">{{ l('OsBalance') }}</th>
                    <th class="fw-bold text-end">{{ l('PrepaidAmount') }}</th>
                    <th class="fw-bold text-end">{{ l('NetOsBalance') }}</th>
                </tr>
            </thead>
            <tbody>
                <template  v-for="item in items" :key="item.code">
                     <template  v-for="(subItem, index) in item.items" :key="item.subSubjectCode">
                        <tr :key="subItem.subSubjectCode">
                            <td>{{ index == 0 ? subItem.subSubjectCode : '' }}</td>
                            <td>{{ index == 0 ? subItem.companyName : '' }}</td>
                            <td class="text-end">{{ subItem.overDays }}</td>
                            <td>{{ subItem.currencyCode }}</td>
                            <td class="text-end">
                                {{ renderAmount(subItem.foreignOutstandingAmount - subItem.foreignOverdueAmount4 -
                                    subItem.foreignOverdueAmount3 - subItem.foreignOverdueAmount2 - subItem.foreignOverdueAmount1)
                                }}
                            </td>
                            <td class="text-end">{{ renderAmount(subItem.foreignOverdueAmount4) }}</td>
                            <td class="text-end">{{ renderAmount(subItem.foreignOverdueAmount3) }}</td>
                            <td class="text-end">{{ renderAmount(subItem.foreignOverdueAmount2) }}</td>
                            <td class="text-end">{{ renderAmount(subItem.foreignOverdueAmount1) }}</td>
                            <td class="text-end">{{ renderAmount(subItem.foreignOutstandingAmount) }}</td>
                            <td class="text-end">{{ renderAmount(subItem.foreignPrepaidDeposit) }}</td>
                            <td class="text-end">{{ renderAmount(subItem.foreignOutstandingAmount - subItem.foreignPrepaidDeposit) }}</td>
                        </tr>
                     </template>
                      <tr class="border-top text-end fw-bold">
                            <th colspan="3">{{ l('SubTotal') }}</th>
                            <th>{{ nativeCurrency }}</th>
                            <td>{{ renderAmount(item.overdueAmount5 )}}</td>
                            <td>{{ renderAmount(item.overdueAmount4) }}</td>
                            <td>{{ renderAmount(item.overdueAmount3) }}</td>
                            <td>{{ renderAmount(item.overdueAmount2) }}</td>
                            <td>{{ renderAmount(item.overdueAmount1) }}</td>
                            <td>{{ renderAmount(item.osBalance) }}</td>
                            <td>{{ renderAmount(item.prepaidAmount) }}</td>
                            <td>{{ renderAmount(item.netOsBalance) }}</td>
                     </tr>
                </template>
            </tbody>
            <tfoot v-if="items.length > 0">
                <tr class="border-top text-end fw-bold">
                    <th colspan="3">{{ l('Total') }}</th>
                    <th>{{ nativeCurrency }}</th>
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
        computed: {
            ...Vuex.mapGetters(['items', 'nativeCurrency', 'params', 'total'])
        },
        methods: {
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