$(function () {
    const l = abp.localization.getResource('Accounting');
    const editModal = new abp.ModalManager(abp.appPath + 'GenenalLedger/SubjectCategory/EditModal'); 
    const isGrantedEdit = abp.auth.isGranted('Accounting.GenenalLedger.SubjectCategory.Edit');
    const isGrantedDelete = abp.auth.isGranted('Accounting.GenenalLedger.SubjectCategory.Deletion');
    let accountTypes = [], categories = [];
    accounting.finance.accountType.getSimpleList().then(result => { accountTypes = result });
    accounting.finance.subjectCategory.getSimpleList().then(result => { categories = result });

    const dataTable = $('#subjectCategoryTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(accounting.finance.subjectCategory.getList),
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
                                        return l('DeletionConfirmationMessage', l('Menu:SubjectCategory'), data.record.code);
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
                    title: l('ParentCode'),
                    data: "parentId",
                    orderable: true,
                    render: function (data) { 
                        if (!data || !categories) {
                            return '';
                        }
                        const category = categories.find(at => at.id === data);
                        return category ? category.code : '';
                    }
                },
                {
                    title: l('AccountTypeId'),
                    data: "accountTypeId",
                    orderable: true, 
                    render: function (data) {
                        if (!data || !accountTypes) {
                            return '';
                        }
                        const accountType = accountTypes.find(at => at.id === data);
                        return accountType ? accountType.code : '';
                    }
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
    const createModal = new abp.ModalManager(abp.appPath + 'GenenalLedger/SubjectCategory/CreateModal');
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
});