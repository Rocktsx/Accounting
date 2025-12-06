
$(function () {
    const l = abp.localization.getResource('Accounting');
    const isGrantedEdit = abp.auth.isGranted('Accounting.GeneralLedger.TransferVoucher.Edit');
    const isGrantedDelete = abp.auth.isGranted('Accounting.GeneralLedger.TransferVoucher.Deletion');
    // 创建一个新的 store 实例 
    function formatDate(value) {
        return (new moment(value)).format("yyyy-MM-DD")
    }

    const draftStatus = 0;

    const store = new Vuex.Store({
        state() {
            return {
                editItem: { details: [] },
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
                enableProject: abp.features.isEnabled('AccountingFeature.ProjectFunction'),
                enableRegion: abp.features.isEnabled('AccountingFeature.RegionFunction'),
                enableDepartment: abp.features.isEnabled('AccountingFeature.DepartmentFunction'),
                enableCustom1: abp.features.isEnabled('AccountingFeature.Custom1Function'),
                enableCustom2: abp.features.isEnabled('AccountingFeature.Custom2Function')
            }
        },
        mutations: {
            increment(state) {
                state.count++
            },
            showModal(state, payload) {
                state.isShowModal = payload.isShowModal;
                state.editItem.id = payload.id || ''

            },
            setIsEdit(state, payload) {
                state.isEdit = payload.isEdit;
            },
            setEditItem(state, payload) {
                const { details, ...others } = payload.item || { details: [] };
                const newDetails = details.map(item => ({
                    ...item,
                    subjectName: '',
                    subSubjectName: '',
                    isSubSubjectType: false,
                    accountTypeCategory: 0
                }));
                state.editItem = { ...others, details: newDetails };
                state.editItem.voucherDate = formatDate(state.editItem.voucherDate);
            },
            setSubjects(state, payload) {
                const { subjects } = payload;
                (subjects || []).forEach(item => {
                    if (!state.subjectMap[item.id]) {
                        state.subjectMap[item.id] = item;
                        state.subjects.push(item);
                    }
                });
            },
            setDetails(state) {
                (state.editItem.details || []).forEach(item => {
                    const subject = state.subjectMap[item.subjectId];
                    if (subject) {
                        const { code, name, isSubSubjectType, accountType } = subject;
                        item.isSubSubjectType = isSubSubjectType;
                        item.accountTypeCategory = accountType ? accountType.category : 0;
                        item.subjectName = code + ' - ' + name;
                    }
                });
                (state.editItem.details || []).forEach(item => {
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
                    if (!state.vendorMap[c.id] && c.isVendor) {
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
        },
        getters: {
            isShowModal: state => state.isShowModal,
            editItem: state => state.editItem,
            totalDebitorAmount: state => {
                return (state.editItem.details || []).reduce((init, item) =>
                    init + (item.debitorCreditor === 1 ?
                        Number(item.nativeAmount) : 0), 0)
            },
            totalCreditorAmount: state => {
                return (state.editItem.details || []).reduce((init, item) =>
                    init + (item.debitorCreditor === -1 ?
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
            enableProject: state => state.enableProject,
            enableRegion: state => state.enableRegion,
            enableDepartment: state => state.enableDepartment,
            enableCustom1: state => state.enableCustom1,
            enableCustom2: state => state.enableCustom2,
            isDraftStatus: state => !state.editItem.id || state.editItem.id && state.editItem.status === draftStatus
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
    const editHandle = function (id, isCopy) {
        const requests = [];
        if (id) {
            requests.push(accounting.finance.transferVoucher.get(id));
        } else {
            requests.push(new Promise(resolve => resolve({
                voucherDate: new Date(),
                prefix: 'JV', genNo: 0, details: []
            })));
        }

        if (!store.getters.isRequestData) {
            requests.push(accounting.basicData.currency.getActiveList().then(result =>
                store.commit('setCurrencies', { currencies: result })));
            requests.push(accounting.finance.accountingSetting.getNativeCurrency()
                .then(result => store.commit('setNativeCurrency', result)));
        }

        store.commit('showModal', { isShowModal: true, id });
        Promise.all(requests).then(results => {
            const item = results[0];
            if (!item.details) {
                item.details = [];
            }
            if (isCopy) {
                item.id = null;
                item.code = '';
                item.prefix = 'JV';
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
                        isIncludeAccountType: true
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
            ajax: abp.libs.datatables.createAjax(accounting.finance.transferVoucher.getList, tvInputAction),
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
                                    visible: function (record) {
                                        return isGrantedEdit && record.status == draftStatus;
                                    }
                                },
                                {
                                    text: l('View'),
                                    iconClass: '',
                                    action: function (data) {
                                        editHandle(data.record.id);
                                    },
                                    visible: function (record) {
                                        return isGrantedEdit && record.status !== draftStatus;
                                    }
                                },
                                {
                                    text: l('Copy'),
                                    iconClass: '',
                                    action: function (data) {
                                        editHandle(data.record.id, true);
                                    },
                                    visible: abp.auth.isGranted('Accounting.GeneralLedger.TransferVoucher.Creation')
                                },
                                {
                                    text: l('UpdateStatus'),
                                    iconClass: '',
                                    action: function (data) {
                                        updateStatusModal.open({ id: data.record.id })
                                    },
                                    visible: abp.auth.isGranted('Accounting.GeneralLedger.TransferVoucher.UpdateStatus')
                                },
                                {
                                    text: l('Delete'),
                                    visible: isGrantedDelete,
                                    confirmMessage: function (data) {
                                        return l('DeletionConfirmationMessage', l('Menu:TransferVoucher'), data.record.code);
                                    },
                                    action: function (data) {
                                        accounting.finance.transferVoucher
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
    const importModal = new abp.ModalManager(abp.appPath + 'GeneralLedger/TransferVouchers/ImportModal');
    $(document).on('click', '#importTVBtn', function () {
        importModal.open();
    });
    $(document).on('click', '#importDataForm [type="submit"]', function (e) {
        e.preventDefault();
        const fileElement = document.querySelector('#importDataForm #file');
        const formData = new FormData();
        formData.append('file', fileElement.files[0]);
        abp.ui.setBusy('#importDataForm .modal-body')
        abp.ajax({
            url: abp.appPath + 'api/transfer-vouchers',
            processData: false,
            contentType: false,
            method: 'POST',
            data: formData,
            success: function (result) {
                importModal.close();
                dataTable.ajax.reload();
                abp.notify.success(l('ImportDataSuccessfully'));
            },
            complete() {
                abp.ui.clearBusy('#importDataForm .modal-body')
            }
        });
    })
    function getLocal(key) {
        return abp.localization.getResource('Accounting')(key);
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
            <button v-if="showSubmit" type="submit" class="btn btn-primary" @click="save"><i class="fa fa-check"></i> {{l('Save')}}</button>
          </div>
        </div>
      </div>
    </div>
</form>`;
    let openedModals = 0
    function setZIndex(modal) {
        openedModals++;
        let zIndex = parseInt($(modal).css('z-index')) + openedModals
        modal.style.zIndex = zIndex;
    }
    const Modal = {
        template: modalTemplate,
        props: {
            value: Boolean,
            title: String,
            modalId: String,
            showSubmit: {
                type: Boolean,
                default: true
            },
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
            l(key) {
                return getLocal(key);
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
                <label for="dueDate" :class="{'is-invalid': errors.dueDate }" class="form-label">{{l('DueDate')}}<span> * </span></label>
                <input v-model="item.dueDate" @change="arapFieldChange" type="date" class="form-control" id="dueDate" name="dueDate">
                 <div id="dueDateFeedback" class="invalid-feedback">
                    {{l('PleaseEnterAValue')}}
                </div>
            </div>
             <div v-if="enableProject" class="mb-2 mx-1">
                <label for="project" class="form-label">{{l('Project')}}</label>
                <input v-model="item.project" type="text" class="form-control" id="project" name="project">
            </div>
            <div v-if="enableRegion" class="mb-2 mx-1">
                <label for="department" class="form-label">{{l('Department')}}</label>
                <input v-model="item.department" type="text" class="form-control" id="department" rows="3" name="department">
            </div>
             <div v-if="enableDepartment" class="mb-2 mx-1">
                <label for="region" class="form-label">{{l('Region')}}</label>
                <input v-model="item.region" type="text" class="form-control" id="region" name="region">
            </div>
            <div v-if="enableCustom1" class="mb-2 mx-1">
                <label for="custom1" class="form-label">{{l('Custom1')}}</label>
                <input v-model="item.custom1" type="text" class="form-control" id="custom1" rows="3" name="custom1">
            </div>
             <div v-if="enableCustom2" class="mb-2 mx-1">
                <label for="custom2" class="form-label">{{l('Custom2')}}</label>
                <input v-model="item.custom2" type="text" class="form-control" id="custom2" name="custom2">
            </div>
        </div>
    </div>
         <template #footer>
            <button type="button" class="btn btn-secondary" @click="autoBalance">{{l('AutoBalance')}}</button>
         </template>
</Modal></div>`;
    const accountTypes = {
        receivable: 2,
        payable: 3
    }
    const EditDetail = {
        components: { Modal },
        template: editDetailTemplate,
        props: ['item', 'value'],
        data() {
            return {
                debitorCreditors: [{ value: 1, text: getLocal('Debitor') }, { value: -1, text: getLocal('Creditor') }],
                errors: {},
                isShow: false,
                accountTypes
            }
        },
        computed: {
            ...Vuex.mapGetters(['subjects', 'companyMap', 'currencies', 'subjectMap', 'clients', 'vendors',
                'enableProject', 'enableRegion', 'enableDepartment', 'enableCustom1', 'enableCustom2'])
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
            if (this.item.isSubSubjectType) {
                this.$nextTick(() => this.initCompanySelect(this.item.accountTypeCategory == this.accountTypes.receivable))
            }
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
                    isSubSubjectType, subSubjectCode, docNo, dueDate } = this.item
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
                    if (!dueDate || (new Date(dueDate)).toString() === 'Invalid Date') {
                        this.errors.dueDate = true;
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
            renderAmount(amount, scale) {
                const num = Number(amount);
                return !Number.isNaN(num) ? num.toFixed(scale ? scale : 2) : '0.00';
            },
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
                    if (dueDate && (new Date(dueDate)).toString() !== 'Invalid Date') {
                        this.errors.dueDate = false;
                    }
                }
            },
            l(key) {
                return getLocal(key);
            },
            getSelect2Language() {
                const languageMap = { 'zh-Hans': 'zh-CN', 'zh-Hant': 'zh-TW' }
                const cultureName = abp.localization.currentCulture.cultureName;
                return languageMap[cultureName] || 'en';
            },
            initSubjectSelect() {
                const _this = this;

                const $subjectId = $('#subjectId');
                const language = this.getSelect2Language();
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
                const $target = $('#subSubjectCode');
                const url = isClient ? '/api/app/client' : '/api/app/vendor'
                const language = this.getSelect2Language();
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
                            _this.setCompanies({ items: items });
                            return { results: results };
                        }
                    },
                    width: '100%',
                    dropdownParent: $('#subSubject'),
                    placeholder: '',
                    allowClear: true,
                    language: language
                });
                $target.on('select2:select', function (e) {
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
    const editModalTemplate = `<div><Modal  v-if="isShowModal" :value="isShowModal" @input="input" 
    @save="save" :title="l(( isDraftStatus ? editItem.id ? 'Edit' : 'New' : 'View') + 'TransferVoucher' )" :show-submit="isDraftStatus">
<div id="content">
    <div class="mb-2 row">
       <div class="col row">
            <label for="prefix" class="form-label col-sm-2 text-end">{{l('Code')}}<span> * </span></label>
            <div v-if="editItem.id" class="col-sm-10">
                 <input v-model="editItem.code" type="text" class="form-control" id="code" name="code" readonly>
            </div>
            <div v-else class="col-sm-10 row">
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
        <label for="voucherDate" class="form-label col-sm-3 text-end">{{l('VoucherDate')}}<span> * </span></label>
        <div class="col-sm-9">
            <input v-model="editItem.voucherDate" :class="{'is-invalid': errors.voucherDate }" type="date" class="form-control" id="voucherDate" name="voucherDate">
             <div id="prefixFeedback" class="invalid-feedback">
                    {{l('PleaseEnterAValue')}}
             </div>
        </div>
       </div>
    </div>
    <ul class="nav nav-tabs"  id="detailTab" role="tablist">
      <li class="nav-item" role="presentation">
        <a class="nav-link active" aria-current="page" href="#" data-bs-toggle="tab" data-bs-target="#details" role="tab" aria-controls="details" aria-selected="true">{{l('Detail')}}</a>
      </li>
    </ul>
    <div class="tab-content pt-0 pb-0" id="detailTabContent">
      <div class="tab-pane fade show active" id="details" role="tabpanel" aria-labelledby="details" tabindex="0">
        <div v-if="isDraftStatus"><button type="button" class="btn btn-primary btn-sm" @click="addDetail"><i class="fa fa-plus"></i> {{l('AddDetail')}}</button></div>
        <div class="items"> 
            <table class="table table-striped" :style="tableStyle">
                <colgroup>
                    <col v-if="isDraftStatus" style="width: 120px;" />
                    <col style="width: 250px;" />
                    <col style="width: 250px;" />
                    <col style="width: 120px;" />
                    <col style="width: 120px;" />
                    <col style="min-width:120px; max-width: 160px;" />
                    <col style="width: 120px;" />
                    <col v-if="showSubSubject" style="width: 250px;" />
                    <col v-if="showSubSubject" style="width: 120px;" />
                    <col v-if="showSubSubject" style="width: 120px;" />
                    <col v-if="enableProject" style="width: 120px;" />
                    <col v-if="enableRegion" style="width: 120px;" />
                    <col v-if="enableDepartment" style="width: 120px;" />
                    <col v-if="enableCustom1" style="width: 120px;" />
                    <col v-if="enableCustom2" style="width: 120px;" />
                </colgroup>
                <thead>
                <tr>
                    <th v-if="isDraftStatus">{{l('Actions')}}</th>
                    <th>{{l('Subject')}}</th>
                        <th>{{l('Description')}}</th>
                        <th>{{l('Debitor')}}<div>{{nativeCurrency}}</div></th>
                        <th>{{l('Creditor')}}<div>{{nativeCurrency}}</div></th>
                        <th><div>{{l('DebitorCreditor')}}</div><div>{{l('Currency')}}</div></th>
                        <th class="text-end normal"><div>{{l('ForeignAmount')}}</div><div>{{l('ExchangeRate')}}</div></th>
                        <th v-if="showSubSubject">{{l('SubSubject')}}</th>
                        <th v-if="showSubSubject">{{l('DocNo')}}</th>
                        <th v-if="showSubSubject">{{l('DueDate')}}</th>
                        <th v-if="enableProject">{{l('Project')}}</th>
                        <th v-if="enableRegion">{{l('Department')}}</th>
                        <th v-if="enableDepartment">{{l('Region')}}</th>
                        <th v-if="enableCustom1">{{l('Custom1')}}</th>
                        <th v-if="enableCustom2">{{l('Custom2')}}</th>
                </tr>
                </thead>
                <tbody>
                    <tr v-for="item in editItem.details || []">
                        <td v-if="isDraftStatus"><div class="btn-group" role="group" aria-label="Button group with nested dropdown">
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
                        <td v-if="showSubSubject">{{ item.subSubjectName }}</td>
                        <td v-if="showSubSubject">{{item.docNo}}</td>
                        <td v-if="showSubSubject">{{formatRowDate(item.dueDate)}}</td>
                        <td v-if="enableProject">{{item.project}}</td>
                        <td v-if="enableRegion">{{item.department}}</td>
                        <td v-if="enableDepartment">{{item.region}}</td>
                        <td v-if="enableCustom1">{{item.custom1}}</td>
                        <td v-if="enableCustom2">{{item.custom2}}</td>
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
      </div>
    </div>
</div> 
</Modal><EditDetail v-if="showDetailModal" v-model="isShowDetail" :item="item" @save="saveDetail" @auto-balance="autoBalance"></EditDetail></div>`;
    function getDefaultDetail() {
        return {
            subjectId: '-',
            subSubjectCode: null,
            description: '',
            debitorCreditor: 1,
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
            accountTypeCategory: 0
        };
    }
    const EditModal = {
        components: { Modal, EditDetail },
        template: editModalTemplate,
        data() {
            return {
                isShowDetail: false,
                item: {},
                errors: {},
                showDetailModal: false
            }
        },
        watch: {
            isShowDetail(value) {
                this.$nextTick(() => { this.showDetailModal = value })
            }
        },
        computed: {
            ...Vuex.mapGetters(['isShowModal', 'editItem', 'totalDebitorAmount', 'isDraftStatus',
                'totalCreditorAmount', 'subjectMap', 'companyMap', 'nativeCurrency',
                'enableProject', 'enableRegion', 'enableDepartment', 'enableCustom1', 'enableCustom2']),
            showSubSubject() {
                return this.editItem.details.filter(item => item.isSubSubjectType).length > 0
            },
            tableStyle() {
                let minWidth = 970, maxWidth = 1140;
                const features = [this.enableProject, this.enableRegion, this.enableDepartment, this.enableCustom1, this.enableCustom2]
                const featureColumnWidth = 120, subSubbjectWidth = 490;
                features.forEach(enable => {
                    if (enable) {
                        minWidth += featureColumnWidth;
                        maxWidth += featureColumnWidth;
                    }
                })
                if (this.showSubSubject) {
                    minWidth += subSubbjectWidth;
                    maxWidth += subSubbjectWidth;
                }

                return {
                    ['min-width']: minWidth.toString() + 'px',
                    ['max-width']: maxWidth.toString() + 'px'
                }
            }
        },
        methods: {
            ...Vuex.mapMutations(['showModal', 'saveDetailItem', 'removeDetailItem']),
            input(value) {
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
                const request = this.editItem.id ? accounting.finance.transferVoucher.update(this.editItem.id, data) :
                    accounting.finance.transferVoucher.create(data);
                request.then(() => {
                    abp.ui.clearBusy('#content');
                    abp.notify.success(this.l('SavedSuccessfully'));
                    this.showModal({ isShowModal: false });
                    $('#voucherTable').DataTable().ajax.reload();
                }).catch(() => {
                    abp.ui.clearBusy('#content');
                });
            },
            renderAmount(amount, scale) {
                const num = Number(amount);
                return !Number.isNaN(num) ? num.toFixed(scale ? scale : 2) : '0.00';
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
            l(key) {
                return getLocal(key);
            },
            formatInputDate(value) {
                return (new moment(value)).format("yyyy-MM-DD")
            },
            formatRowDate(value) {
                return value ? new Date(value).toLocaleDateString() : ''
            },
            autoBalance() {
                const item = this.item;
                const currencyRate = Number(item.currencyRate);
                if (!currencyRate) {
                    return
                }
                let balanceAmount = this.totalDebitorAmount - this.totalCreditorAmount
                if (item.nativeAmount) {
                    if (item.debitorCreditor === 1) {
                        balanceAmount -= Number(item.nativeAmount);
                    } else {
                        balanceAmount += Number(item.nativeAmount);
                    }
                }
                const debitorCreditor = balanceAmount > 0 ? -1 : 1;
                item.nativeAmount = Math.abs(balanceAmount);
                item.foreignAmount = Number((item.nativeAmount / currencyRate).toFixed(2))
                item.debitorCreditor = debitorCreditor;
            }
        }
    }

    const App = {
        components: { EditModal },
        template: `<div><EditModal /></div>`
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
