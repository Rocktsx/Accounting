$(function () {
    const l = abp.localization.getResource('Accounting');
    const editModal = new abp.ModalManager(abp.appPath + 'GenenalLedger/AccountType/EditModal'); 
    const isGrantedEdit = abp.auth.isGranted('Accounting.GenenalLedger.AccountType.Edit');
    const isGrantedDelete = abp.auth.isGranted('Accounting.GenenalLedger.AccountType.Deletion');

    const dataTable = $('#accountTypeTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[2, "asc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(accounting.finance.accountType.getList),
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
                                        editModal.open({ id: data.record.id});
                                    },
                                    visible: isGrantedEdit
                                },
                                {
                                    text: l('Delete'),
                                    visible: isGrantedDelete,
                                    confirmMessage: function (data) {
                                        return l('DeletionConfirmationMessage', l('Menu:AccountType'), data.record.code);
                                    },
                                    action: function (data) {
                                        accounting.finance.accountType
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
                    title: l('Name'),
                    data: "name",
                    orderable: true,
                },
                {
                    title: l('OtherName'),
                    orderable: true,
                    data: "otherName"
                }, 
                {
                    title: l('TrialBalanceSort'),
                    data: "trialBalanceSort",
                    orderable: false
                },
                {
                    title: l('ProfitAndLossSort'),
                    data: "profitAndLossSort",
                    orderable: false
                },
                {
                    title: l('BalanceSheetSort'),
                    data: "balanceSheetSort",
                    orderable: false
                },
                {
                    title: l('TrialBalanceGroup'),
                    data: "trialBalanceGroup",
                    orderable: false
                },
                {
                    title: l('ProfitAndLossGroup'),
                    data: "profitAndLossGroup",
                    orderable: false
                },
                {
                    title: l('BalanceSheetGroup'),
                    data: "balanceSheetGroup",
                    orderable: false
                }
            ]
        })
    );
    const createModal = new abp.ModalManager(abp.appPath + 'GenenalLedger/AccountType/CreateModal');
    createModal.onResult(function () {
        dataTable.ajax.reload();
    });
    $(document).on('click', '#newAccountTypeBtn', function (e) {
        e.preventDefault();
        createModal.open();
    }); 

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });
});