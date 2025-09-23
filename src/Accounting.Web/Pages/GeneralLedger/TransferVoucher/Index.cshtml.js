
$(function () {
    const l = abp.localization.getResource('Accounting');
    const isGrantedEdit = abp.auth.isGranted('Accounting.GeneralLedger.TransferVoucher.Edit');
    const isGrantedDelete = abp.auth.isGranted('Accounting.GeneralLedger.TransferVoucher.Deletion');
    // 创建一个新的 store 实例 
    function formatDate(value) {
        return (new moment(value)).format("yyyy-MM-DD")
    }
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
                currencies: [],
                nativeCurrency: '',
                isRequestData: false
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
                state.editItem = payload.item || { details: [] };
                state.editItem.voucherDate = formatDate(state.editItem.voucherDate);

                if (state.subjects.length > 0) {
                    (state.editItem.details || []).forEach(d => {
                        if (d.subjectId) {
                            const subject = state.subjectMap[d.subjectId];
                            if (subject) {
                                d.isSubSubjectType = subject.isSubSubjectType;
                                d.accountTypeCode = subject.accountTypeCode;
                            }
                        }
                    });
                }
            },
            setSubjects(state, payload) {
                state.subjects = payload.subjects || [];
                state.subjectMap = {};
                (state.subjects).forEach(s => {
                    state.subjectMap[s.id] = s;
                });
            },
            setCompanies(state, payload) {
                const { companies, clients, vendors } = payload
                state.companies = companies || [];
                state.clients = clients || [];
                state.vendors = vendors || [];

                state.companyMap = {};
                (state.companies).forEach(c => {
                    state.companyMap[c.id] = c;
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
            }
        },
        getters: {
            isShowModal: state => state.isShowModal,
            editItem: state => state.editItem,
            totalDebitorAmount: state => {
                return (state.editItem.details || []).reduce((init, item) => init + (item.debitorCreditor === 1 ? Number(item.nativeAmount) : 0), 0)
            },
            totalCreditorAmount: state => {
                return (state.editItem.details || []).reduce((init, item) => init + (item.debitorCreditor === -1 ? Number(item.nativeAmount) : 0 ), 0)
            },
            subjects: state => state.subjects,
            subjectMap: state => state.subjectMap,
            companies: state => state.companies,
            companyMap: state => state.companyMap,
            clients: state => state.clients,
            vendors: state => state.vendors,
            currencies: state => state.currencies,
            nativeCurrency: state => state.nativeCurrency,
            isRequestData: state => state.isRequestData
        }
    }) 
    const tvInputAction = function (requestData, dataTableSettings) {
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
    const editHandle = function (id) {
        const requests = [];
        if (id) {
            requests.push(accounting.finance.transferVoucher.get(id));
        } else {
            requests.push(new Promise(resolve => resolve({ voucherDate: new Date(), prefix: 'JV', genNo: 0, details: [] })));
        }

        if (!store.getters.isRequestData) {
            requests.push(accounting.finance.subject.getVoucherSimpleList().then(result => store.commit('setSubjects', { subjects: result })));
            requests.push(accounting.basicData.client.getList({ maxResultCount: 1000 }));
            requests.push(accounting.basicData.vendor.getList({ maxResultCount: 1000 }));
            requests.push(accounting.basicData.currency.getActiveList().then(result => store.commit('setCurrencies', { currencies: result })));
            requests.push(accounting.finance.accountingSetting.getNativeCurrency().then(result => store.commit('setNativeCurrency', result)));
        }
       
        store.commit('showModal', { isShowModal: true });
        Promise.all(requests).then(results => {
            const item = results[0];
            store.commit('setEditItem', { item });
             
            if (!store.getters.isRequestData) {
                const clientList = results[2].items || [];
                const vendorList = results[3].items || [];
                const companyList = clientList.concat(vendorList);
                store.commit('setCompanies', { companies: companyList, clients: clientList, vendors: vendorList });
                store.commit('setIsRequestData', { isRequestData: true });
            } 
        });
    }
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
                                    visible: isGrantedEdit
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
            ]
        })
    );

    $(document).on('keydown', '#searchForm', function (e) {
        if (e.which !== 13) {
            return false;
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


    function getLocal(key) {
        return abp.localization.getResource('Accounting')(key);
    }
    const modalTemplate = `
<form ref="modal" class="needs-validation" novalidate>
    <div :class="[value ? 'show d-block' : '']" role="dialog" aria-modal="true" class="modal fade" tabindex="-1"  style="background:rgba(157, 159, 160, 0.8);">
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
            <button type="button" class="btn btn-outline-primary" data-bs-dismiss="modal" @click="close">{{l('Cancel')}}</button>
            <button type="submit" class="btn btn-primary" @click="save"><i class="fa fa-check"></i> {{l('Save')}}</button>
          </div>
        </div>
      </div>
    </div>
</form>`;
    const Modal = {
        template: modalTemplate,
        props: ['value', 'title'],
        mounted() {
            this.$nextTick(() => {
                document.body.classList.add('modal-open');
                document.body.appendChild(this.$refs.modal);
            })
        },
        beforeDestroy() {
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
<Modal :value="value" @input="input" @save="save" :title="l('Detail')">
    <div>
       <div style="display: grid; grid-template-columns: 1fr 1fr;">
             <div class="mb-2 mx-1">
                <label for="subjectId" class="form-label">{{l('Subject')}}<span> * </span></label>
                <select v-model="item.subjectId" @change="subjectChange" :class="{'is-invalid': errors.subjectId }" class="form-control" id="subjectId" name="subjectId">
                    <option value="">--</option>
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
             <div v-if="item.isSubSubjectType" class="mb-2 mx-1">
                <label for="subSubjectCode" :class="{'is-invalid': errors.subSubjectCode }" class="form-label">{{l('SubSubject')}}<span> * </span></label>
                <select v-if="item.accountTypeCode ==='AR'" v-model="item.subSubjectCode" @change="arapFieldChange"  key="ar" class="form-control" id="subSubjectCode" name="subSubjectCode">
                    <option value="">--</option>
                    <option v-for="subItem in clients || []" :key="subItem.id" :value="subItem.id">{{subItem.code + ' - '+ subItem.name }}</option>
                 </select>
                 <select v-else v-model="item.subSubjectCode" @change="arapFieldChange" key="ap" class="form-control" id="subSubjectCode" name="subSubjectCode">
                    <option value="">--</option>
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
             <div class="mb-2 mx-1">
                <label for="project" class="form-label">{{l('Project')}}</label>
                <input v-model="item.project" type="text" class="form-control" id="project" name="project">
            </div>
            <div class="mb-2 mx-1">
                <label for="department" class="form-label">{{l('Department')}}</label>
                <input v-model="item.department" type="text" class="form-control" id="department" rows="3" name="department">
            </div>
             <div class="mb-2 mx-1">
                <label for="region" class="form-label">{{l('Region')}}</label>
                <input v-model="item.region" type="text" class="form-control" id="region" name="region">
            </div>
            <div class="mb-2 mx-1">
                <label for="custom1" class="form-label">{{l('Custom1')}}</label>
                <input v-model="item.custom1" type="text" class="form-control" id="custom1" rows="3" name="custom1">
            </div>
             <div class="mb-2 mx-1">
                <label for="custom2" class="form-label">{{l('Custom2')}}</label>
                <input v-model="item.custom2" type="text" class="form-control" id="custom2" name="custom2">
            </div>
        </div> 
    </div>
</Modal></div>`;
    const EditDetail = {
        components: { Modal },
        template: editDetailTemplate,
        props: ['item', 'value'],
        data() {
            return {
                debitorCreditors: [{ value: 1, text: getLocal('Debitor') }, { value: -1, text: getLocal('Creditor') }],
                errors: {}
            }
        },
        computed: {
            ...Vuex.mapGetters(['subjects', 'companies', 'currencies', 'subjectMap', 'clients', 'vendors'])
        },
        methods: {
            input(value) {
                this.$emit('input', value);
            },
            validate() {
                const { subjectId, currencyCode, currencyRate, foreignAmount, debitorCreditor,
                    isSubSubjectType, subSubjectCode, docNo, dueDate } = this.item
                let errorCount = 0;
                this.errors = {};
                if (!subjectId) {
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
                    if (!subSubjectCode) {
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
                    this.item.foreignAmount = this.item.foreignAmount.trim();
                }
                if (this.item.currencyRate) {
                    this.item.currencyRate = this.item.currencyRate.trim();
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
                    const { isSubSubjectType, accountTypeCode, debitorCreditor, currencyCode } = subject;
                    this.item.isSubSubjectType = isSubSubjectType;
                    this.item.accountTypeCode = accountTypeCode;
                    this.item.debitorCreditor = debitorCreditor;
                    this.errors.subjectId = false;
                    if (currencyCode) {
                        this.item.currencyCode = currencyCode;
                        this.currencyChange();
                    }
                    this.setNativeAmount();
                }
                this.item.subSubjectCode = '';
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
            }
        }
    }
    const editModalTemplate = `<div><Modal :value="isShowModal" @input="input" @save="save" :title="l(editItem.id ? 'EditTransferVoucher' : 'NewTransferVoucher' )">
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
        <div><button type="button" class="btn btn-primary btn-sm" @click="addDetail"><i class="fa fa-plus"></i> {{l('AddDetail')}}</button></div>
        <div class="items"> 
            <table class="table table-striped" style="min-width: 2060px;max-width: 2230px;">
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
                    <col style="width: 120px;" />
                    <col style="width: 120px;" />
                    <col style="width: 120px;" />
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
                        <th>{{l('Project')}}</th>
                        <th>{{l('Department')}}</th>
                        <th>{{l('Region')}}</th>
                        <th>{{l('Custom1')}}</th>
                        <th>{{l('Custom2')}}</th>
                </tr>
                </thead>
                <tbody>
                    <tr v-for="item in editItem.details || []">
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
                        <td>{{ subjectMap[item.subjectId] ? (subjectMap[item.subjectId].code + ' - '+ subjectMap[item.subjectId].name) : item.subjectId }}</td>
                        <td>{{item.description}}</td>
                        <td>{{item.debitorCreditor === 1 ? renderAmount(item.nativeAmount) : ''}}</td>
                        <td>{{item.debitorCreditor === -1 ? renderAmount(item.nativeAmount) : ''}}</td>
                        <td><div>{{item.debitorCreditor === 1 ? l('Debitor'): l('Creditor')}}</div><div>{{item.currencyCode}}</div></td>
                        <td class="text-end"><div>{{renderAmount(item.foreignAmount)}}</div><div>{{renderAmount(item.currencyRate, 7)}}</div></td>
                        <td>{{item.subSubjectCode && companyMap[item.subSubjectCode] ? (companyMap[item.subSubjectCode].code + ' - '+ companyMap[item.subSubjectCode].name) : item.subSubjectCode}}</td>
                        <td>{{item.docNo}}</td>
                        <td>{{formatRowDate(item.dueDate)}}</td>
                        <td>{{item.project}}</td>
                        <td>{{item.department}}</td>
                        <td>{{item.region}}</td>
                        <td>{{item.custom1}}</td>
                        <td>{{item.custom2}}</td>
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
</Modal><EditDetail v-if="isShowDetail" v-model="isShowDetail" :item="item" @save="saveDetail"></EditDetail></div>`;
    function getDefaultDetail() {
        return {
            subjectId: null,
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
            isSubSubjectType: false
        };
    }
    const EditModal = {
        components: { Modal, EditDetail },
        template: editModalTemplate,
        data() {
            return {
                isShowDetail: false,
                item: {},
                errors: {}
            }
        },
        computed: {
            ...Vuex.mapGetters(['isShowModal', 'editItem', 'totalDebitorAmount',
                'totalCreditorAmount', 'subjectMap', 'companyMap', 'nativeCurrency'])
        },
        methods: {
            ...Vuex.mapMutations(['showModal']),
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
                const index = (this.editItem.details || []).findIndex(obj => obj === item);
                if (index >= 0) {
                    this.editItem.details.splice(index, 1);
                }
                this.isShowDetail = false;
            },
            saveDetail() {
                const index = (this.editItem.details || []).findIndex(obj => obj === this.item);
                if (index < 0) {
                    this.editItem.details = [...this.editItem.details, this.item];
                } else {
                    this.editItem.details = [...this.editItem.details];
                }
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
                return value ? new Date(value).toLocaleDateString(): ''
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
