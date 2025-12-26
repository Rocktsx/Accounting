$(function () {
    const l = abp.localization.getResource('Accounting');
    const isGrantedEdit = abp.auth.isGranted('Accounting.GeneralLedger.BankReconciliation.Edit');

    const debitorCreditor = {
        debitor: 1,
        creditor: -1
    }
    const inputAction = function () {
        const isPresented = $('#isPresented').val()
        return {
            subjectId: $('#subjectId').val(),
            isPresented: isPresented ? isPresented === 'true' : null,
            prefix: $('#prefix').val().trim(),
            startDate: $('#startDate').val().trim(),
            endDate: $('#endDate').val().trim(),
            startNo: $('#startNo').val().trim(),
            endNo: $('#endNo').val().trim(),
            referenceNo: $('#referenceNo').val().trim()
        };
    };
    const accountTypeTypes = {
        bank: 1
    };

    function initSubjectSelect() {
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
                        accountTypeCategory: accountTypeTypes.bank
                    };
                },
                processResults: function (data) {
                    const items = data.items;
                    const results = items.map(function (item) {
                        const { name, id, code } = item;
                        const text = code + ' - ' + name;
                        return {
                            id,
                            text: text,
                            displayName: text
                        }
                    });
                    return { results: results };
                }
            },
            width: '100%',
            placeholder: '',
            allowClear: true,
            language: language
        });
    }
    initSubjectSelect();

    const dataTable = $('#bankReconTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(accounting.finance.bankReconciliation.getList, inputAction),
            columnDefs: [
                {
                    title: l('VoucherDate'),
                    data: "voucherDate",
                    orderable: false,
                    dataFormat: 'date'
                },
                {
                    title: l('VoucherCode'),
                    orderable: false,
                    data: "voucherCode",
                },
                {
                    title: l('PaymentReference'),
                    orderable: false,
                    data: "paymentReference",
                },
                {
                    title: l('Description'),
                    orderable: false,
                    data: "description",
                },
                {
                    title: l('Debitor'),
                    data: "nativeAmount",
                    orderable: false,
                    className: 'text-end',
                    render: function (data, type, row) {
                        return row.debitorCreditor == debitorCreditor.debitor ? renderAmount(data) : '';
                    }
                },
                {
                    title: l('Creditor'),
                    data: "nativeAmount",
                    orderable: false,
                    className: 'text-end',
                    render: function (data, type, row) {
                        return row.debitorCreditor == debitorCreditor.creditor ? renderAmount(data) : '';
                    }
                },
                {
                    title: l('IsPresented'),
                    data: "isPresented",
                    orderable: false,
                    className: 'text-end',
                    render: function (data, type, row) {
                        return '<input class="form-check-input" type="checkbox" name="isPresented" ' +
                            (data ? 'checked' : '') + ' ' + (isGrantedEdit ? '' : 'disabled') + ' ' +
                            ('data-detailid="' + row.voucherDetailId + '"') + ' ' +
                            ('data-id="' + (row.id || '') + '"') +
                            ('data-value="' + row.isPresented + '"')
                            + '>';
                    }
                }
            ]
        })
    );

    $('#bankReconTable').on('preXhr.dt', function () {
        abp.ui.setBusy('#voucherTable')
    });
    $('#bankReconTable').on('xhr.dt', function (e) {
        abp.ui.clearBusy('#voucherTable')
    });

    $(document).on('click', '#searchBtn', function (e) {
        e.preventDefault();
        dataTable.ajax.reload();
    });
    $(document).on('click', '#saveBtn', function (e) {
        e.preventDefault();
        const params = [];
        var items = $('#bankReconTable [name="isPresented"]');
        $.each(items, (index, item) => {
            const $this = $(item)
            const param = {
                voucherDetailId: $this.attr('data-detailid'),
                id: $this.attr('data-id'),
                isPresented: $this.attr('data-value') === 'true',
            };
            const isChecked = $this.is(':checked');
            if (isChecked !== param.isPresented) {
                param.isPresented = isChecked;
                params.push(param)
            }
        });
        accounting.finance.bankReconciliation.addOrUpdateMany(params).then(() => {
            abp.notify.success(l('SavedSuccessfully'));
            dataTable.ajax.reload();
        }).catch(() => { })
    });
    $(document).on('click', '#setAll', function (e) {
        e.preventDefault();
        const isPresented = $('#isPresented').val() === 'true';
        var items = $('#bankReconTable [name="isPresented"]');
        $.each(items, (index, item) => {
            const $this = $(item);
            if (isPresented) {
                $this.attr('checked', 'checked')
            } else {
                $this.attr('checked', null)
            }
        }); 
    });
});