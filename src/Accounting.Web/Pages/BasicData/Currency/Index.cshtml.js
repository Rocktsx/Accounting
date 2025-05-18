$(function () {
    var l = abp.localization.getResource('Accounting');
    var amountRender = DataTable.render.number(null, null, 7, '', '');
    var dataTable = $('#currencyTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[3, "asc"]],
            searching: false,
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
                                        editModal.open({ sourceCurrency: data.record.sourceCurrency, targetCurrency: data.record.targetCurrency });
                                    },
                                    //visible: abp.auth.isGranted('Accounting.CurrencyEdit')
                                },
                                {
                                    text: l('Delete'),
                                    //visible: abp.auth.isGranted('Accounting.CurrencyDeletion'),
                                    confirmMessage: function (data) {
                                        return l('CurrencyDeletionConfirmationMessage',
                                            data.record.targetCurrency);
                                    },
                                    action: function (data) {
                                        accounting.basicData.currency
                                            .delete({ sourceCurrency: data.record.sourceCurrency, targetCurrency: data.record.targetCurrency })
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
    $('#newCurrencyButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });

    var editModal = new abp.ModalManager(abp.appPath + 'BasicData/Currency/EditModal');
    editModal.onResult(function () {
        dataTable.ajax.reload();
    });
});