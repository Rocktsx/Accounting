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
                group = {
                    code: code,
                    item: current,
                    items: [],
                    ...getTotalObject(),
                }
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
        accounting.finance.reports.profitAndLossReport.getTwelveMonthsList(params).then(function (result) {
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
        <h3 class="text-center">{{l('ProfitAndLoss')}}</h3>
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
                     <template  v-for="secondaryItem in item.items" :key="item.code">
                        <tr :key="item.code + 'name'" class="fw-bold">
                            <td colspan="4">{{ secondaryItem.item.secondaryGroupName }}</td>
                        </tr>
                        <tr v-for="subItem in secondaryItem.items" :key="secondaryItem.code">
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
                         <tr :key="item.code + 'subTotal'" class="border-top fw-bold" >
                            <td class="text-end" colspan="2">{{ l('SubTotal') }}</td>
                            <td v-for="field in fields" :key="field" class="text-end">
                                {{ renderAmount(secondaryItem[field] * secondaryItem.incomeExpenses) }}
                            </td>
                       </tr>
                    </template>
                    <tr v-if="item.code == 1" :key="item.code + 'total'" class="fw-bold" >
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
        data() {
            return {
                fields: Object.keys(getTotalObject())
            }
        },
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