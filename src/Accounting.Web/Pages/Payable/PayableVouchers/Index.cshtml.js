$(function () {
    const l = abp.localization.getResource('Accounting');
    const isGrantedEdit = abp.auth.isGranted('Accounting.Payable.PayableVoucher.Edit');
    const isGrantedDelete = abp.auth.isGranted('Accounting.Payable.PayableVoucher.Deletion');
    const voucherRequests = accounting.finance.payableVoucher;
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

    const getEmptyReceipts = () => ({ items: [], totalCount: 0, currentPage: 0 });
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
        debitorCreditor: debitCredit.debitor,
        paymentReference: '',
        isSelected: false,
        rowid: getRowId()
    });

    const voucherStatus = {
        draft: 0,
        approval: 1,
        void: 2
    }
    const draftStatus = voucherStatus.draft;

    const { createApp, ref, computed, onMounted, watch, nextTick, useTemplateRef, onBeforeUnmount, markRaw } = Vue;
    const { defineStore, storeToRefs, createPinia } = Pinia;

    const useCompanyStore = defineStore('company', () => {
        const companyMap = ref({})
        const clientMap = ref({})
        const vendorMap = ref({})
        const companies = ref([])
        const clients = ref([])
        const vendors = ref([])

        const setCompanies = (payload) => {
            const { items } = payload;
            (items || []).forEach(c => {
                companyMap.value[c.id] = c;
                if (!clientMap.value[c.id] && c.isClient) {
                    clientMap.value[c.id] = markRaw(c);
                    clients.value.push(c)
                }
                if (!vendorMap.value[c.id] && c.isVendor) {
                    vendorMap.value[c.id] = markRaw(c);
                    vendors.value.push(c)
                }
            });
        }

        return { companyMap, clientMap, vendorMap, companies, clients, vendors, setCompanies }
    });

    const useSubjectStore = defineStore('subject', () => {
        const subjectMap = ref({})
        const subjects = ref([])

        const setSubjects = (payload) => {
            const items = payload.subjects || [];
            items.forEach(item => {
                if (!subjectMap.value[item.id]) {
                    subjectMap.value[item.id] = markRaw(item);
                    subjects.value.push(item);
                }
            });
        }

        return { subjects, subjectMap, setSubjects }
    });
    const useCurrencyStore = defineStore('currency', () => {
        const currencies = ref([])
        const nativeCurrency = ref('')

        const setCurrencies = (payload) => {
            currencies.value = payload.currencies || [];
        }
        const setNativeCurrency = (payload) => {
            nativeCurrency.value = payload;
        }

        return { currencies, nativeCurrency, setCurrencies, setNativeCurrency }
    });

    const prefix = 'PV'
    const getVoucher = () => {
        return {
            id: '', code: '', prefix: prefix, genNo: 0,
            status: voucherStatus.draft,
            voucherDate: new Date(), debitorId: '', details: []
        }
    }
    const useVoucherStore = defineStore('voucher', () => {
        const editItem = ref(getVoucher())
        const payments = ref([])
        const receipts = ref(getEmptyReceipts())
        const paymentMethods = ref([])

        const subjectStore = useSubjectStore();
        const companyStore = useCompanyStore();
        const currencyStore = useCurrencyStore();

        const setEditItem = (payload) => {
            const { details, ...others } = payload.item || getVoucher();
            const isClearData = payload.isClearData;
            let debitorId = '';
            const newDetails = details.map(item => {
                if (item.subSubjectCode && !debitorId) {
                    debitorId = item.subSubjectCode;
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
            editItem.value = { ...others, details: newDetails, debitorId };
            editItem.value.voucherDate = formatDate(editItem.value.voucherDate);
            if (isClearData !== false) { 
                payments.value = [];
                receipts.value = getEmptyReceipts();
            }
        }
        const saveDetailItem = (payload) => {
            const { item } = payload;
            const index = editItem.value.details.findIndex(obj => obj == item);
            if (index < 0) {
                editItem.value.details.push(item)
            } else {
                editItem.value.details = [...editItem.value.details];
            }
        }
        const removeDetailItem = (payload) => {
            const index = (editItem.value.details || []).findIndex(obj => obj === payload.item);
            if (index >= 0) {
                editItem.value.details.splice(index, 1);
            }
        }
        const setDetails = ({ isClearData }) => {
            if (isClearData !== false) {
                payments.value = [];
            }
            (editItem.value.details || []).forEach(item => {
                const subject = subjectStore.subjectMap[item.subjectId];
                if (subject) {
                    const { code, name, isSubSubjectType, accountType } = subject;
                    const category = accountType ? accountType.category : 0;
                    item.isSubSubjectType = isSubSubjectType;
                    item.accountTypeCategory = category;
                    item.subjectName = code + ' - ' + name;
                    if (isClearData !== false && category != accountTypes.receivable && category != accountTypes.payable) {
                        payments.value.push({ ...item });
                    }
                }
                const company = companyStore.companyMap[item.subSubjectCode];
                if (company) {
                    const { code, name, } = company;
                    item.subSubjectName = code + ' - ' + name;
                }
            });
        }
        const setReceipts = (payload) => {
            receipts.value = payload || getEmptyReceipts();
        }
        const addPaymentItem = () => {
            payments.value.push(getPaymentItem(currencyStore.nativeCurrency.value));
        }
        const setPayments = (payload) => {
            payments.value = (payload || []).map(item => ({ ...item, rowid: getRowId() }));
        }
        const removePaymentItem = (payload) => {
            const index = payments.value.findIndex(obj => obj === payload.item);
            if (index >= 0) {
                payments.value.splice(index, 1);
            }
        }
        const setPaymentMethods = (payload) => {
            paymentMethods.value = payload || [];
            subjectStore.setSubjects(paymentMethods.value);
        }

        const clearData = () => {
            payments.value = [];
            addPaymentItem();
            receipts.value = getEmptyReceipts();
        }
        const totalDebitorAmount = computed(() => {
            return (editItem.value.details || []).reduce((init, item) =>
                init + (item.debitorCreditor === debitCredit.debitor ?
                    Number(item.nativeAmount) : 0), 0)
        });
        const totalCreditorAmount = computed(() => {
            return (editItem.value.details || []).reduce((init, item) =>
                init + (item.debitorCreditor === debitCredit.creditor ?
                    Number(item.nativeAmount) : 0), 0)
        });
        const isDraftStatus = computed(() => !editItem.value.id || editItem.value.id && editItem.value.status === draftStatus);
        const totalPaymentAmount = computed(() => {
            return payments.value.reduce((init, item) =>
                init + Number(item.nativeAmount), 0);
        });
        const totalReceiptAmount = computed(() => {
            return receipts.value.items.reduce((init, item) =>
                init + Number(item.nativeCurrentPaid), 0);
        });

        return {
            editItem, totalDebitorAmount, totalCreditorAmount, isDraftStatus,
            payments, receipts, paymentMethods, totalPaymentAmount, totalReceiptAmount,
            setEditItem, saveDetailItem, removeDetailItem, setDetails, setReceipts,
            addPaymentItem, setPayments, setPayments, removePaymentItem,
            removePaymentItem, setPaymentMethods, clearData
        }
    });

    const useEditModalStore = defineStore('editModal', () => {
        const isShowModal = ref(false)
        const isRequestData = ref(false)
        const isEdit = ref(false)
        const voucherStore = useVoucherStore();

        const showModal = (payload) => {
            isShowModal.value = payload.isShowModal;
            voucherStore.editItem.id = payload.id || ''
            voucherStore.editItem.debitorId = ''
        }
        const setIsEdit = (payload) => {
            isEdit.value = payload.isEdit;
        }

        const setIsRequestData = (payload) => {
            isRequestData.value = payload;
        }
        return {
            isShowModal, isRequestData, isEdit, showModal, setIsEdit, setIsRequestData
        }
    });

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
    function initCompanySelect(isClient, targetSelector, dropdownParent, selectEvent, setCompanies) {
        const $target = $(targetSelector);
        const url = isClient ? '/api/app/client' : '/api/app/vendor'
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
    <div ref="modal" :class="[value ? 'show d-block' : '']" :id="modalId" role="dialog" aria-modal="true" class="modal fade" tabindex="-1"  style="background:rgba(108, 108, 108, 0.65);">
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
        emits: ['input', 'save'],
        setup(props, { emit }) {
            const modalRef = useTemplateRef('modal');
            const formRef = useTemplateRef('form');

            const close = () => {
                emit('input', !props.value);
            };

            const save = (e) => {
                e.preventDefault();
                emit('save');
            };

            onMounted(() => {
                nextTick(() => {
                    document.body.classList.add('modal-open');
                    document.body.classList.add('overflow-hidden');
                    document.body.style.paddingRight = '15px';
                    document.body.appendChild(formRef.value);
                    setZIndex(modalRef.value)
                })
            });

            onBeforeUnmount(() => {
                document.body.classList.remove('modal-open');
                document.body.classList.remove('overflow-hidden');
                document.body.style.paddingRight = null;
                document.body.removeChild(formRef.value);
            });

            return {
                modalRef,
                formRef,
                close,
                save,
                l,
            }
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
        emits: ['change-page'],
        setup(props, { emit }) {
            const pages = ref([]);
            const maxPage = ref(0);

            const emitChangePage = (newPage) => {
                if (newPage == props.currentPage) {
                    return
                }
                emit('change-page', newPage);
            }

            const changePage = (newPage) => {
                emitChangePage(newPage);
            }
            const previousPage = () => {
                const newPage = Math.max(props.currentPage - 1, 1);
                emitChangePage(newPage);
            }
            const nextPage = () => {
                const newPage = Math.min(props.currentPage + 1, maxPage.value);
                emitChangePage(newPage);
            }

            const generatePage = () => {
                const currentPage = props.currentPage;
                const maxPage = Math.ceil(props.totalCount / maxResultCount);
                maxPage.value = maxPage;
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
                pages.value = [];
                for (let i = start; i <= end; i++) {
                    pages.value.push(i);
                }
            }

            watch(() => props.currentPage, () => {
                generatePage()
            }, { immediate: true });

            watch(() => props.totalCount, () => {
                generatePage()
            }, { immediate: true });

            return {
                pages,
                maxPage,
                changePage,
                previousPage,
                nextPage
            };
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
                     <col v-if="isDraftStatus"/>
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
                    <th v-if="isDraftStatus">{{l('Actions')}}</th>
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
                        <td v-if="isDraftStatus">
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
                        <button v-if="isDraftStatus" type="button" class="btn btn-primary btn-sm" @click="()=> addPaymentItem()"><i class="fa-solid fa-plus"></i></button>
                    </td>
                    </tr>
                </tfoot>
            </table> 
    </div>`;
    const Payments = {
        template: paymentsTemplate,
        setup() {
            const currencyStore = useCurrencyStore();
            const { currencies, nativeCurrency } = storeToRefs(currencyStore);

            const subjectStore = useSubjectStore();
            const { subjects, subjectMap } = storeToRefs(subjectStore);
            const { setSubjects } = subjectStore

            const voucherStore = useVoucherStore();
            const { payments, paymentMethods, receipts, totalPaymentAmount,
                isDraftStatus } = storeToRefs(voucherStore);
            const { addPaymentItem, removePaymentItem } = voucherStore

            const setNativeAmount = (item) => {
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
            }
            const currencyChange = (item) => {
                const currency = currencies.value.find(c => c.targetCurrency === item.currencyCode);
                if (currency) {
                    item.currencyRate = renderAmount(currency.exchangeRate, 7);;
                    setNativeAmount(item);
                }
            }
            const subjectChange = (item) => {
                const subject = subjectMap.value[item.subjectId];
                if (subject) {
                    const { debitorCreditor, currencyCode } = subject;
                    item.debitorCreditor = debitorCreditor;

                    item.currencyCode = currencyCode || nativeCurrency;
                    if (item.currencyCode) {
                        currencyChange(item);
                    }
                    setNativeAmount(item);
                }
            }
            onMounted(() => {
                if (payments.value.length === 0) {
                    addPaymentItem();
                }
            });

            return {
                currencies,
                nativeCurrency,
                subjects,
                subjectMap,
                payments,
                paymentMethods,
                receipts,
                totalPaymentAmount,
                isDraftStatus,
                setSubjects,
                addPaymentItem,
                removePaymentItem,
                debitorCreditors: [
                    { value: debitCredit.debitor, text: '+' },
                    { value: debitCredit.creditor, text: '-' }],
                renderAmount,
                l,
                setNativeAmount,
                currencyChange,
                subjectChange
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
                 <col v-if="isDraftStatus"/>
             </colgroup>
                <thead>
                <tr>
                    <th>&nbsp;</th>
                    <th>{{l('Date')}}</th>
                    <th>{{l('DocNo')}}</th>
                    <th>{{l('Currency')}}</th>
                    <th>{{l('ExchangeRate')}}</th>
                    <th class="text-end normal">{{l('ForeignAmount')}}</th>
                    <th>{{ l('ApRvDeposit') }}</th>
                    <th>{{ l('ArPvDeposit') }}</th>
                    <th>{{ l('PaymentAmount') }}</th>
                    <th>{{nativeCurrency}}</th>
                    <th v-if="isDraftStatus">{{l('Actions')}}</th>
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
                        <td>{{ item.debitorCreditor === debitCredit.creditor ? renderAmount(item.osAmount) : '0.00' }}</td>
                        <td>{{ item.debitorCreditor === debitCredit.debitor ? renderAmount(item.osAmount) : '0.00' }}</td>
                        <td>
                            <input v-model="item.currentPaid" @change="()=> currentPaidChanged(item)" type="text" class="form-control" name="currentPaid">
                        </td>
                        <td>{{ renderAmount(item.nativeCurrentPaid) }}</td>
                        <td v-if="isDraftStatus">
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
                    <td :colspan="isDraftStatus ? 2 : 1">{{renderAmount(totalReceiptAmount)}}</td>
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
        setup(props, { emit }) {
            const currencyStore = useCurrencyStore();
            const { nativeCurrency } = storeToRefs(currencyStore);

            const voucherStore = useVoucherStore();
            const { receipts, totalReceiptAmount, isDraftStatus } = storeToRefs(voucherStore);

            const fullPay = (item) => {
                const { currencyRate, foreignAmount } = item;
                const foreignAmt = Number(foreignAmount);
                const rate = Number(currencyRate);
                const amount = Math.round(foreignAmt * rate, 2);
                item.currentPaid = foreignAmt;
                item.nativeCurrentPaid = amount;
            }
            const unpaid = (item) => {
                item.currentPaid = 0;
                item.nativeCurrentPaid = 0;
            }
            const currentPaidChanged = (item) => {
                changeCurrentPaid(item);
            }
            const changePage = (newPage) => {
                emit('change-page', newPage);
            }
            const getItemType = ({ accTypeCategory, debitorCreditor }) => {
                if (accTypeCategory === accountTypes.receivable) {
                    //应收/预付
                    return debitorCreditor == debitCredit.debitor ? l('ReceivableTxt') : l('Advances');
                }
                else {
                    //应付/预收
                    return debitorCreditor == debitCredit.creditor ? l('PayableTxt') : l('Prepayment');
                }
            }

            const totalNativeAmount = computed(() => {
                return (receipts.value.items || []).reduce((init, item) =>
                    init + Number(item.nativeAmount), 0);
            })

            return {
                receipts,
                totalReceiptAmount,
                isDraftStatus,
                totalNativeAmount,
                nativeCurrency,
                debitCredit,
                renderAmount,
                l,
                formatRowDate,
                fullPay,
                unpaid,
                currentPaidChanged,
                changePage,
                getItemType
            }
        }
    };
    const payableDetailTemplate = `<div>
     <div v-if="isDraftStatus" class="mb-2">
        <button type="button" class="btn btn-primary btn-sm me-1" @click="autoBalance">{{l('AutoBalance')}}</button>
        <button type="button" class="btn btn-primary btn-sm me-1" @click="defaultPay">{{l('DefaultPay')}}</button>
        <button type="button" class="btn btn-primary btn-sm me-1" @click="fullPay">{{l('FullPay')}}</button>
        <button type="button" class="btn btn-primary btn-sm" @click="generateDetails">{{l('GenerateVoucher')}}</button>
    </div>
      <Payments />
      <Receipts  @change-page="changePage"/>
    </div>`
    const PayableDetail = {
        components: { Payments, Receipts },
        template: payableDetailTemplate,
        emits: ['change-page'],
        setup(props, { emit }) {
            const voucherStore = useVoucherStore();
            const { editItem, payments, totalReceiptAmount, receipts, totalPaymentAmount,
                isDraftStatus } = storeToRefs(voucherStore);
            const { setEditItem, setDetails } = voucherStore

            const changePage = (newPage) => {
                emit('change-page', newPage);
            }
            const autoBalance = () => {
                const item = payments.value.find(obj => obj.isSelected);
                if (!item) {
                    abp.message.info(l('PleaseChoicePaymentItem'));
                    return;
                }
                const currencyRate = Number(item.currencyRate);
                if (!currencyRate) {
                    return
                }
                let balanceAmount = totalReceiptAmount.value - totalPaymentAmount.value
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
            }
            const defaultPay = () => {
                let balanceAmount = totalPaymentAmount.value - totalReceiptAmount.value
                if (balanceAmount <= 0) {
                    return;
                }
                for (let i = 0; i < receipts.value.items.length; i++) {
                    const item = receipts.value.items[i];
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
            }
            const fullPay = () => {
                receipts.value.items.forEach(item => {
                    item.currentPaid = item.foreignAmount;
                    changeCurrentPaid(item);
                })
            }
            const generateDetails = () => {
                const receiptItems = receipts.value.items.filter(item => item.currentPaid > 0);
                const paymentItems = payments.value.filter(item =>
                    item.foreignAmount > 0
                    && item.subjectId
                    && item.currencyCode
                    && item.currencyRate > 0
                );
                const debitorId = editItem.value.debitorId;
                if (!debitorId) {
                    abp.message.info(this.l('PleaseEnterDebitor'));
                    return
                }
                if (paymentItems.length === 0) {
                    abp.message.info(this.l('PleaseEnterPaymentItemInfo'));
                    return
                }
                const param = {
                    debitorId,
                    receipts: receiptItems,
                    payments: paymentItems
                };
                voucherRequests.generateDetails
                    (param).then(result => {
                        setEditItem({ item: { ...editItem.value, details: result || [] }, isClearData: false });
                        setDetails({ isClearData: false });
                    }).catch(() => {
                        setEditItem({ item: { ...editItem.value, details: [] }, isClearData: false });
                    })
            }

            return {
                receipts,
                payments,
                totalReceiptAmount,
                totalPaymentAmount,
                editItem,
                isDraftStatus,
                setEditItem,
                setDetails,
                l,
                changePage,
                autoBalance,
                defaultPay,
                fullPay,
                generateDetails
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
            <div class="mb-2 mx-1">
                <label for="paymentReference" class="form-label">{{l('PaymentReference')}}</label>
                <input v-model="item.paymentReference" type="text" class="form-control" id="paymentReference" name="paymentReference">
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
        emits: ['input', 'save', 'auto-balance'],
        setup(props, { emit }) {
            const isShow = ref(false);
            const errors = ref({});

            const currencyStore = useCurrencyStore();
            const { currencies, nativeCurrency } = storeToRefs(currencyStore);

            const subjectStore = useSubjectStore();
            const { subjects, subjectMap } = storeToRefs(subjectStore);
            const { setSubjects } = subjectStore

            const companyStore = useCompanyStore();
            const { clients, vendors, companyMap } = storeToRefs(companyStore);
            const { setCompanies } = companyStore;

            const input = (value) => {
                emit('input', value);
            }
            const validate = () => {
                const { subjectId, currencyCode, currencyRate, foreignAmount, debitorCreditor,
                    isSubSubjectType, subSubjectCode, docNo, dueDate } = props.item
                let errorCount = 0;
                errors.value = {};
                if (!subjectId || subjectId === '-') {
                    errors.value.subjectId = true;
                    errorCount++;
                }
                if (!currencyCode) {
                    errors.value.currencyCode = true;
                    errorCount++;
                }
                if (!currencyRate || isNaN(Number(currencyRate)) || Number(currencyRate) <= 0) {
                    errors.value.currencyRate = true;
                    errorCount++;
                }
                if (!foreignAmount || isNaN(Number(foreignAmount)) || Number(foreignAmount) <= 0) {
                    errors.value.foreignAmount = true;
                    errorCount++;
                }
                if (!debitorCreditor) {
                    errors.value.debitorCreditor = true;
                    errorCount++;
                }
                if (isSubSubjectType) {
                    if (!subSubjectCode || subSubjectCode === '-') {
                        errors.value.subSubjectCode = true;
                        errorCount++;
                    }
                    if (!docNo) {
                        errors.value.docNo = true;
                        errorCount++;
                    }
                }
                return errorCount === 0;
            };
            const save = () => {
                if (!validate()) {
                    return;
                }
                emit('save')
            };

            const setNativeAmount = () => {
                const item = props.item;
                if (item.foreignAmount) {
                    item.foreignAmount = item.foreignAmount.toString().trim();
                }
                if (item.currencyRate) {
                    item.currencyRate = item.currencyRate.toString().trim();
                }
                const foreignAmount = Number(item.foreignAmount);
                const currencyRate = Number(item.currencyRate);
                const amount = foreignAmount * currencyRate;
                if (!isNaN(foreignAmount) && foreignAmount > 0) {
                    errors.value.foreignAmount = false;
                }
                if (!isNaN(currencyRate) && currencyRate > 0) {
                    errors.value.currencyRate = false;
                }
                item.nativeAmount = renderAmount(amount)
            };

            const currencyChange = () => {
                let currency = currencies.value.find(c => c.targetCurrency === props.item.currencyCode);
                if (!currency) {
                    currency = currencies.value.find(c => c.targetCurrency === nativeCurrency.value);
                }
                if (currency) {
                    errors.value.currencyCode = false;
                    props.item.currencyRate = renderAmount(currency.exchangeRate, 7);
                    setNativeAmount();
                }
            }
            const initItemCompanySelect = (isClient) => {
                initCompanySelect(isClient, '#subSubjectCode', '#subSubject',
                    e => {
                        props.item.subSubjectCode = e.params.data.id;
                        props.item.subSubjectName = e.params.data.text;
                        const company = companyMap.value[e.params.data.id];
                        if (company && company.currency) {
                            props.item.currencyCode = company.currency;
                            currencyChange();
                        }
                    }, setCompanies);
            };

            const subjectChange = () => {
                const item = props.item;
                const subject = subjectMap.value[item.subjectId];
                item.isSubSubjectType = false;
                if (subject) {
                    const { isSubSubjectType, accountType, debitorCreditor, currencyCode, name, code } = subject;
                    item.isSubSubjectType = isSubSubjectType;
                    item.accountTypeCategory = accountType ? accountType.category : 0;
                    item.debitorCreditor = debitorCreditor;
                    item.subjectName = code + ' - ' + name;
                    errors.value.subjectId = false;
                    item.currencyCode = currencyCode || nativeCurrency.value;
                    currencyChange();
                    setNativeAmount();
                    if (item.isSubSubjectType) {
                        nextTick(() => initItemCompanySelect(item.accountTypeCategory == accountTypes.receivable))
                    }
                }
                item.subSubjectCode = '-';
                item.docNo = '';
                item.dueDate = null;
            }
            const arapFieldChange = () => {
                const { isSubSubjectType, subSubjectCode, docNo, dueDate } = props.item
                if (isSubSubjectType) {
                    if (subSubjectCode) {
                        errors.value.subSubjectCode = false;
                    }
                    if (docNo) {
                        errors.value.docNo = false;
                    }
                }
            };

            const initSubjectSelect = () => {
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
                            setSubjects({ subjects: items });
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
                    props.item.subjectId = e.params.data.id;
                    subjectChange();
                });
            }

            const autoBalance = () => {
                emit('auto-balance')
            }

            watch(() => props.value, (newValue) => {
                isShow.value = newValue
            }, { immediate: true });

            onMounted(() => {
                initSubjectSelect()
                if (props.item.isSubSubjectType) {
                    nextTick(() => initItemCompanySelect(props.item.accountTypeCategory == accountTypes.receivable))
                }
            });

            onBeforeUnmount(() => {
                $('#subjectId').off('select2:select');
            });

            return {
                debitorCreditors: [
                    { value: debitCredit.debitor, text: l('Debitor') },
                    { value: debitCredit.creditor, text: l('Creditor') }],
                errors,
                isShow,
                accountTypes,
                companyMap,
                clients,
                vendors,
                currencies,
                subjects,
                subjectMap,
                autoBalance,
                input,
                save,
                subjectChange,
                arapFieldChange,
                currencyChange,
                setNativeAmount,
                l,
                renderAmount
            }
        }
    }
    const editHeaderTemplate = `<div class="head mb-2 row">
      <div class="col row">
        <label for="debitorId" class="form-label col-sm-3 text-end">{{l('Debitor')}}<span> * </span></label>
        <div  id="debitor" class="col-sm-9">
            <select v-model="editItem.debitorId" :class="{'is-invalid': errors.debitorId }" class="form-control" id="debitorId" name="debitorId">
                <option value="-">--</option>
                <option v-for="subItem in vendors || []" :key="subItem.id" :value="subItem.id">{{subItem.code + ' - '+ subItem.name }}</option>
            </select>
             <div id="debitorIdFeedback" class="invalid-feedback">
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
        emits: ['creditor-change'],
        setup(props, { emit }) {
            const companyStore = useCompanyStore();
            const { vendors } = storeToRefs(companyStore);
            const { setCompanies } = companyStore;

            const voucherStore = useVoucherStore();
            const { editItem, clearData } = storeToRefs(voucherStore);

            onMounted(() => {
                nextTick(() => {
                    initCompanySelect(false, '#debitorId', '#debitor',
                        e => {
                            editItem.value.debitorId = e.params.data.id;
                            emit('creditor-change', 1);
                            voucherStore.clearData();
                        }, setCompanies);
                })
            });
            return {
                editItem,
                vendors,
                l
            }
        }
    }
    const voucherDetailsTemplate = `<div>
        <div v-if="isDraftStatus">
            <button type="button" class="btn btn-primary btn-sm" @click="addDetail">
                <i class="fa fa-plus"></i> {{l('AddDetail')}}
            </button>
        </div>
        <div class="items">
            <table class="table table-responsive table-striped" :style="tableStyle">
                <colgroup>
                    <col v-if="isDraftStatus" style="width: 120px;" />
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
                    <th v-if="isDraftStatus">{{l('Actions')}}</th>
                    <th>{{l('Subject')}}</th>
                        <th>{{l('Description')}}</th>
                        <th>{{l('Debitor')}}<div>{{nativeCurrency}}</div></th>
                        <th>{{l('Creditor')}}<div>{{nativeCurrency}}</div></th>
                        <th><div>{{l('DebitorCreditor')}}</div><div>{{l('Currency')}}</div></th>
                        <th class="text-end normal"><div>{{l('ForeignAmount')}}</div><div>{{l('ExchangeRate')}}</div></th>
                        <th>{{l('SubSubject')}}</th>
                        <th>{{l('DocNo')}}</th>
                        <th>{{l('PaymentReference')}}</th>
                </tr>
                </thead>
                <tbody>
                    <tr v-for="item in editItem.details || []" :key="item.rowid">
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
                        <td>{{item.debitorCreditor === debitCredit.debitor ? renderAmount(item.nativeAmount) : ''}}</td>
                        <td>{{item.debitorCreditor === debitCredit.credittor ? renderAmount(item.nativeAmount) : ''}}</td>
                        <td><div>{{item.debitorCreditor === debitCredit.debitor ? l('Debitor'): l('Creditor')}}</div><div>{{item.currencyCode}}</div></td>
                        <td class="text-end"><div>{{renderAmount(item.foreignAmount)}}</div><div>{{renderAmount(item.currencyRate, 7)}}</div></td>
                        <td>{{ item.subSubjectName }}</td>
                        <td>{{item.docNo}}</td>
                        <td>{{ item.paymentReference }}</td>
                    </tr>
                </tbody>
                <tfoot>
                    <tr>
                    <td :colspan="isDraftStatus? 3: 2" class="text-end">{{l('Total')}}</td>
                    <td>{{renderAmount(totalDebitorAmount)}}</td>
                    <td colspan="11">{{renderAmount(totalCreditorAmount)}}</td>
                    </tr>
                </tfoot>
            </table>
        </div>
    </div>`
    const VoucherDetails = {
        template: voucherDetailsTemplate,
        emits: ['add-detail', 'show-detail', 'delete-detail'],
        setup(props, { emit }) {
            const voucherStore = useVoucherStore();
            const { editItem, totalDebitorAmount, totalCreditorAmount, isDraftStatus } = storeToRefs(voucherStore);

            const currencyStore = useCurrencyStore();
            const { nativeCurrency } = storeToRefs(currencyStore);

            const addDetail = () => {
                emit('add-detail');
            }
            const showDetail = (item) => {
                emit('show-detail', item);
            }
            const deleteDetail = (item) => {
                emit('delete-detail', item);
            }

            return {
                editItem,
                totalDebitorAmount,
                totalCreditorAmount,
                nativeCurrency,
                isDraftStatus,
                tableStyle: {
                    ['min-width']: '1460px',
                    ['max-width']: '1630px'
                },
                debitCredit,
                addDetail,
                showDetail,
                deleteDetail,
                l,
                renderAmount,
                formatRowDate,
            }
        }
    }

    const editModalTemplate = `<div><Modal  v-if="isShowModal" :value="isShowModal" @input="input" @save="save" 
    :title="l( (isDraftStatus ? editItem.id ? 'Edit' : 'New': 'View') + 'PayableVoucher' )" :show-submit="isDraftStatus">
<div id="content">
    <EditHeader v-if="isShowHeader && (editItem.id && editItem.debitorId  || !editItem.id )" :errors="errors" @creditor-change="getDetailsByDebitor" />
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
        <VoucherDetails  @add-detail="addDetail" @show-detail="showDetail" @delete-detail="deleteDetail"/>
      </div>
     <div :class="[!showDetailTab? 'show active':'']" class="tab-pane fade" id="receipts-tab-pane" role="tabpanel" aria-labelledby="receipts-tab" tabindex="0">
      <PayableDetail  @change-page="changeReceiptPage"/>
     </div>
    </div>
</div> 
</Modal><EditDetail v-if="showDetailModal" :value="isShowDetail" :item="item" @input="detailInput" @save="saveDetail" @auto-balance="autoBalance"></EditDetail></div>`;

    const EditModal = {
        components: { Modal, EditHeader, VoucherDetails, EditDetail, PayableDetail },
        template: editModalTemplate,
        setup() {
            const isShowDetail = ref(false);
            const showDetailModal = ref(false);
            const item = ref(getDefaultDetail());
            const errors = ref({});
            const showDetailTab = ref(false);
            const isShowHeader = ref(false);

            const editModalSotre = useEditModalStore();
            const { isShowModal } = storeToRefs(editModalSotre);
            const { showModal } = editModalSotre;

            const voucherStore = useVoucherStore();
            const { editItem, totalDebitorAmount, totalCreditorAmount, isDraftStatus } = storeToRefs(voucherStore);
            const { saveDetailItem, removeDetailItem, setReceipts } = voucherStore;

            watch(isShowDetail, (value) => {
                nextTick(() => { showDetailModal.value = value })
            });

            watch(isShowModal, (value) => {
                nextTick(() => { isShowHeader.value = value })
            });

            const input = (value) => {
                showDetailTab.value = false;
                showModal({ isShowModal: value })
            }
            const detailInput = (value) => {
                isShowDetail.value = value;
            }
            const validate = () => {
                const { id, prefix, voucherDate } = editItem.value;
                errors.value = {};
                let errorCount = 0;
                let message = ''
                if (!id && !prefix) {
                    errors.value.prefix = true;
                    errorCount++;
                }
                if (!voucherDate || (new Date(voucherDate)).toString() === 'Invalid Date') {
                    errors.value.voucherDate = true;
                    errorCount++;
                }
                if (totalCreditorAmount.value == 0 || totalDebitorAmount.value == 0) {
                    message = l('DebitorCreditorAmountMustGreaterThanZero');
                    errorCount++;
                }
                if (totalCreditorAmount.value !== totalDebitorAmount.value) {
                    message += l('VoucherDoesNotBalance');
                    errorCount++;
                };
                if (message) {
                    abp.message.error(message);
                }
                return errorCount === 0;
            }
            const save = () => {
                editItem.value.voucherDate = document.querySelector("#voucherDate").value;
                if (!validate()) {
                    return;
                }
                abp.ui.setBusy('#content');
                const data = { ...editItem.value }
                data.voucherDate = data.voucherDate;
                const request = editItem.value.id ? accounting.finance.transferVoucher.update(editItem.value.id, data) :
                    accounting.finance.transferVoucher.create(data);
                request.then(() => {
                    abp.ui.clearBusy('#content');
                    abp.notify.success(l('SavedSuccessfully'));
                    showModal({ isShowModal: false });
                    $('#voucherTable').DataTable().ajax.reload();
                }).catch(() => {
                    abp.ui.clearBusy('#content');
                });
            }
            const showDetail = (obj) => {
                item.value = obj;
                isShowDetail.value = true;
            }
            const deleteDetail = (obj) => {
                removeDetailItem({ item: obj })
                isShowDetail.value = false;
            }
            const saveDetail = () => {
                saveDetailItem({ item: item.value });
                isShowDetail.value = false;
            }
            const addDetail = () => {
                item.value = getDefaultDetail();
                isShowDetail.value = true;
            }

            const autoBalance = () => {
                const itemValue = item.value;
                const currencyRate = Number(itemValue.currencyRate);
                if (!currencyRate) {
                    return
                }
                let balanceAmount = totalDebitorAmount.value - totalCreditorAmount.value
                if (itemValue.nativeAmount) {
                    if (itemValue.debitorCreditor === debitCredit.debitor) {
                        balanceAmount -= Number(itemValue.nativeAmount);
                    } else {
                        balanceAmount += Number(itemValue.nativeAmount);
                    }
                }
                const debitorCreditor = balanceAmount > 0 ? debitCredit.creditor : debitCredit.debitor;
                item.value.nativeAmount = Math.abs(balanceAmount);
                item.value.foreignAmount = Number((itemValue.nativeAmount / currencyRate).toFixed(2))
                item.value.debitorCreditor = debitorCreditor;
            }
            const getDetailsByDebitor = (newPage) => {
                const params = {
                    debitorId: editItem.value.debitorId,
                    maxResultCount,
                    skipCount: ((newPage || 1) - 1) * maxResultCount
                };
                voucherRequests
                    .getPayableDetailsByDebitor(params)
                    .then(response => {
                        setReceipts({ ...(response || {}), currentPage: newPage });
                    }).catch(() => { });
            }

            const changeReceiptPage = (newPage) => {
                getDetailsByDebitor(newPage);
            }

            const formatInputDate = (value) => {
                return formatDate(value)
            }

            return {
                isShowDetail,
                item,
                errors,
                showDetailModal,
                showDetailTab,
                isShowHeader,
                debitCredit,
                isShowModal,
                showModal,
                editItem,
                isDraftStatus,
                input,
                detailInput,
                save,
                showDetail,
                deleteDetail,
                saveDetail,
                addDetail,
                autoBalance,
                changeReceiptPage,
                getDetailsByDebitor,
                l,
                renderAmount,
                formatInputDate
            }
        }
    }

    const App = {
        components: { EditModal },
        template: `<div><EditModal /></div>`,
    }

    const app = createApp(App);
    app.use(createPinia());
    app.mount('#app');

    const editModalSotre = useEditModalStore();
    const voucherStore = useVoucherStore();
    const currencyStore = useCurrencyStore();
    const subjectStore = useSubjectStore();
    const companyStore = useCompanyStore();

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
    const editHandle = function (id) {
        const requests = [];
        if (id) {
            requests.push(voucherRequests.get(id));
        } else {
            requests.push(new Promise(resolve => resolve(getVoucher())));
        }

        if (!editModalSotre.isRequestData) {
            requests.push(accounting.basicData.currency.getActiveList().then(result =>
                currencyStore.setCurrencies({ currencies: result })));
            requests.push(accounting.finance.accountingSetting.getNativeCurrency()
                .then(result => currencyStore.setNativeCurrency(result)));

            requests.push(accounting.finance.subject.getList({
                maxResultCount: 1000,
                isPaymentMethod: true,
            }).then(subjectResult => {
                voucherStore.setPaymentMethods(subjectResult.items)
            }))
            requests.push(accounting.finance.subject.getList({
                maxResultCount: 10,
                isIncludeAccountType: true,
                isIncludeReceivableSubject: true
            }).then(subjectResult => {
                subjectStore.setSubjects({
                    subjects: subjectResult.items,
                    setDetail: true
                })
            }))
        }
        editModalSotre.showModal({ isShowModal: true, id });

        Promise.all(requests).then(results => {
            const item = results[0];
            if (!item.details) {
                item.details = [];
            }
            voucherStore.setEditItem({ item });
            if (id) {
                const details = (item.details || [])
                const subjectIds = details.filter(obj =>
                    !subjectStore.subjectMap[obj.subjectId]).map(
                        detailItem => detailItem.subjectId);
                let setDetail = false
                if (subjectIds.length > 0) {
                    accounting.finance.subject.getList({
                        maxResultCount: subjectIds.length,
                        sorting: '',
                        subjectIds,
                        isIncludeAccountType: true,
                    }).then(subjectResult => {
                        subjectStore.setSubjects({
                            subjects: subjectResult.items,
                            setDetail: true
                        })
                        voucherStore.setDetails()
                    })
                } else {
                    setDetail = true
                }
                const companyIds = details.filter(obj => obj.subSubjectCode &&
                    !companyStore.companyMap[obj.subSubjectCode]).map(
                        detailItem => detailItem.subSubjectCode)
                if (companyIds.length) {
                    accounting.basicData.company.getList({
                        maxResultCount: companyIds.length,
                        sorting: '',
                        ids: companyIds
                    }).then(companyResult => {
                        companyStore.setCompanies({ items: companyResult.items })
                        voucherStore.setDetails()
                    })
                } else {
                    setDetail = true
                }
                if (setDetail) {
                    voucherStore.setDetails()
                }
                voucherRequests.getPayableDetails(id).then(result => {
                    const items = result || [];
                    voucherStore.setReceipts({ items: items, totalCount: items.length, currentPage: 1 })
                });
            }
            if (!editModalSotre.isRequestData) {
                editModalSotre.setIsRequestData({ isRequestData: true });
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
                                    text: l('UpdateStatus'),
                                    iconClass: '',
                                    action: function (data) {
                                        updateStatusModal.open({ id: data.record.id })
                                    },
                                    visible: abp.auth.isGranted('Accounting.Payable.PayableVoucher.UpdateStatus')
                                },
                                {
                                    text: l('Delete'),
                                    visible: isGrantedDelete,
                                    confirmMessage: function (data) {
                                        return l('DeletionConfirmationMessage', l('Menu:PayableVoucher'), data.record.code);
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
                        return data === voucherStatus.approval ? l('Approval') :
                            data === voucherStatus.void ? l('Void') : l('Draft');
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
});
