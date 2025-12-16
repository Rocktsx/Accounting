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
                group = {
                    code: code,
                    item: current,
                    items: [],
                    total: 0,
                    displayFactor: current.accountTypeGroup == accountTypeGroups.assets ? 1 : -1
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
        console.log("groups", groups, secondaryGroups)
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
                    netAssets: 0,
                }
            }
        },
        mutations: {
            setItems(state, payload) {
                const { items, total } = handleData(payload.items);
                state.items = items;
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
            total: (state) => state.total
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
        accounting.finance.generalLedgerReports.balanceSheetReport.getYtdList(params).then(function (result) {
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

    function formatDate(value) {
        let format = abp.localization.currentCulture.dateTimeFormat.shortDatePattern;
        format = format.replaceAll('d', 'D');
        return (new moment(new Date(value))).format(format)
    }

    const headerTemplate = `
    <div class="header">
        <h3 class="text-center">{{l('BalanceSheet')}}</h3>
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
                     <tr :key="item.code + 'name'" class="fw-bold">
                        <td colspan="4">{{ item.item.groupName }}</td>
                     </tr>
                     <template  v-for="secondaryItem in item.items" :key="item.code">
                        <tr :key="item.code + 'secname'">
                        <td colspan="3">{{ secondaryItem.item.secondaryGroupName }}</td>
                        </tr>
                        <tr v-for="subItem in secondaryItem.items" :key="secondaryItem.code">
                            <td>{{ subItem.subjectCode }}</td>
                            <td>{{ subItem.subjectName }}</td>
                            <td class="text-end">{{ renderAmount(subItem.nativeAmount * item.displayFactor) }}</td>
                        </tr>
                       <tr :key="item.code + 'subTotal'" class="border-top fw-bold" >
                            <td class="text-end" colspan="2">{{ l('SubTotal') }}</td>
                            <td class="text-end">
                                {{ renderAmount(secondaryItem.total) }}
                            </td>
                       </tr>
                    </template>
                    <tr :key="item.code + 'total'" class="fw-bold" >
                        <td class="text-end" colspan="2">{{ l('TotalText') }}&nbsp;{{ item.item.groupName }}</td>
                        <td class="text-end border-bottom">
                            {{ renderAmount(item.total) }}
                        </td>
                    </tr>
                    <tr v-if="item.item.accountTypeGroup == accountTypeGroups.liabilities" :key="item.code + 'nettotal'" class="fw-bold" >
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
        data() {
            return {
                accountTypeGroups
            }
        },
        computed: {
            ...Vuex.mapGetters(['items', 'nativeCurrency','total', 'params'])
        },
        methods: {
            formatDate,
            l,
            renderAmount(amount) {
                const num = Number(amount);
                return !Number.isNaN(num) ? num.toFixed(2) : '0.00';
            },
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