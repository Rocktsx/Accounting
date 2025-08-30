$(function () {
    const l = abp.localization.getResource('Accounting');
    const editModal = new abp.ModalManager(abp.appPath + 'GenenalLedger/AccountingPeriod/EditModal'); 
    const isGrantedEdit = abp.auth.isGranted('Accounting.GenenalLedger.AccountingPeriod.Edit');
    const isGrantedDelete = abp.auth.isGranted('Accounting.GenenalLedger.AccountingPeriod.Deletion');

    const dataTable = $('#table').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[2, "asc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(accounting.finance.accountingPeriod.getList),
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
                                        return l('DeletionConfirmationMessage', l('Menu:AccountingPeriod'), data.record.code);
                                    },
                                    action: function (data) {
                                        accounting.finance.accountingPeriod
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
                    title: l('AccPeriod'),
                    data: "code",
                    orderable: true
                },
                {
                    title: l('StartDate'),
                    data: "startDate",
                    orderable: true,
                    dataFormat: 'date'
                },
                {
                    title: l('EndDate'),
                    orderable: true,
                    data: "endDate",
                    dataFormat: 'date'
                }, 
                {
                    title: l('IsCurrentPeriod'),
                    data: "isCurrentPeriod",
                    orderable: false,
                    className: 'text-end',
                    render: function (data) {
                        return data ? '<i class="fa fa-check"></i>' : '<i class="fa fa-xmark"></i>'; 
                    }
                }
            ]
        })
    );
    const createModal = new abp.ModalManager(abp.appPath + 'GenenalLedger/AccountingPeriod/CreateModal');
    createModal.onResult(function () {
        dataTable.ajax.reload();
    });
    $(document).on('click', '#newBtn', function (e) {
        e.preventDefault();
        createModal.open();
    }); 

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });
});