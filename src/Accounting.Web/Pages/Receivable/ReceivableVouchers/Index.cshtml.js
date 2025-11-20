
$(function () {
    const l = abp.localization.getResource('Accounting');
    const isGrantedEdit = abp.auth.isGranted('Accounting.Receivable.ReceivableVoucher.Edit');
    const isGrantedDelete = abp.auth.isGranted('Accounting.Receivable.ReceivableVoucher.Deletion');
    const voucherRequests = accounting.finance.receivableVoucher;
    const debitCredit = {
        debitor: 1,
        creditor: -1
    }
    const accountTypes = {
        receivable: 2,
        payable: 3
    }
    function formatDate(value) {
        return (new moment(value)).format("yyyy-MM-DD")
    }

    const renderAmount = (amount, scale) => {
        const num = Number(amount);
        return !Number.isNaN(num) ? num.toFixed(scale ? scale : 2) : '0.00';
    }

    const emptyReceipts = { items: [], totalCount: 0, currentPage: 0 }
    const getRowId = (function () {
        let rowId = 0;
        return function () {
            return rowId++;
        }
    })();
    const getPaymentItem = (currency) => ({
        subjectId: '',
        currencyCode: currency || '',
        currencyRate: renderAmount(1, 7),
        foreignAmount: 0,
        nativeAmount: 0,
        debitorCreditor: 1,
        paymentReference: '',
        isSelected: false,
        rowid: getRowId()
    });
    const setSubjects = (state, subjects) => {
        subjects.forEach(item => {
            if (!state.subjectMap[item.id]) {
                state.subjectMap[item.id] = item;
                state.subjects.push(item);
            }
        });
    }

    // 创建一个新的 store 实例 
    const store = new Vuex.Store({
        state() {
            return {
                editItem: {
                    details: [],
                    creditorId: ''
                },
                isShowModal: false,
                subjects: [],
                companies: [],
                subjectMap: {},
                companyMap: {},
                clients: [],
                vendors: [],
                clientMap: {},
                vendorMap: {},
                currencies: [],
                nativeCurrency: '',
                isRequestData: false,
                payments: [],
                receipts: emptyReceipts,
                paymentMethods: [],
            }
        },
        mutations: {
            increment(state) {
                state.count++
            },
            showModal(state, payload) {
                state.isShowModal = payload.isShowModal;
            },
            setIsEdit(state, payload) {
                state.isEdit = payload.isEdit;
            },
            setEditItem(state, payload) {
                const { details, ...others } = payload.item || { details: [] };
                let creditorId = '';
                const newDetails = details.map(item => {
                    if (item.subSubjectCode && !creditorId) {
                        creditorId = item.subSubjectCode;
                    }
                    return {
                        ...item,
                        subjectName: '',
                        subSubjectName: '',
                        isSubSubjectType: false,
                        accountTypeCategory: 0,
                        rowid: getRowId()
                    };
                });
                state.editItem = { ...others, details: newDetails, creditorId };
                state.editItem.voucherDate = formatDate(state.editItem.voucherDate);
                state.payments = [];
                state.receipts = emptyReceipts;
            },
            setSubjects(state, payload) {
                const { subjects } = payload;
                setSubjects(state, subjects || []);
            },
            setDetails(state) {
                state.payments = [];
                (state.editItem.details || []).forEach(item => {
                    const subject = state.subjectMap[item.subjectId];
                    if (subject) {
                        const { code, name, isSubSubjectType, accountType } = subject;
                        const category = accountType ? accountType.category : 0;
                        item.isSubSubjectType = isSubSubjectType;
                        item.accountTypeCategory = category;
                        item.subjectName = code + ' - ' + name;
                        if (category != accountTypes.receivable && category != accountTypes.payable) {
                            state.payments.push({ ...item });
                        }
                    }
                    const company = state.companyMap[item.subSubjectCode];
                    if (company) {
                        const { code, name, } = company;
                        item.subSubjectName = code + ' - ' + name;
                    }
                });

            },
            setCompanies(state, payload) {
                const { items } = payload;
                (items || []).forEach(c => {
                    state.companyMap[c.id] = c;
                    if (!state.clientMap[c.id] && c.isClient) {
                        state.clientMap[c.id] = c;
                        state.clients.push(c)
                    }
                    if (!state.vendorMap[c.id] && c.isVender) {
                        state.vendorMap[c.id] = c;
                        state.vendors.push(c)
                    }
                });
            },
            setCurrencies(state, payload) {
                state.currencies = payload.currencies || [];
            },
            setNativeCurrency(state, payload) {
                state.nativeCurrency = payload;
            },
            setIsRequestData(state, payload) {
                state.isRequestData = payload;
            },
            saveDetailItem(state, payload) {
                const { item } = payload;
                const index = state.editItem.details.findIndex(obj => obj == item);
                if (index < 0) {
                    state.editItem.details.push(item)
                } else {
                    state.editItem.details = [...state.editItem.details];
                }
            },
            removeDetailItem(state, payload) {
                const index = (state.editItem.details || []).findIndex(obj => obj === payload.item);
                if (index >= 0) {
                    state.editItem.details.splice(index, 1);
                }
            },
            setReceipts(state, payload) {
                state.receipts = payload || emptyReceipts;
            },
            addPaymentItem(state) {
                state.payments.push(getPaymentItem(state.nativeCurrency));
            },
            setPayments(state, payload) {
                state.payments = (payload || []).map(item => ({ ...item, rowid: getRowId() }));
            },
            removePaymentItem(state, payload) {
                const index = state.payments.findIndex(obj => obj === payload.item);
                if (index >= 0) {
                    state.payments.splice(index, 1);
                }
            },
            setPaymentMethods(state, payload) {
                state.paymentMethods = payload || [];
                setSubjects(state, state.paymentMethods);
            }
        },
        getters: {
            isShowModal: state => state.isShowModal,
            editItem: state => state.editItem,
            totalDebitorAmount: state => {
                return (state.editItem.details || []).reduce((init, item) =>
                    init + (item.debitorCreditor === debitCredit.debitor ?
                        Number(item.nativeAmount) : 0), 0)
            },
            totalCreditorAmount: state => {
                return (state.editItem.details || []).reduce((init, item) =>
                    init + (item.debitorCreditor === debitCredit.creditor ?
                        Number(item.nativeAmount) : 0), 0)
            },
            subjects: state => state.subjects,
            subjectMap: state => state.subjectMap,
            companies: state => state.companies,
            companyMap: state => state.companyMap,
            clients: state => state.clients,
            vendors: state => state.vendors,
            currencies: state => state.currencies,
            nativeCurrency: state => state.nativeCurrency,
            isRequestData: state => state.isRequestData,
            payments: state => state.payments,
            receipts: state => state.receipts,
            paymentMethods: state => state.paymentMethods,
            totalPaymentAmount: state => {
                return state.payments.reduce((init, item) =>
                    init + Number(item.nativeAmount), 0);
            },
            totalReceiptAmount: state => {
                return state.receipts.items.reduce((init, item) =>
                    init + Number(item.nativeCurrentPaid), 0);
            },
        }
    })
    const tvInputAction = function () {
        return {
            filter: $('#code').val().trim(),
            prefix: $('#prefix').val().trim(),
            startDate: $('#startDate').val().trim(),
            endDate: $('#endDate').val().trim(),
            startNo: $('#startNo').val().trim(),
            endNo: $('#endNo').val().trim(),
            docNo: $('#docNo').val().trim()
        };
    };
    const prefix = 'RV'
    const editHandle = function (id, isCopy) {
        const requests = [];
        if (id) {
            requests.push(voucherRequests.get(id));
        } else {
            requests.push(new Promise(resolve => resolve({
                voucherDate: new Date(),
                prefix: prefix, genNo: 0, details: []
            })));
        }

        if (!store.getters.isRequestData) {
            requests.push(accounting.basicData.currency.getActiveList().then(result =>
                store.commit('setCurrencies', { currencies: result })));
            requests.push(accounting.finance.accountingSetting.getNativeCurrency()
                .then(result => store.commit('setNativeCurrency', result)));

            requests.push(accounting.finance.subject.getList({
                maxResultCount: 1000,
                isPaymentMethod: true,
            }).then(subjectResult => {
                store.commit('setPaymentMethods', subjectResult.items)
            }))
            requests.push(accounting.finance.subject.getList({
                maxResultCount: 10,
                isIncludeAccountType: true,
                isIncludeReceivableSubject: true
            }).then(subjectResult => {
                store.commit('setSubjects', {
                    subjects: subjectResult.items,
                    setDetail: true
                })
            }))
        }

        store.commit('showModal', { isShowModal: true });
        Promise.all(requests).then(results => {
            const item = results[0];
            if (!item.details) {
                item.details = [];
            }
            if (isCopy) {
                item.id = null;
                item.code = '';
                item.prefix = prefix;
                item.genNo = 0;
                item.details.forEach(detail => {
                    detail.id = null;
                    detail.voucherId = null;
                })
            }
            store.commit('setEditItem', { item });
            if (id) {
                const details = (item.details || [])
                const subjectIds = details.filter(obj =>
                    !store.state.subjectMap[obj.subjectId]).map(
                        detailItem => detailItem.subjectId);
                let setDetail = false
                if (subjectIds.length > 0) {
                    accounting.finance.subject.getList({
                        maxResultCount: subjectIds.length,
                        sorting: '',
                        subjectIds,
                        isIncludeAccountType: true,
                    }).then(subjectResult => {
                        store.commit('setSubjects', {
                            subjects: subjectResult.items,
                            setDetail: true
                        })
                        store.commit('setDetails')
                    })
                } else {
                    setDetail = true
                }
                const companyIds = details.filter(obj => obj.subSubjectCode &&
                    !store.state.companyMap[obj.subSubjectCode]).map(
                        detailItem => detailItem.subSubjectCode)
                if (companyIds.length) {
                    accounting.basicData.company.getList({
                        maxResultCount: companyIds.length,
                        sorting: '',
                        ids: companyIds
                    }).then(companyResult => {
                        store.commit('setCompanies', { items: companyResult.items })
                        store.commit('setDetails')
                    })
                } else {
                    setDetail = true
                }
                if (setDetail) {
                    store.commit('setDetails')
                }
                voucherRequests.getReceivableDetails(id).then(result => {
                    const items = result || [];
                    store.commit('setReceipts', { items: items, totalCount: items.length, currentPage: 1 })
                });
            }
            if (!store.getters.isRequestData) {
                store.commit('setIsRequestData', { isRequestData: true });
            }
        });
    }

    abp.modals.UpdateStatusModal = function () {
        function initModal(modalManager, args) {
            const { id } = args;
            $("#updateStatusForm").attr('data-id', id);
        };

        return {
            initModal: initModal
        };
    };

    const updateStatusModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'GeneralLedger/Components/UpdateStatusModal',
        modalClass: 'UpdateStatusModal'
    });

    const dataTable = $('#voucherTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(voucherRequests.getList, tvInputAction),
            columnDefs: [
                {
                    title: l('Actions'),
                    orderable: false,
                    visible: isGrantedEdit || isGrantedDelete,
                    rowAction: {
                        items:
                            [
                                {
                                    text: l('Edit'),
                                    iconClass: '',
                                    action: function (data) {
                                        editHandle(data.record.id);
                                    },
                                    visible: isGrantedEdit
                                },
                                {
                                    text: l('Copy'),
                                    iconClass: '',
                                    action: function (data) {
                                        editHandle(data.record.id, true);
                                    },
                                    visible: abp.auth.isGranted('Accounting.Receivable.ReceivableVoucher.Creation')
                                },
                                {
                                    text: l('UpdateStatus'),
                                    iconClass: '',
                                    action: function (data) {
                                        updateStatusModal.open({ id: data.record.id })
                                    },
                                    visible: abp.auth.isGranted('Accounting.Receivable.ReceivableVoucher.UpdateStatus')
                                },
                                {
                                    text: l('Delete'),
                                    visible: isGrantedDelete,
                                    confirmMessage: function (data) {
                                        return l('DeletionConfirmationMessage', l('Menu:ReceivableVoucher'), data.record.code);
                                    },
                                    action: function (data) {
                                        voucherRequests
                                            .delete(data.record.id)
                                            .then(function () {
                                                abp.notify.success(l('SuccessfullyDeleted'));
                                                dataTable.ajax.reload();
                                            });
                                    }
                                }
                            ]
                    }
                },
                {
                    title: l('Code'),
                    data: "code",
                    orderable: true
                },
                {
                    title: l('VoucherDate'),
                    data: "voucherDate",
                    orderable: true,
                    dataFormat: 'date'
                },
                {
                    title: l('Status'),
                    data: "status",
                    orderable: true,
                    render: function (data) {
                        return data === 1 ? l('Approval') : data === 2 ? l('Void') : l('Draft');
                    }
                },
                {
                    title: l('CreationTime'),
                    data: "creationTime",
                    orderable: true,
                    dataFormat: 'datetime'
                },
                {
                    title: l('LastModificationTime'),
                    data: "lastModificationTime",
                    orderable: true,
                    dataFormat: 'datetime'
                },
            ]
        })
    );

    $(document).on('keydown', '#searchForm', function (e) {
        if (e.which !== 13) {
            return;
        }
        e.preventDefault();
        dataTable.ajax.reload();
    })
    $(document).on('click', '#searchBtn', function (e) {
        e.preventDefault();
        dataTable.ajax.reload();
    });
    $('#voucherTable').on('preXhr.dt', function () {
        abp.ui.setBusy('#voucherTable')
    });
    $('#voucherTable').on('xhr.dt', function (e) {
        abp.ui.clearBusy('#voucherTable')
    });
    $(document).on('click', '#newVoucherBtn', function () {
        editHandle();
    });
    $(document).on('click', '#updateStatusForm [type="submit"]', function (e) {
        e.preventDefault();
        const bodySelector = '#updateStatusForm .modal-body';
        abp.ui.setBusy(bodySelector)
        const id = $("#updateStatusForm").attr('data-id');
        accounting.finance.transferVoucher.updateStatus(id, $('#newStatus').val()).then(() => {
            updateStatusModal.close();
            dataTable.ajax.reload();
            abp.ui.clearBusy(bodySelector);
            abp.notify.success(l('SavedSuccessfully'));
        }).catch(() => abp.ui.clearBusy(bodySelector));
    })

    let openedModals = 0
    function setZIndex(modal) {
        openedModals++;
        let zIndex = parseInt($(modal).css('z-index')) + openedModals
        modal.style.zIndex = zIndex;
    }

    function getSelect2Language() {
        const languageMap = { 'zh-Hans': 'zh-CN', 'zh-Hant': 'zh-TW' }
        const cultureName = abp.localization.currentCulture.cultureName;
        return languageMap[cultureName] || 'en';
    }
    function initCompanySelect(isClient, targetSelector, dropdownParent, selectEvent) {
        const $target = $(targetSelector);
        const url = isClient ? '/api/app/client' : '/api/app/vendor'
        const language = getSelect2Language();
        const setCompanies = (items) => store.commit('setCompanies', { items })
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
                    const results = items.map(function (item, index) {
                        const { name, id, code } = item;
                        const text = code + ' - ' + name;
                        return {
                            id,
                            text: text,
                            displayName: text
                        }
                    });
                    setCompanies(items);
                    return { results: results };
                }
            },
            width: '100%',
            dropdownParent: dropdownParent ? $(dropdownParent) : null,
            placeholder: '',
            allowClear: true,
            language: language
        });
        $target.on('select2:select', function (e) {
            selectEvent(e);
        });
    }
    function getDefaultDetail() {
        return {
            subjectId: '-',
            subSubjectCode: null,
            description: '',
            debitorCreditor: debitCredit.debitor,
            currencyCode: '',
            currencyRate: 1,
            foreignAmount: 0,
            nativeAmount: 0,
            docNo: '',
            dueDate: null,
            project: '',
            department: '',
            region: '',
            custom1: '',
            custom2: '',
            itemQty: 0,
            isOriginal: true,
            paymentReference: '',
            isSubSubjectType: false,
            subjectName: '',
            subSubjectName: '',
            accountTypeCategory: 0,
            rowid: getRowId()
        };
    }

    const maxResultCount = 10
    function formatRowDate(value) {
        return value ? new Date(value).toLocaleDateString() : ''
    }

    const modalTemplate = `
<form ref="form"  class="needs-validation" novalidate>
    <div ref="modal" :class="[value ? 'show d-block' : '']" :id="modalId" role="dialog" aria-modal="true" class="modal fade" tabindex="-1"  style="background:rgba(157, 159, 160, 0.8);">
      <div class="modal-dialog modal-xl" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{title}}</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close" @click="close"></button>
          </div>
          <div class="modal-body">
            <slot></slot>
          </div>
          <div class="modal-footer">
            <slot name="footer"></slot>
            <button type="button" class="btn btn-outline-primary" data-bs-dismiss="modal" @click="close">{{l('Cancel')}}</button>
            <button type="submit" class="btn btn-primary" @click="save"><i class="fa fa-check"></i> {{l('Save')}}</button>
          </div>
        </div>
      </div>
    </div>
</form>`;
    const Modal = {
        template: modalTemplate,
        props: {
            value: Boolean,
            title: String,
            modalId: String
        },
        mounted() {
            this.$nextTick(() => {
                document.body.classList.add('modal-open');
                document.body.appendChild(this.$refs.form);
                setZIndex(this.$refs.modal)
            })
        },
        unmounted() {
            document.body.classList.remove('modal-open');
            document.body.removeChild(this.$refs.modal);
        },
        methods: {
            close() {
                this.$emit('input', !this.value);
            },
            save(e) {
                e.preventDefault();
                this.$emit('save');
            },
            l
        }
    }

    const PageTemplate = `<nav v-if="totalCount > 0" aria-label="Page navigation">
  <ul class="pagination">
    <li class="page-item">
      <a @click="previousPage" class="page-link" href="#" aria-label="Previous">
        <span aria-hidden="true">&laquo;</span>
      </a>
    </li>
      <li v-for="i in pages" :key="i"  :class="{ active: i === currentPage }" class="page-item">
      <a @click="()=>changePage(i)" class="page-link" href="#">{{ i }}</a>
    </li>
    <li class="page-item">
      <a @click="nextPage" class="page-link" href="#" aria-label="Next">
        <span aria-hidden="true">&raquo;</span>
      </a>
    </li>
  </ul>
</nav>`;
    const Page = {
        props: ['currentPage', 'totalCount'],
        template: PageTemplate,
        data() {
            return {
                pages: [],
                maxPage: 0
            };
        },
        watch: {
            currentPage: {
                handler() {
                    this.generatePage()
                },
                immediate: true
            },
            totalCount: {
                handler() {
                    this.generatePage()
                },
                immediate: true
            }
        },
        methods: {
            changePage(newPage) {
                this.emitChangePage(newPage);
            },
            previousPage() {
                const newPage = Math.max(this.currentPage - 1, 1);
                this.emitChangePage(newPage);
            },
            nextPage() {
                const newPage = Math.min(this.currentPage + 1, this.maxPage);
                this.emitChangePage(newPage);
            },
            emitChangePage(newPage) {
                if (newPage == this.currentPage) {
                    return
                }
                this.$emit('change-page', newPage);
            },
            generatePage() {
                const currentPage = this.currentPage;
                const maxPage = Math.ceil(this.totalCount / maxResultCount);
                this.maxPage = maxPage;
                const step = 1;
                let start = currentPage - step;
                let end = currentPage + step;
                if (start <= 0) {
                    start = currentPage;
                }
                if (end > maxPage) {
                    end = maxPage;
                }
                if (currentPage == 1 && end + step <= maxPage) {
                    end += step
                }
                if (currentPage == maxPage && start - step > 0) {
                    start -= step
                }
                this.pages = [];
                for (let i = start; i <= end; i++) {
                    this.pages.push(i);
                }
            }
        }
    }

    const paymentsTemplate = `<div class="payments"> 
    <table class="table table-responsive table-striped align-middle">
                 <colgroup>
                     <col/>
                     <col/>
                     <col style="width: 150px;"/>
                     <col style="width: 150px;"/>
                     <col/>
                     <col style="width: 150px;"/>
                     <col/>
                     <col/>
                 </colgroup>
                <thead>
                <tr> 
                    <th>{{ l('PayMethod') }}</th>
                    <th>{{ l('Currency') }}</th>
                    <th>{{ l('ExchangeRate')}}</th>
                    <th>{{ l('Amount') }}</th>
                    <th>{{ nativeCurrency }}</th>
                    <th>{{ l('PaymentReference') }}</th>
                    <th>+/-</th>
                    <th>{{l('Actions')}}</th>
                </tr>
                </thead>
                <tbody>
                    <tr v-for="item in payments || []" :key="item.rowid">
                        <td>
                             <select v-model="item.subjectId" @change="()=> subjectChange(item)" class="form-control lpx-select2" id="subjectId" name="subjectId" >
                                <option value="">--</option>
                                <option v-for="subjectItem in paymentMethods || []" :key="subjectItem.id" :value="subjectItem.id">{{subjectItem.code + ' - '+ subjectItem.name }}</option>
                            </select>
                        </td>
                        <td>
                            <select v-model="item.currencyCode" @change="()=> currencyChange(item)" class="form-control" id="currencyCode" name="currencyCode">
                                <option value="">--</option>
                                <option v-for="currencyItem in currencies || []" :key="currencyItem.id" :value="currencyItem.targetCurrency">{{ currencyItem.targetCurrency }}</option>
                            </select>
                        </td>
                        <td> 
                             <input v-model="item.currencyRate" @change="()=> setNativeAmount(item)" type="text" class="form-control" name="currencyRate">
                        </td>
                        <td>
                            <input v-model="item.foreignAmount" @change="()=> setNativeAmount(item)" type="text" class="form-control" name="foreignAmount">
                        </td>
                        <td>{{ renderAmount(item.nativeAmount) }}</td>
                        <td>
                            <input v-model="item.paymentReference" type="text" class="form-control" name="paymentReference">
                        </td>
                        <td>
                            <select v-model="item.debitorCreditor" class="form-control" id="debitorCreditor" name="debitorCreditor">
                                <option v-for="crdrItem in debitorCreditors" :key="crdrItem.value" :value="crdrItem.value">{{crdrItem.text}}</option>
                            </select>
                        </td>
                        <td>
                            <a @click="()=> removePaymentItem({ item })" :title="l('Delete')" class="me-1" href="#"><i class="fa-solid fa-trash"></i></a>
                            <input type="checkbox" v-model="item.isSelected"  name="isSelected" class="form-check-input">
                        </td>
                    </tr>
                </tbody>
                <tfoot>
                    <tr>
                    <td colspan="4" class="text-end">{{l('Total')}}</td>
                    <td>{{renderAmount(totalPaymentAmount)}}</td>
                    <td colspan="3" class="text-end">
                        <button type="button" class="btn btn-primary btn-sm" @click="()=> addPaymentItem()"><i class="fa-solid fa-plus"></i></button>
                    </td>
                    </tr>
                </tfoot>
            </table> 
    </div>`;
    const Payments = {
        template: paymentsTemplate,
        data() {
            return {
                debitorCreditors: [
                    { value: debitCredit.debitor, text: '+' },
                    { value: debitCredit.creditor, text: '-' }],
            }
        },
        computed: {
            ...Vuex.mapGetters(['subjects', 'currencies', 'subjectMap',
                'payments', 'nativeCurrency', 'paymentMethods', 'receipts',
                'totalPaymentAmount'])
        },
        mounted() {
            if (this.payments.length === 0) {
                this.addPaymentItem();
            }
        },
        methods: {
            renderAmount,
            l,
            ...Vuex.mapMutations(['setSubjects', 'addPaymentItem', 'removePaymentItem']),
            setNativeAmount(item) {
                if (item.foreignAmount) {
                    item.foreignAmount = item.foreignAmount.toString().trim();
                }
                if (item.currencyRate) {
                    item.currencyRate = item.currencyRate.toString().trim();
                }
                const foreignAmount = Number(item.foreignAmount);
                const currencyRate = Number(item.currencyRate);
                const amount = Math.round(foreignAmount * currencyRate, 2);

                item.nativeAmount = amount
            },
            currencyChange(item) {
                const currency = this.currencies.find(c => c.targetCurrency === item.currencyCode);
                if (currency) {
                    item.currencyRate = this.renderAmount(currency.exchangeRate, 7);;
                    this.setNativeAmount(item);
                }
            },
            subjectChange(item) {
                const subject = this.subjectMap[item.subjectId];
                if (subject) {
                    const { debitorCreditor, currencyCode } = subject;
                    item.debitorCreditor = debitorCreditor;

                    item.currencyCode = currencyCode || this.nativeCurrency;
                    if (item.currencyCode) {
                        this.currencyChange(item);
                    }
                    this.setNativeAmount(item);
                }
            }
        }
    };
    const receiptsTemplate = `<div class="receipts">
    <table class="table table-responsive table-striped align-middle">
                 <colgroup>
                 <col/>
                 <col/>
                 <col/>
                 <col/>
                 <col/>
                 <col />
                 <col/>
                 <col />
                 <col style="width: 150px;" />
                 <col />
                 <col/>
             </colgroup>
                <thead>
                <tr>
                    <th>&nbsp;</th>
                    <th>{{l('Date')}}</th>
                    <th>{{l('DocNo')}}</th>
                    <th>{{l('Currency')}}</th>
                    <th>{{l('ExchangeRate')}}</th>
                    <th class="text-end normal">{{l('ForeignAmount')}}</th>
                    <th>{{ l('ArPvDeposit') }}</th>
                    <th>{{ l('ApRvDeposit') }}</th>
                    <th>{{ l('PaymentAmount') }}</th>
                    <th>{{nativeCurrency}}</th>
                    <th>{{l('Actions')}}</th>
                </tr>
                </thead>
                <tbody>
                    <tr v-for="item in receipts.items || []" :key="item.docNo">
                        <td>{{ getItemType(item) }}</td>
                        <td>{{ formatRowDate(item.dueDate) }}</td>
                        <td>{{ item.docNo }}</td>
                        <td>{{ item.currencyCode }}</td>
                        <td>{{ renderAmount(item.currencyRate, 7) }}</td>
                        <td>{{ renderAmount(item.foreignAmount) }}</td>
                        <td>{{ item.debitorCreditor === debitCredit.debitor ? renderAmount(item.osAmount) : '0.00' }}</td>
                        <td>{{ item.debitorCreditor === debitCredit.creditor ? renderAmount(item.osAmount) : '0.00' }}</td>
                        <td>
                            <input v-model="item.currentPaid" @change="()=> currentPaidChanged(item)" type="text" class="form-control" name="currentPaid">
                        </td>
                        <td>{{ renderAmount(item.nativeCurrentPaid) }}</td>
                        <td> 
                            <a @click="()=> fullPay(item)" :title="l('FullPay')" class="me-1" href="#"><i class="fa-solid fa-f"></i></a>
                             <a @click="()=> unpaid(item)" :title="l('Unpaid')" href="#"><i class="fa-solid fa-n"></i></a>
                        </td>
                    </tr>
                </tbody>
                <tfoot>
                    <tr>
                    <td colspan="5" class="text-end">{{l('Total')}}</td>
                    <td>{{renderAmount(totalNativeAmount)}}</td>
                    <td colspan="3"></td>
                    <td colspan="2">{{renderAmount(totalReceiptAmount)}}</td>
                    </tr>
                </tfoot>
            </table>
            <Page :current-page="receipts.currentPage" :total-count="receipts.totalCount" @change-page="changePage"/>
    </div>`;
    function changeCurrentPaid(item) {
        const { currentPaid, currencyRate, foreignAmount } = item;
        let currentPaidAmount = Number(currentPaid);
        if (isNaN(currentPaidAmount) || currentPaidAmount < 0) {
            currentPaidAmount = 0;
        }
        const foreignAmt = Number(foreignAmount);
        if (currentPaidAmount > foreignAmt) {
            currentPaidAmount = foreignAmt;
            item.currentPaid = foreignAmt;
        }

        const rate = Number(currencyRate);
        const amount = Math.round(currentPaidAmount * rate, 2);

        item.nativeCurrentPaid = amount;
    }
    const Receipts = {
        components: { Page },
        template: receiptsTemplate,
        data() {
            return {
                debitCredit
            }
        },
        computed: {
            ...Vuex.mapGetters(['receipts', 'nativeCurrency', 'totalReceiptAmount']),
            totalNativeAmount() {
                return (this.receipts.items || []).reduce((init, item) =>
                    init + Number(item.nativeAmount), 0);
            },
        },
        methods: {
            renderAmount,
            l,
            formatRowDate,
            fullPay(item) {
                const { currencyRate, foreignAmount } = item;
                const foreignAmt = Number(foreignAmount);
                const rate = Number(currencyRate);
                const amount = Math.round(foreignAmt * rate, 2);
                item.currentPaid = foreignAmt;
                item.nativeCurrentPaid = amount;
            },
            unpaid(item) {
                item.currentPaid = 0;
                item.nativeCurrentPaid = 0;
            },
            currentPaidChanged(item) {
                changeCurrentPaid(item);
            },
            changePage(newPage) {
                this.$emit('change-page', newPage);
            },
            getItemType({ accTypeCategory, debitorCreditor }) {
                const l = this.l;
                if (accTypeCategory === accountTypes.receivable) {
                    //应收/预付
                    return debitorCreditor == debitCredit.debitor ? l('ReceivableTxt') : l('Advances');
                }
                else {
                    //应付/预收
                    return debitorCreditor == debitCredit.debitor ? l('PayableTxt') : l('Prepayment');
                }
            }
        }
    };
    const receivableDetailTemplate = `<div>
     <div class="mb-2"> 
        <button type="button" class="btn btn-primary btn-sm" @click="autoBalance">{{l('AutoBalance')}}</button>
        <button type="button" class="btn btn-primary btn-sm" @click="defaultPay">{{l('DefaultPay')}}</button>
        <button type="button" class="btn btn-primary btn-sm" @click="fullPay">{{l('FullPay')}}</button>
        <button type="button" class="btn btn-primary btn-sm" @click="generateDetails">{{l('GenerateVoucher')}}</button>
    </div>
      <Payments />
      <Receipts  @change-page="changePage"/>
    </div>`
    const ReceivableDetail = {
        components: { Payments, Receipts },
        template: receivableDetailTemplate,
        data() {
            return {
            }
        },
        computed: {
            ...Vuex.mapGetters(['receipts', 'payments', 'totalReceiptAmount',
                'totalPaymentAmount', 'editItem']),
        },
        methods: {
            l,
            ...Vuex.mapMutations(['setEditItem', 'setDetails']),
            changePage(newPage) {
                this.$emit('change-page', newPage);
            },
            autoBalance() {
                const item = this.payments.find(obj => obj.isSelected);
                if (!item) {
                    abp.message.info(this.l('PleaseChoicePaymentItem'));
                    return;
                }
                const currencyRate = Number(item.currencyRate);
                if (!currencyRate) {
                    return
                }
                let balanceAmount = this.totalReceiptAmount - this.totalPaymentAmount
                if (item.nativeAmount) {
                    if (item.debitorCreditor === debitCredit.debitor) {
                        balanceAmount -= Number(item.nativeAmount);
                    } else {
                        balanceAmount += Number(item.nativeAmount);
                    }
                }
                const debitorCreditor = balanceAmount >= 0 ? debitCredit.debitor : debitCredit.creditor;
                item.nativeAmount = Math.abs(balanceAmount);
                item.foreignAmount = Number((item.nativeAmount / currencyRate).toFixed(2))
                item.debitorCreditor = debitorCreditor;
            },
            defaultPay() {
                let balanceAmount = this.totalPaymentAmount - this.totalReceiptAmount
                if (balanceAmount <= 0) {
                    return;
                }
                for (let i = 0; i < this.receipts.items.length; i++) {
                    const item = this.receipts.items[i];
                    const foreignAmount = Number(item.foreignAmount);
                    const currentPaid = Number(item.currentPaid);
                    if (foreignAmount == currentPaid) {
                        continue;
                    }
                    const itemBalance = foreignAmount - currentPaid;
                    if (balanceAmount >= itemBalance) {
                        item.currentPaid = currentPaid + itemBalance;
                        item.nativeCurrentPaid = Math.round(item.currentPaid * Number(item.currencyRate), 2);
                        balanceAmount -= itemBalance;
                    }
                }
            },
            fullPay() {
                this.receipts.items.forEach(item => {
                    item.currentPaid = item.foreignAmount;
                    changeCurrentPaid(item);
                })
            },
            generateDetails() {
                const receipts = this.receipts.items.filter(item => item.currentPaid > 0);
                const payments = this.payments.filter(item =>
                    item.foreignAmount > 0
                    && item.subjectId
                    && item.currencyCode
                    && item.currencyRate > 0
                );
                const creditorId = this.editItem.creditorId;
                if (!creditorId) {
                    abp.message.info(this.l('PleaseEnterCreditor'));
                    return
                }
                if (payments.length === 0) {
                    abp.message.info(this.l('PleaseEnterPaymentItemInfo'));
                    return
                }
                const param = {
                    creditor: creditorId,
                    receipts,
                    payments
                };
                voucherRequests.generateDetails
                    (param).then(result => {
                        this.setEditItem({ item: { ...this.editItem, details: result || [] } });
                        this.setDetails();
                    }).catch(() => {
                        this.setEditItem({ item: { ...this.editItem, details: [] } });
                    })
            }
        }
    }
    const editDetailTemplate = `<div>
<Modal v-if="isShow" :value="isShow" @input="input" @save="save" :title="l('Detail')" modal-id="edit-modal">
    <div>
       <div style="display: grid; grid-template-columns: 1fr 1fr;">
             <div class="mb-2 mx-1">
                <label for="subjectId" class="form-label">{{l('Subject')}}<span> * </span></label>
                <select v-model="item.subjectId" @change="subjectChange" :class="{'is-invalid': errors.subjectId }" class="form-control lpx-select2" id="subjectId" name="subjectId" placeholder="">
                    <option value="-">--</option>
                    <option v-for="subjectItem in subjects || []" :key="subjectItem.id" :value="subjectItem.id">{{subjectItem.code + ' - '+ subjectItem.name }}</option>
                </select>
                <div id="subjectIdFeedback" class="invalid-feedback">
                   {{l('PleaseEnterAValue')}}
                </div>
            </div>
            <div class="mb-2 mx-1">
                <label for="description" class="form-label">{{l('Description')}}</label>
                <textarea v-model="item.description" class="form-control" id="description" rows="2" name="description"></textarea>
            </div>
            <div class="mb-2 mx-1">
                <label for="debitorCreditor" class="form-label">{{l('DebitorCreditor')}}<span> * </span></label>
                 <select v-model="item.debitorCreditor" :class="{'is-invalid': errors.debitorCreditor }" class="form-control" id="debitorCreditor" name="debitorCreditor">
                    <option v-for="crdrItem in debitorCreditors" :key="crdrItem.value" :value="crdrItem.value">{{crdrItem.text}}</option>
                </select>
                <div id="debitorCreditorFeedback" class="invalid-feedback">
                    {{l('PleaseEnterAValue')}}
                </div>
            </div>
            <div class="mb-2 mx-1">
                <label for="currencyCode" class="form-label">{{l('Currency')}}<span> * </span></label>
                <select v-model="item.currencyCode" @change="currencyChange" :class="{'is-invalid': errors.currencyCode }" class="form-control" id="currencyCode" name="currencyCode">
                    <option value="">--</option>
                    <option v-for="currencyItem in currencies || []" :key="currencyItem.id" :value="currencyItem.targetCurrency">{{ currencyItem.targetCurrency }}</option>
                </select>
                 <div id="currencyFeedback" class="invalid-feedback">
                    {{l('PleaseEnterAValue')}}
                </div>
            </div>
            <div class="mb-2 mx-1">
                <label for="foreignAmount" class="form-label">{{l('ForeignAmount')}}<span> * </span></label>
                <input v-model="item.foreignAmount" @change="setNativeAmount" :class="{'is-invalid': errors.foreignAmount }" type="text" class="form-control" id="foreignAmount" name="foreignAmount">
                 <div id="foreignAmountFeedback" class="invalid-feedback">
                   {{l('PleaseEnterAValue')}}
                </div>
            </div>
            <div class="mb-2 mx-1">
                <label for="currencyRate" class="form-label">{{l('ExchangeRate')}}<span> * </span></label>
                <input v-model="item.currencyRate" @change="setNativeAmount" :class="{'is-invalid': errors.currencyRate }" type="text" class="form-control" id="currencyRate" name="currencyRate">
                 <div id="currencyRateFeedback" class="invalid-feedback">
                   {{l('PleaseEnterAValue')}}
                </div>
            </div>
             <div class="mb-2 mx-1">
                <label for="nativeAmount" class="form-label">{{l('NativeAmount')}}</label>
                <input v-model="item.nativeAmount" type="text" class="form-control" id="nativeAmount" name="nativeAmount" readonly>
            </div>
             <div v-if="item.isSubSubjectType" class="mb-2 mx-1" id="subSubject" style="position: relative;">
                <label for="subSubjectCode" :class="{'is-invalid': errors.subSubjectCode }" class="form-label">{{l('SubSubject')}}<span> * </span></label>
                <select v-if="item.accountTypeCategory === accountTypes.receivable" v-model="item.subSubjectCode" @change="arapFieldChange"  key="ar" class="form-control" id="subSubjectCode" name="subSubjectCode">
                    <option value="-">--</option>
                    <option v-for="subItem in clients || []" :key="subItem.id" :value="subItem.id">{{subItem.code + ' - '+ subItem.name }}</option>
                 </select>
                 <select v-else v-model="item.subSubjectCode" @change="arapFieldChange" key="ap" class="form-control" id="subSubjectCode" name="subSubjectCode">
                    <option value="-">--</option>
                    <option v-for="subItem in vendors || []" :key="subItem.id" :value="subItem.id">{{subItem.code + ' - '+ subItem.name }}</option>
                 </select>
                 <div id="subSubjectCodeFeedback" class="invalid-feedback">
                    {{l('PleaseEnterAValue')}}
                </div>
             </div>
              <div v-if="item.isSubSubjectType" class="mb-2 mx-1">
                <label for="docNo" :class="{'is-invalid': errors.docNo }" class="form-label">{{l('DocNo')}}<span> * </span></label>
                <input v-model="item.docNo" @change="arapFieldChange" type="text" class="form-control" id="docNo" name="docNo">
                 <div id="docNoback" class="invalid-feedback">
                    {{l('PleaseEnterAValue')}}
                </div>
            </div>
            <div v-if="item.isSubSubjectType" class="mb-2 mx-1">
                <label for="dueDate" class="form-label">{{l('DueDate')}}<span> * </span></label>
                <input v-model="item.dueDate" @change="arapFieldChange" type="date" class="form-control" id="dueDate" name="dueDate">
            </div>
        </div>
    </div>
         <template #footer>
            <button type="button" class="btn btn-secondary" @click="autoBalance">{{l('AutoBalance')}}</button>
         </template>
</Modal></div>`;

    const EditDetail = {
        components: { Modal },
        template: editDetailTemplate,
        props: ['item', 'value'],
        data() {
            return {
                debitorCreditors: [
                    { value: debitCredit.debitor, text: l('Debitor') },
                    { value: debitCredit.creditor, text: l('Creditor') }],
                errors: {},
                isShow: false,
                accountTypes
            }
        },
        computed: {
            ...Vuex.mapGetters(['subjects', 'companyMap', 'currencies', 'subjectMap', 'clients', 'vendors'])
        },
        watch: {
            value: {
                handler(newValue) {
                    this.isShow = newValue;
                },
                immediate: true
            }
        },
        mounted() {
            this.initSubjectSelect();
        },
        unmounted() {
            $('#subjectId').off('select2:select');
        },
        methods: {
            ...Vuex.mapMutations(['setSubjects', 'setCompanies']),
            input(value) {
                this.$emit('input', value);
            },
            validate() {
                const { subjectId, currencyCode, currencyRate, foreignAmount, debitorCreditor,
                    isSubSubjectType, subSubjectCode, docNo } = this.item
                let errorCount = 0;
                this.errors = {};
                if (!subjectId || subjectId === '-') {
                    this.errors.subjectId = true;
                    errorCount++;
                }
                if (!currencyCode) {
                    this.errors.currencyCode = true;
                    errorCount++;
                }
                if (!currencyRate || isNaN(Number(currencyRate)) || Number(currencyRate) <= 0) {
                    this.errors.currencyRate = true;
                    errorCount++;
                }
                if (!foreignAmount || isNaN(Number(foreignAmount)) || Number(foreignAmount) <= 0) {
                    this.errors.foreignAmount = true;
                    errorCount++;
                }
                if (!debitorCreditor) {
                    this.errors.debitorCreditor = true;
                    errorCount++;
                }
                if (isSubSubjectType) {
                    if (!subSubjectCode || subSubjectCode === '-') {
                        this.errors.subSubjectCode = true;
                        errorCount++;
                    }
                    if (!docNo) {
                        this.errors.docNo = true;
                        errorCount++;
                    }
                }
                return errorCount === 0;
            },
            save() {
                if (!this.validate()) {
                    return;
                }
                this.$emit('save')
            },
            renderAmount,
            setNativeAmount() {
                if (this.item.foreignAmount) {
                    this.item.foreignAmount = this.item.foreignAmount.toString().trim();
                }
                if (this.item.currencyRate) {
                    this.item.currencyRate = this.item.currencyRate.toString().trim();
                }
                const foreignAmount = Number(this.item.foreignAmount);
                const currencyRate = Number(this.item.currencyRate);
                const amount = foreignAmount * currencyRate;
                if (!isNaN(foreignAmount) && foreignAmount > 0) {
                    this.errors.foreignAmount = false;
                }
                if (!isNaN(currencyRate) && currencyRate > 0) {
                    this.errors.currencyRate = false;
                }
                this.item.nativeAmount = this.renderAmount(amount)
            },
            currencyChange() {
                const currency = this.currencies.find(c => c.targetCurrency === this.item.currencyCode);
                if (currency) {
                    this.errors.currencyCode = false;
                    this.item.currencyRate = this.renderAmount(currency.exchangeRate, 7);
                    this.setNativeAmount();
                }
            },
            subjectChange() {
                const subject = this.subjectMap[this.item.subjectId];
                this.item.isSubSubjectType = false;
                if (subject) {
                    const { isSubSubjectType, accountType, debitorCreditor, currencyCode, name, code } = subject;
                    this.item.isSubSubjectType = isSubSubjectType;
                    this.item.accountTypeCategory = accountType ? accountType.category : 0;
                    this.item.debitorCreditor = debitorCreditor;
                    this.item.subjectName = code + ' - ' + name;
                    this.errors.subjectId = false;
                    if (currencyCode) {
                        this.item.currencyCode = currencyCode;
                        this.currencyChange();
                    }
                    this.setNativeAmount();
                    if (this.item.isSubSubjectType) {
                        this.$nextTick(() => this.initCompanySelect(this.item.accountTypeCategory == this.accountTypes.receivable))
                    }
                }
                this.item.subSubjectCode = '-';
                this.item.docNo = '';
                this.item.dueDate = null;
            },
            arapFieldChange() {
                const { isSubSubjectType, subSubjectCode, docNo, dueDate } = this.item
                if (isSubSubjectType) {
                    if (subSubjectCode) {
                        this.errors.subSubjectCode = false;
                    }
                    if (docNo) {
                        this.errors.docNo = false;
                    }
                }
            },
            l,
            initSubjectSelect() {
                const _this = this;

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
                                isIncludeAccountType: true
                            };
                        },
                        processResults: function (data) {
                            const items = data.items;
                            const results = items.map(function (item, index) {
                                const { name, id, code } = item;
                                const text = code + ' - ' + name;
                                return {
                                    id,
                                    text: text,
                                    displayName: text
                                }
                            });
                            _this.setSubjects({ subjects: items });
                            return { results: results };
                        }
                    },
                    width: '100%',
                    dropdownParent: $('#edit-modal'),
                    placeholder: '',
                    allowClear: true,
                    language: language
                });
                $subjectId.on('select2:select', function (e) {
                    _this.item.subjectId = e.params.data.id;
                    _this.subjectChange();
                });
            },
            initCompanySelect(isClient) {
                const _this = this;
                initCompanySelect(isClient, '#subSubjectCode', '#subSubject',
                    e => {
                        _this.item.subSubjectCode = e.params.data.id;
                        _this.item.subSubjectName = e.params.data.text;
                        const company = _this.companyMap[e.params.data.id];
                        if (company && company.currency) {
                            _this.item.currencyCode = company.currency;
                            _this.currencyChange();
                        }
                    });
            },
            autoBalance() {
                this.$emit('auto-balance')
            }
        }
    }
    const editHeaderTemplate = `<div class="head mb-2 row">
      <div class="col row">
        <label for="creditorId" class="form-label col-sm-3 text-end">{{l('Creditor')}}<span> * </span></label>
        <div  id="creditor" class="col-sm-9">
            <select v-model="editItem.creditorId" :class="{'is-invalid': errors.creditorId }" class="form-control" id="creditorId" name="creditorId">
                <option value="-">--</option>
                <option v-for="subItem in clients || []" :key="subItem.id" :value="subItem.id">{{subItem.code + ' - '+ subItem.name }}</option>
            </select>
             <div id="creditorIdFeedback" class="invalid-feedback">
                    {{l('PleaseEnterAValue')}}
             </div>
        </div>
       </div>
       <div class="col row">
            <label for="prefix" class="form-label col-sm-3 text-end">{{l('Code')}}<span> * </span></label>
            <div v-if="editItem.id" class="col-sm-9">
                 <input v-model="editItem.code" type="text" class="form-control" id="code" name="code" readonly>
            </div>
            <div v-else class="col-sm-9 row">
                <div class="col pe-0">
                    <input v-model="editItem.prefix" :class="{'is-invalid': errors.prefix }" type="text" class="form-control" id="prefix" name="prefix" placeholder="JV">
                     <div id="prefixFeedback" class="invalid-feedback">
                        {{l('PleaseEnterAValue')}}
                    </div>
                </div>
                <div class="col">
                    <input v-model="editItem.genNo" type="text" class="form-control" id="genNo" name="genNo">
                </div>
            </div> 
       </div>
       <div class="col row">
        <label for="voucherDate" class="form-label col-sm-4 text-end">{{l('VoucherDate')}}<span> * </span></label>
        <div class="col-sm-8">
            <input v-model="editItem.voucherDate" :class="{'is-invalid': errors.voucherDate }" type="date" class="form-control" id="voucherDate" name="voucherDate">
             <div id="prefixFeedback" class="invalid-feedback">
                    {{l('PleaseEnterAValue')}}
             </div>
        </div>
       </div> 
    </div> `
    const EditHeader = {
        template: editHeaderTemplate,
        props: ['errors'],
        computed: {
            ...Vuex.mapGetters(['editItem', 'clients']),
        },
        mounted() {
            const _this = this;
            this.$nextTick(() => {
                initCompanySelect(true, '#creditorId', '#creditor',
                    e => {
                        _this.editItem.creditorId = e.params.data.id;
                        _this.$emit('creditor-change', 1);
                    });
            })
        },
        methods: {
            l,
        }
    }
    const voucherDetailsTemplate = `<div>
        <div>
            <button type="button" class="btn btn-primary btn-sm" @click="addDetail">
                <i class="fa fa-plus"></i> {{l('AddDetail')}}
            </button>
        </div>
        <div class="items">
            <table class="table table-responsive table-striped" :style="tableStyle">
                <colgroup>
                    <col style="width: 120px;" />
                    <col style="width: 250px;" />
                    <col style="width: 250px;" />
                    <col style="width: 120px;" />
                    <col style="width: 120px;" />
                    <col style="min-width:120px; max-width: 160px;" />
                    <col style="width: 120px;" />
                    <col style="width: 250px;" />
                    <col style="width: 120px;" />
                    <col style="width: 120px;" />
                </colgroup>
                <thead>
                <tr>
                    <th>{{l('Actions')}}</th>
                    <th>{{l('Subject')}}</th>
                        <th>{{l('Description')}}</th>
                        <th>{{l('Debitor')}}<div>{{nativeCurrency}}</div></th>
                        <th>{{l('Creditor')}}<div>{{nativeCurrency}}</div></th>
                        <th><div>{{l('DebitorCreditor')}}</div><div>{{l('Currency')}}</div></th>
                        <th class="text-end normal"><div>{{l('ForeignAmount')}}</div><div>{{l('ExchangeRate')}}</div></th>
                        <th>{{l('SubSubject')}}</th>
                        <th>{{l('DocNo')}}</th>
                        <th>{{l('DueDate')}}</th>
                </tr>
                </thead>
                <tbody>
                    <tr v-for="item in editItem.details || []" :key="item.rowid">
                        <td><div class="btn-group" role="group" aria-label="Button group with nested dropdown">
                                <div class="btn-group" role="group">
                                <button type="button" class="btn btn-primary btn-sm dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="fa fa-cog me-1"></i>{{l('Actions')}}
                                </button>
                                <ul class="dropdown-menu">
                                    <li><a  @click="()=> showDetail(item)"  class="dropdown-item" href="#">{{l('Edit')}}</a></li>
                                    <li><a  @click="()=> deleteDetail(item)"  class="dropdown-item" href="#">{{l('Delete')}}</a></li>
                                </ul>
                                </div>
                            </div>
                        </td>
                        <td>{{ item.subjectName }}</td>
                        <td>{{item.description}}</td>
                        <td>{{item.debitorCreditor === 1 ? renderAmount(item.nativeAmount) : ''}}</td>
                        <td>{{item.debitorCreditor === -1 ? renderAmount(item.nativeAmount) : ''}}</td>
                        <td><div>{{item.debitorCreditor === 1 ? l('Debitor'): l('Creditor')}}</div><div>{{item.currencyCode}}</div></td>
                        <td class="text-end"><div>{{renderAmount(item.foreignAmount)}}</div><div>{{renderAmount(item.currencyRate, 7)}}</div></td>
                        <td>{{ item.subSubjectName }}</td>
                        <td>{{item.docNo}}</td>
                        <td>{{formatRowDate(item.dueDate)}}</td>
                    </tr>
                </tbody>
                <tfoot>
                    <tr>
                    <td colspan="3" class="text-end">{{l('Total')}}</td>
                    <td>{{renderAmount(totalDebitorAmount)}}</td>
                    <td colspan="11">{{renderAmount(totalCreditorAmount)}}</td>
                    </tr>
                </tfoot>
            </table>
        </div>
    </div>`
    const VoucherDetails = {
        template: voucherDetailsTemplate,
        data() {
            return {
                tableStyle: {
                    ['min-width']: '1460px',
                    ['max-width']: '1630px'
                }
            }
        },
        computed: {
            ...Vuex.mapGetters(['editItem', 'totalDebitorAmount',
                'totalCreditorAmount', 'nativeCurrency'])
        },
        methods: {
            l,
            renderAmount,
            formatRowDate,
            addDetail() {
                this.$emit('add-detail');
            }
        }
    }

    const editModalTemplate = `<div><Modal  v-if="isShowModal" :value="isShowModal" @input="input" @save="save" :title="l(editItem.id ? 'EditReceivableVoucher' : 'NewReceivableVoucher' )">
<div id="content">
    <EditHeader v-if="isShowHeader" :errors="errors" @creditor-change="getDetailsByDebitor" />
    <ul class="nav nav-tabs"  id="detailTab" role="tablist">
      <li class="nav-item" role="presentation">
        <a :class="[showDetailTab? 'active':'']" class="nav-link" aria-current="page" href="#" data-bs-toggle="tab" data-bs-target="#details-tab-pane"
            role="tab" aria-controls="details-tab-pane" aria-selected="true" @click="()=>showDetailTab=true">{{l('Voucher')}}</a>
      </li>
       <li class="nav-item" role="presentation">
        <a :class="[!showDetailTab? 'active':'']" class="nav-link" aria-current="page" href="#" data-bs-toggle="tab" data-bs-target="#receipts-tab-pane"
            role="tab" aria-controls="receipts-tab-pane" aria-selected="true" @click="()=>showDetailTab=false">{{l('Detail')}}</a>
      </li>
    </ul>
    <div class="tab-content pt-0 pb-0" id="detailTabContent">
      <div :class="[showDetailTab? 'show active':'']" class="tab-pane fade" id="details-tab-pane" role="tabpanel" aria-labelledby="details-tab" tabindex="0">
        <VoucherDetails  @add-detail="addDetail"/>
      </div>
     <div :class="[!showDetailTab? 'show active':'']" class="tab-pane fade" id="receipts-tab-pane" role="tabpanel" aria-labelledby="receipts-tab" tabindex="0">
      <ReceivableDetail  @change-page="changeReceiptPage"/>
     </div>
    </div>
</div> 
</Modal><EditDetail v-if="showDetailModal" v-model="isShowDetail" :item="item" @save="saveDetail" @auto-balance="autoBalance"></EditDetail></div>`;

    const EditModal = {
        components: { Modal, EditHeader, VoucherDetails, EditDetail, ReceivableDetail },
        template: editModalTemplate,
        data() {
            return {
                isShowDetail: false,
                item: {},
                errors: {},
                showDetailModal: false,
                showDetailTab: false,
                isShowHeader: false,
            }
        },
        watch: {
            isShowDetail(value) {
                this.$nextTick(() => { this.showDetailModal = value })
            },
            isShowModal(value) {
                this.$nextTick(() => { this.isShowHeader = value })
            }
        },
        computed: {
            ...Vuex.mapGetters(['isShowModal', 'editItem'])
        },
        methods: {
            ...Vuex.mapMutations(['showModal', 'saveDetailItem', 'setReceipts',
                'removeDetailItem']),
            input(value) {
                this.showDetailTab = false;
                this.showModal({ isShowModal: value })
            },
            validate() {
                const { id, prefix, voucherDate } = this.editItem
                this.errors = {};
                let errorCount = 0;
                let message = ''
                if (!id && !prefix) {
                    this.errors.prefix = true;
                    errorCount++;
                }
                if (!voucherDate || (new Date(voucherDate)).toString() === 'Invalid Date') {
                    this.errors.voucherDate = true;
                    errorCount++;
                }
                if (this.totalCreditorAmount == 0 || this.totalDebitorAmount == 0) {
                    message = this.l('DebitorCreditorAmountMustGreaterThanZero');
                    errorCount++;
                }
                if (this.totalCreditorAmount !== this.totalDebitorAmount) {
                    message += this.l('VoucherDoesNotBalance');
                    errorCount++;
                };
                if (message) {
                    abp.message.error(message);
                }
                return errorCount === 0;
            },
            save() {
                this.editItem.voucherDate = document.querySelector("#voucherDate").value;
                if (!this.validate()) {
                    return;
                }
                abp.ui.setBusy('#content');
                const data = { ...this.editItem }
                data.voucherDate = data.voucherDate;
                const request = this.editItem.id ? voucherRequests.update(this.editItem.id, data) :
                    voucherRequests.create(data);
                request.then(() => {
                    abp.ui.clearBusy('#content');
                    abp.notify.success(this.l('SavedSuccessfully'));
                    this.showModal({ isShowModal: false });
                    $('#voucherTable').DataTable().ajax.reload();
                }).catch(() => {
                    abp.ui.clearBusy('#content');
                });
            },
            showDetail(item) {
                this.item = item;
                const { dueDate } = this.item;
                if (dueDate) {
                    this.item.dueDate = this.formatInputDate(dueDate)
                }
                this.isShowDetail = true;
            },
            deleteDetail(item) {
                this.removeDetailItem({ item })
                this.isShowDetail = false;
            },
            saveDetail() {
                this.saveDetailItem({ item: this.item });
                this.isShowDetail = false;
            },
            addDetail() {
                this.item = getDefaultDetail();
                this.isShowDetail = true;
            },
            changeReceiptPage(newPage) {
                this.getDetailsByDebitor(newPage);
            },
            getDetailsByDebitor(newPage) {
                const params = {
                    debitorId: this.editItem.creditorId,
                    maxResultCount,
                    skipCount: (newPage - 1) * maxResultCount
                };
                voucherRequests
                    .getReceivableDetailsByDebitor(params)
                    .then(response => {
                        this.setReceipts({ ...(response || {}), currentPage: newPage });
                    }).catch(() => { });
            },
            l,
            formatInputDate(value) {
                return (new moment(value)).format("yyyy-MM-DD")
            },
            autoBalance() {
                const item = this.item;
                const currencyRate = Number(item.currencyRate);
                if (!currencyRate) {
                    return
                }
                let balanceAmount = this.totalDebitorAmount - this.totalCreditorAmount
                if (item.nativeAmount) {
                    if (item.debitorCreditor === debitCredit.debitor) {
                        balanceAmount -= Number(item.nativeAmount);
                    } else {
                        balanceAmount += Number(item.nativeAmount);
                    }
                }
                const debitorCreditor = balanceAmount > 0 ? debitCredit.creditor : debitCredit.debitor;
                item.nativeAmount = Math.abs(balanceAmount);
                item.foreignAmount = Number((item.nativeAmount / currencyRate).toFixed(2))
                item.debitorCreditor = debitorCreditor;
            }
        }
    }

    const App = {
        components: { EditModal },
        template: `<div><EditModal /></div>`,
    }

    const app = new Vue({
        components: { App },
        template: `<App />`,
        el: '#app',
        store,
        abp,
        accounting
    });
});
