$(function () {
    var l = abp.localization.getResource('Accounting'); 
    var editModal = new abp.ModalManager(abp.appPath + 'BasicData/Currency/EditModal');
    var amountRender = DataTable.render.number(null, null, 7, '', '');
    var dataTable = $('#currencyTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[3, "asc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(
                accounting.basicData.currency.getList),
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
                                        editModal.open({ id: data.record.id});
                                    },
                                    visible: abp.auth.isGranted('Accounting.BasicData.Currency.Edit')
                                },
                                {
                                    text: l('Delete'),
                                    visible: abp.auth.isGranted('Accounting.BasicData.Currency.Deletion'),
                                    confirmMessage: function (data) {
                                        return l('CurrencyDeletionConfirmationMessage',
                                            data.record.targetCurrency);
                                    },
                                    action: function (data) {
                                        accounting.basicData.currency
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
                    title: l('SourceCurrency'),
                    data: "sourceCurrency",
                    orderable: false,
                },
                {
                    title: l('SourceAmount'),
                    data: "sourceAmount",
                    orderable: false,
                    render: amountRender
                },
                {
                    title: l('TargetCurrency'),
                    data: "targetCurrency"
                },
                {
                    title: l('TargetAmount'),
                    data: "targetAmount",
                    orderable: false,
                    render: amountRender
                },
                {
                    title: l('ExchangeRate'),
                    data: "exchangeRate",
                    render: amountRender
                },
                {
                    title: l('EffectiveDate'),
                    data: "effectiveDate",
                    dataFormat: 'date'
                },
                {
                    title: l('IsActive'),
                    data: "isActive",
                    render: function (data) {
                        return data ? '<i class="fa fa-check"></i>' : '<i class="fa fa-xmark"></i>'; 
                    }
                }
            ]
        })
    );
    var createModal = new abp.ModalManager(abp.appPath + 'BasicData/Currency/CreateModal');
    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $(document).on('click', '#newCurrency', function (e) {
        e.preventDefault();
        createModal.open();
    }); 

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });
});