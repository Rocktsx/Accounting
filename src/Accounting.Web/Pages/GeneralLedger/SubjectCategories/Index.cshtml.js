$(function () {
    const l = abp.localization.getResource('Accounting');
    const editModal = new abp.ModalManager(abp.appPath + 'GeneralLedger/SubjectCategories/EditModal');
    const isGrantedEdit = abp.auth.isGranted('Accounting.GeneralLedger.SubjectCategory.Edit');
    const isGrantedDelete = abp.auth.isGranted('Accounting.GeneralLedger.SubjectCategory.Deletion');
     
    const inputAction = function () {
        return {
            isIncludeParent: true
        };
    };

    const dataTable = $('#subjectCategoryTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(accounting.finance.accountType.getList, inputAction),
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
                                        editModal.open({ id: data.record.id });
                                    },
                                    visible: isGrantedEdit
                                },
                                {
                                    text: l('Delete'),
                                    visible: isGrantedDelete,
                                    confirmMessage: function (data) {
                                        return l('DeletionConfirmationMessage', l('Menu:SubjectCategory'), data.record.code);
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
                    title: l('Parent'),
                    data: "parentId",
                    orderable: true,
                    render: function (data, type, row) {
                        if (!data || !row.parent) {
                            return '';
                        }
                        return row.parent.name;
                    }
                },
                {
                    title: l('TrialBalanceSort'),
                    data: "trialBalanceSort",
                    orderable: true,
                },
                {
                    title: l('ProfitAndLossSort'),
                    data: "profitAndLossSort",
                    orderable: true,
                },
                {
                    title: l('BalanceSheetSort'),
                    data: "balanceSheetSort",
                    orderable: true,
                },
                {
                    title: l('TrialBalanceGroup'),
                    data: "trialBalanceGroup",
                    orderable: true,
                },
                {
                    title: l('ProfitAndLossGroup'),
                    data: "profitAndLossGroup",
                    orderable: true,
                },
                {
                    title: l('BalanceSheetGroup'),
                    data: "balanceSheetGroup",
                    orderable: true,
                }
            ]
        })
    );
    const createModal = new abp.ModalManager(abp.appPath + 'GeneralLedger/SubjectCategories/CreateModal');
    createModal.onResult(function () {
        dataTable.ajax.reload();
    });
    $(document).on('click', '#newSubjectCategoryBtn', function (e) {
        e.preventDefault();
        createModal.open();
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });
    $(document).on('abp-ajax-success', '#subjectCategoryForm', function () {
        $('#subjectCategoryForm').slideUp();
        var l = abp.localization.getResource('Accounting');
        abp.notify.success(l('SavedSuccessfully'));
    });
    $('#subjectCategoryTable').on('preXhr.dt', function () {
        abp.ui.setBusy('#subjectCategoryTable')
    });
    $('#subjectCategoryTable').on('xhr.dt', function (e) {
        abp.ui.clearBusy('#subjectCategoryTable')
    });
});