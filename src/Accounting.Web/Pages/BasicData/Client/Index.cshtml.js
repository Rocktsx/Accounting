$(function () {
    var l = abp.localization.getResource('Accounting');

    const model = {
        addresses: [], contacts: [], isEditAddress: false, isEditContact: false, editItem: {}
    };

    const getRowId = () => {
        let rowId = 0;

        return () => {
            return ++rowId;
        };
    }
    const getNewRowId = getRowId();
    const removeRow = (table, row, dataField) => {
        let data = model[dataField];
        const index = data.findIndex(item => item.rid === row.rid);
        if (index !== -1) {
            data.splice(index, 1);
            table.row((idx, item) => item.rid === row.rid).remove().draw();
        }
    }
    const addRow = (table, item, dataField) => {
        item["rid"] = getNewRowId();
        model[dataField].push(item);
        table.row.add(item).draw();
    }
    const editRow = (table, item, dataField) => {
        const data = model[dataField];
        const editItem = model.editItem
        const index = data.findIndex(obj => editItem.rid == obj.rid);
        if (index > -1) {
            data[index] = { ...editItem, ...item };
            const row = table.row((idx, obj) => obj.rid === editItem.rid);
            if (row.length) {
                row.data(data[index]).draw();
            }
        }
    }
    const getFormValues = ($form, prefix) => {
        const result = {};
        $form.find('input:not([type="hidden"]),textarea,select').each(function (ele) {
            const $this = $(this);
            let name = $this.attr("name");
            if (prefix) {
                name = name.replace(prefix, "");
            }
            if (name) {
                const field = name.substring(0, 1).toLowerCase() + name.substring(1)  
                if (field.startsWith("is")) {
                    result[field] = ($this.is(':checkbox') ? $this : $this.prev()).is(":checked");
                } else {
                    result[field] = $this.val();
                }
            }
        });
        return result;
    }
    abp.modals.CompnayAddressAndContact = function () {
        function initModal(modalManager, args) {
            const $form = $("#addressForm, #contactForm"); //modalManager.getForm();
            const { isEdit, prefix } = args || {};
            const record = model.editItem;
            if (!isEdit || !$form) {
                return
            }

            $form.find('input:not([type="hidden"]),textarea').each(function (ele) {
                const $this = $(this);
                let name = $this.attr("name");
                if (name) {
                    let fieldName = name;
                    if (prefix) {
                        fieldName = fieldName.replace(prefix, "");
                    }
                    const field = fieldName.substring(0, 1).toLowerCase() + fieldName.substring(1)
                    $this.val(record[field]);
                    if (field.startsWith("is") && record[field] !== "false") {
                        ($this.is(':checkbox') ? $this : $this.prev()).attr("checked", record[field]);
                    }
                }
            });
        };

        return {
            initModal: initModal
        };
    };
    abp.modals.CreateEditCompany = function () {
        function initModal(modalManager, args) {
            const addressModal = new abp.ModalManager(
                {
                    viewUrl: abp.appPath + 'BasicData/Client/CreateAddressModal',
                    modalClass: 'CompnayAddressAndContact'
                });
            const addressDataTable = $('#addressTable').DataTable(
                abp.libs.datatables.normalizeConfiguration({
                    serverSide: false,
                    paging: false,
                    order: [[1, "asc"]],
                    searching: false,
                    scrollX: true,
                    fixedHeader: {
                        footer: false
                    },
                    data: model.addresses,
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
                                                model.isEditAddress = true;
                                                model.editItem = data.record;
                                                addressModal.open({ isEdit: true, prefix: 'Address.' });
                                            },
                                            //visible: abp.auth.isGranted('Accounting.BasicData.Client.Edit')
                                        },
                                        {
                                            text: l('Delete'),
                                            //visible: abp.auth.isGranted('Accounting.BasicData.Client.Deletion'), 
                                            action: function (data) {
                                                removeRow(addressDataTable, data.record, "addresses");
                                            }
                                        }
                                    ]
                            }
                        },
                        {
                            title: l('IsBilling'),
                            data: "isBilling",
                            render: function (data) {
                                return data ? '<i class="fa fa-check"></i>' : '<i class="fa fa-xmark"></i>';
                            }
                        },
                        {
                            title: l('IsShipping'),
                            data: "isShipping",
                            render: function (data) {
                                return data ? '<i class="fa fa-check"></i>' : '<i class="fa fa-xmark"></i>';
                            }
                        },
                        {
                            title: l('Name'),
                            data: "name"
                        },
                        {
                            title: l('ContactPerson'),
                            data: "contactPerson"
                        },
                        {
                            title: l('Telephone'),
                            data: "telephone"
                        },
                        {
                            title: l('Address'),
                            data: "address"
                        },
                        {
                            title: l('Fax'),
                            data: "fax"
                        },
                        {
                            title: l('Email'),
                            data: "email"
                        },
                        {
                            title: l('Remark'),
                            data: "remark"
                        },
                        {
                            title: l('Country'),
                            data: "country"
                        },
                        {
                            title: l('Region'),
                            data: "region"
                        },
                        {
                            title: l('District'),
                            data: "district"
                        }
                    ]
                })
            );

            $('#newAddressBtn').click(function (e) {
                e.preventDefault();
                model.isEditAddress = false;
                addressModal.open();
            });

            $(document).on('click', '#addressForm button[type="submit"]', function (e) {
                e.preventDefault();
                const form = $('#addressForm');
                if (!form.valid()) {
                    return;
                }
                const address = getFormValues(form, 'Address.');
                if (model.isEditAddress) {
                    editRow(addressDataTable, address, "addresses");
                } else {
                    addRow(addressDataTable, address, "addresses");
                }
                addressModal.close();
                return false;
            });


            const contactModal = new abp.ModalManager({
                viewUrl: abp.appPath + 'BasicData/Client/CreateContactModal',
                modalClass: 'CompnayAddressAndContact'
            });
            const contactDataTable = $('#contactTable').DataTable(
                abp.libs.datatables.normalizeConfiguration({
                    serverSide: false,
                    paging: false,
                    order: [[1, "asc"]],
                    searching: false,
                    scrollX: true,
                    data: model.contacts,
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
                                                model.editItem = data.record;
                                                model.isEditContact = true;
                                                contactModal.open({ isEdit: true });
                                            },
                                            //visible: abp.auth.isGranted('Accounting.BasicData.Client.Edit')
                                        },
                                        {
                                            text: l('Delete'),
                                            //visible: abp.auth.isGranted('Accounting.BasicData.Client.Deletion'), 
                                            action: function (data) {
                                                removeRow(contactDataTable, data.record, "contacts");
                                            }
                                        }
                                    ]
                            }
                        },
                        {
                            title: l('ContactName'),
                            data: "contactName"
                        },
                        {
                            title: l('Department'),
                            data: "department"
                        },
                        {
                            title: l('Position'),
                            data: "position"
                        },
                        {
                            title: l('DirectLine'),
                            data: "directLine"
                        },
                        {
                            title: l('Telephone'),
                            data: "telephone"
                        },
                        {
                            title: l('Fax'),
                            data: "fax"
                        },
                        {
                            title: l('Email'),
                            data: "email"
                        },
                        {
                            title: l('Remark'),
                            data: "remark"
                        }
                    ]
                })
            );

            $('#newContactBtn').click(function (e) {
                e.preventDefault();
                model.isEditContact = false;
                contactModal.open();
            });
            $(document).on('click', '#contactForm button[type="submit"]', function (e) {
                e.preventDefault();
                const form = $('#contactForm');
                if (!form.valid()) {
                    return;
                }
                const item = getFormValues(form);
                if (model.isEditContact) {
                    editRow(contactDataTable, item, "contacts");
                } else {
                    addRow(contactDataTable, item, "contacts");
                }
                contactModal.close();
                return false;
            });
        };

        return {
            initModal: initModal
        };
    };

    var createModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'BasicData/Client/CreateModal',
        modalClass: 'CreateEditCompany'
    });
    var editModal = new abp.ModalManager(abp.appPath + 'BasicData/Client/EditModal');

    var dataTable = $('#clientTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(accounting.basicData.company.getList, { isClient: true }),
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

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });
    $(document).on('click', '#newClientButton', function (e) {
        e.preventDefault();
        createModal.open();
    })

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });
    $(document).on('click', '#clientForm button[type="submit"]', function (e) {
        e.preventDefault();
        const form = $('#clientForm');
        if (!form.valid()) {
            return;
        }
        const data = getFormValues(form,'Client.');
        data.addresses = model.addresses
        data.contacts = model.contacts
        data.code = '';
        data.genNo = data.genNo.trim() || 0

        accounting.basicData.company.create(data).then(function () { 
            abp.notify.success(l('SavedSuccessfully'));
            createModal.close();
            dataTable.ajax.reload();
        })
        return false;
    });
});