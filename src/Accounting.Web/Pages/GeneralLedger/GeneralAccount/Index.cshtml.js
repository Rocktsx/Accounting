$(function () {
    const l = abp.localization.getResource('Accounting');
    const editModal = new abp.ModalManager(abp.appPath + 'GeneralLedger/GeneralAccount/EditModal'); 
    const isGrantedEdit = abp.auth.isGranted('Accounting.GeneralLedger.GeneralAccount.Edit');
    const isGrantedDelete = abp.auth.isGranted('Accounting.GeneralLedger.GeneralAccount.Deletion'); 

    const dataTable = $('#generalAccountTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(accounting.finance.subjectCategory.getFilteredQueryList),
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
                                        return l('DeletionConfirmationMessage', l('Menu:GeneralAccount'), data.record.code);
                                    },
                                    action: function (data) {
                                        accounting.finance.subjectCategory
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
                    data: "parentName",
                    orderable: false
                },
                {
                    title: l('SubjectCategory'),
                    data: "accountTypeName",
                    orderable: false
                },
                {
                    title: l('DebitorCreditor'),
                    data: "debitorCreditor",
                    orderable: false,
                    render: function (data) {
                        return data === -1 ? l('Creditor') : data === 1 ? l('Debitor') : '';
                    }
                },
                {
                    title: l('ShowDetail'),
                    data: "showDetail",
                    orderable: false,
                    render: function (data) {
                        return data ? '<i class="fa fa-check"></i>' : '<i class="fa fa-xmark"></i>';
                    }
                },
                {
                    title: l('Description'),
                    data: "description",
                    orderable: false,
                }
            ]
        })
    );
    const createModal = new abp.ModalManager(abp.appPath + 'GeneralLedger/GeneralAccount/CreateModal');
    createModal.onResult(function () {
        dataTable.ajax.reload();
    });
    $(document).on('click', '#newGeneralAccountBtn', function (e) {
        e.preventDefault();
        createModal.open();
    }); 

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });
    $(document).on('abp-ajax-success', '#generalAccountForm', function () {
        $('#generalAccountForm').slideUp();
        var l = abp.localization.getResource('Accounting');
        abp.notify.success(l('SavedSuccessfully'));
    });
    const importModal = new abp.ModalManager(abp.appPath + 'GeneralLedger/GeneralAccount/ImportModal');
    $(document).on('click', '#importGeneralAccountBtn', function () {
        importModal.open();
    });
    $(document).on('click', '#importDataForm [type="submit"]', function (e) {
        e.preventDefault();
        const fileElement = document.querySelector('#importDataForm #file');
        const formData = new FormData();
        formData.append('file', fileElement.files[0]);
        abp.ui.setBusy('#importDataForm .modal-body')
        abp.ajax({
            url: abp.appPath + 'api/subject-category',
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
});