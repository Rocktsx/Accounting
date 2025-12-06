$(function () {
    const l = abp.localization.getResource('Accounting');

    const inputAction = function (requestData, dataTableSettings) {
        return {
            code: $('#code').val().trim(),
            voucherType: $('#voucherType').val(),
            status: $('#status').val()
        };
    }; 
    const voucherStatus = {
        draft: 0,
        approval: 1,
        void: 2
    }
    const voucherType = {
        journalVoucher: 0,
        receivableVoucher: 1
    }

    const tableSelector = '#voucherStates';
    const dataTable = $(tableSelector).DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(accounting.finance.voucherState.getList, inputAction),
            columnDefs: [
                {
                    title: l('VoucherType'),
                    data: "voucherType",
                    orderable: true,
                    render: function (data) {
                        return data === voucherType.journalVoucher ? l('JournalVoucher') :
                            data === voucherType.receivableVoucher ? l('ReceivableVoucher') : l('PayableVoucher');
                    }
                },
                {
                    title: l('Code'),
                    data: "code",
                    orderable: true
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
    const updateStatusModal = new abp.ModalManager(abp.appPath + 'GeneralLedger/Components/UpdateStatusModal');
    $(document).on('click', "#updateStatusBtn", function () {
        updateStatusModal.open();
    })
    $(document).on('click', '#updateStatusForm [type="submit"]', function (e) {
        e.preventDefault();
        const bodySelector = '#updateStatusForm .modal-body';
        abp.ui.setBusy(bodySelector)
        accounting.finance.voucherState.updateStatus(inputAction(), $('#newStatus').val()).then(() => {
            updateStatusModal.close();
            dataTable.ajax.reload();
            abp.ui.clearBusy(bodySelector);
            abp.notify.success(l('SavedSuccessfully'));
        }).catch(() => abp.ui.clearBusy(bodySelector));
    })
    $(document).on('click', '#search', function(){
        dataTable.ajax.reload();
    });
    $(document).on('keydown', '#searchForm', function (e) {
        if (e.which !== 13) {
            return;
        }
        e.preventDefault();
        dataTable.ajax.reload(); 
    })
    $(tableSelector).on('preXhr.dt', function () {
        abp.ui.setBusy(tableSelector)
    });
    $(tableSelector).on('xhr.dt', function (e) {
        abp.ui.clearBusy(tableSelector)
    });
})