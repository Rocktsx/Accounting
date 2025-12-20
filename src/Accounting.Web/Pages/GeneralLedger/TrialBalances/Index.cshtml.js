$(function () {
    const l = abp.localization.getResource('Accounting');

    function getFormParams() {
        return {
            endDate: $('#endDate').val(),
            periodId: $('#periodId').val(),
        }
    }

    function handleData(items) {
        const groups = [];
        const total = {
            debitor: 0,
            creditor: 0,
            count: items.length
        }
        const secondaryGroups = {}
        items.reduce((prev, current) => {
            const code = current.groupCode;
            let group = prev[code];
            if (!group) {
                group = {
                    code: code,
                    item: current,
                    items: [],
                }
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
                }
                secondaryGroups[secondaryCode] = secondaryGroup;
                group.items.push(secondaryGroup);
            } 
            secondaryGroup.items.push(current);

            if (current.nativeAmount > 0) {
                total.debitor += current.nativeAmount;
            } else {
                total.creditor += Math.abs(current.nativeAmount);
            }


            return prev;
        }, {});
         
        return {
            items: groups, total
        };
    }
    const store = new Vuex.Store({
        state() {
            return {
                items: [],
                params: getFormParams(),
                nativeCurrency: '',
                total: {
                    debitor: 0,
                    creditor: 0,
                    count: 0
                }
            }
        },
        mutations: {
            setItems(state, payload) {
                const { items } = payload;
                const result = handleData(items);
                state.items = result.items;
                state.total = result.total;
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

    accounting.finance.accountingSetting.getNativeCurrency()
        .then(result => store.commit('setNativeCurrency', result))
        .catch(() => { });

    $(document).on('click', '#searchBtn', function () {
        const params = getFormParams();
        store.commit('setParams', params);
        store.commit('setItems', { items: [] });
        const busyEle = '.body';
        abp.ui.setBusy(busyEle);
        accounting.finance.reports.trialBalanceReport.getYtdList(params).then(function (result) {
            store.commit('setItems', { items: result || [] });
            abp.ui.clearBusy(busyEle);
        }).catch(function () {
            abp.ui.clearBusy(busyEle);
        });
    });
    $(document).on('change', '#periodId', function (e) {
        const $this = $(this);
        const $option = $this.find('option:selected');
        const endDate = $option.attr('data-end-date');
        $('#endDate').val(endDate);
    });

    const headerTemplate = `
    <div class="header">
        <h3 class="text-center">{{l('TrialBalance')}}</h3>
        <div class="text-center">{{ l('AsAt') }}&nbsp;{{ formatDate(params.endDate) }}</div>
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
                <tr>
                    <th  colspan="4" class="text-end">{{ l('AsAt') }}&nbsp;{{ formatDate(params.endDate) }}</th>
                </tr>
                <tr class="border-bottom">
                    <th class="fw-bold">{{ l('SubjectCode') }}</th>
                    <th class="fw-bold">{{ l('SubjectName') }}</th>
                    <th class="fw-bold text-end">{{ l('Debitor') }}</th>
                    <th class="fw-bold text-end">{{ l('Creditor') }}</th>
                </tr>
                <tr>
                    <th></th>
                    <th></th>
                    <th class="text-end">{{ nativeCurrency }}</th>
                    <th class="text-end">{{ nativeCurrency }}</th>
                </tr>
            </thead>
            <tbody>
                <template  v-for="item in items" :key="item.code"> 
                     <tr :key="item.code + 'name'" class="fw-bold"> 
                        <td colspan="4">{{ item.item.groupName }}</td>
                     </tr>
                     <template  v-for="secondaryItem in item.items" :key="item.code">
                        <tr :key="item.code + 'name'">
                        <td colspan="4">{{ secondaryItem.item.secondaryGroupName }}</td>
                        </tr>
                        <tr v-for="subItem in secondaryItem.items" :key="secondaryItem.code">
                            <td>{{ subItem.subjectCode }}</td>
                            <td>{{ subItem.subjectName }}</td>
                            <td class="text-end">{{ subItem.nativeAmount >= 0 ? renderAmount(Math.abs(subItem.nativeAmount)): '' }}</td>
                            <td class="text-end">{{ subItem.nativeAmount < 0 ? renderAmount(Math.abs(subItem.nativeAmount)): '' }}</td>
                        </tr>
                    </template>
                    <tr :key="item.code + 'empty'">
                        <td colspan="4"></td>
                    </tr>
                </template>
            </tbody>
            <tfoot v-if="items.length > 0">
                  <tr class="border-top fw-bold" >
                        <td class="text-end">{{ l('Items') }}</td>
                        <td>
                            <div class="row">
                                <div class="col">{{ total.count  }}</div>
                                <div class="col text-end">{{ l('Total') }}</div>
                            </div>
                        </td>
                        <td class="text-end">{{ renderAmount(total.debitor) }}</td>
                        <td class="text-end">{{ renderAmount(total.creditor) }}</td>
                  </tr >
            </tfoot>
        </table>
    </div>`;


    const Body = {
        template: bodyTemplate,
        computed: {
            ...Vuex.mapGetters(['items', 'nativeCurrency', 'total', 'params'])
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