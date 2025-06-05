$(function () {
    var l = abp.localization.getResource('Accounting');  
    var dataTable = $('#clientTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(accounting.basicData.company.getList, { isClient : true }),
            columnDefs: [
                {
                    title: l('Actions'),
                    orderable: false,
                    rowAction: {
                        items:
                            [
                                {
                                    text: l('Edit'),
                                    iconClass: '',
                                    action: function (data) {
                                        //editModal.open({ id: data.record.id });
                                    },
                                    //visible: abp.auth.isGranted('Accounting.BasicData.Client.Edit')
                                },
                                {
                                    text: l('Delete'),
                                    //visible: abp.auth.isGranted('Accounting.BasicData.Client.Deletion'),
                                    confirmMessage: function (data) {
                                        return l('ClientDeletionConfirmationMessage',
                                            data.record.name);
                                    },
                                    action: function (data) {
                                        accounting.basicData.company
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
                    data: "code"
                },
                {
                    title: l('Name'),
                    data: "name"
                },
                 
                {
                    title: l('OtherName'),
                    data: "otherName"
                },
                {
                    title: l('NickName'),
                    data: "nickName"
                }
            ]
        })
    );
    var createModal = new abp.ModalManager(abp.appPath + 'BasicData/Client/CreateModal');
    createModal.onResult(function () {
        dataTable.ajax.reload();
    });
    $(document).on('click', '#neClientButton', function (e) {
        e.preventDefault();
        //createModal.open();
    }) 

    var editModal = new abp.ModalManager(abp.appPath + 'BasicData/Client/EditModal');
    
    editModal.onResult(function () {
        dataTable.ajax.reload();
    });
});