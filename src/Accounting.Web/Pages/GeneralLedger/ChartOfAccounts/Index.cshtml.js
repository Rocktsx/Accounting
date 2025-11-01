$(function () {
    const l = abp.localization.getResource('Accounting');
    const editModal = new abp.ModalManager(abp.appPath + 'GeneralLedger/ChartOfAccounts/EditModal');
    const isGrantedEdit = abp.auth.isGranted('Accounting.GeneralLedger.Subject.Edit');
    const isGrantedDelete = abp.auth.isGranted('Accounting.GeneralLedger.Subject.Deletion');
    let categories = [], selectedCategory = {};

    function getCategories() {
        const cagegoryPromise = accounting.finance.subjectCategory.getFilteredQueryList({ maxResultCount: 1000, sorting: 'code' });
        const subjectPromise = accounting.finance.subject.getList({ maxResultCount: 1000, sorting: 'code' });
        Promise.all([cagegoryPromise, subjectPromise]).then(results => {
            categories = results[0].items.map(item => ({ text: item.code + ' - ' + item.name, item, isCategory: true }));
            const subjects = results[1].items.map(item => ({ text: item.code + ' - ' + item.name, item, isCategory: false }));

            const nodes = getTree(categories, subjects);
            const treeData = [{
                text: l('Menu:Subject'),
                nodes: nodes,
                item: {},
                isCategory: true
            }]

            $('#coaTree').treeview({
                data: treeData, nodeIcon: '', expandIcon: 'fa fa-plus',
                emptyIcon: 'fa fa-leaf', collapseIcon: 'fa fa-minus', checkedIcon: '',
                selectedIcon: 'fa fa-check', uncheckedIcon: ''
            });
        });

    }
    function getNodes(items, parentItem, isCategory) {
        return items.filter(o => isCategory ? o.item.parentId === parentItem.id && o.item.level === parentItem.level + 1 :
            o.item.subjectCategoryId === parentItem.id);
    }
    function getTreeNode(nodes, categories, subjects) {
        if (!nodes || nodes.length === 0) {
            return
        }

        nodes.forEach(item => {
            const subCategories = getNodes(categories, item.item, true);
            const subSubjects = getNodes(subjects, item.item);
            item['nodes'] = subCategories.concat(subSubjects);
            getTreeNode(subCategories, categories, subjects);
        });
    }
    function getTree(categories, subjects) {
        const roots = categories.filter(item => !item.item.parentId)

        roots.forEach(obj => {
            const subCategories = getNodes(categories, obj.item, true);
            const subSubjects = getNodes(subjects, obj.item);
            obj['nodes'] = subCategories.concat(subSubjects);
            getTreeNode(subCategories, categories, subjects);
        });
        const allSubjects = subjects.filter(item => !item.item.subjectCategoryId);
        roots.push(...allSubjects);
        return roots;
    }
    const subjectInputAction = function (requestData, dataTableSettings) {
        const id = selectedCategory.id || null;
        return {
            subjectCategoryId: id,
        };
    };
    function boolRender(data) {
        return data ? '<i class="fa fa-check"></i>' : '<i class="fa fa-xmark"></i>';
    }
    function debitorCreditorRender(data) {
        return data === -1 ? l('Creditor') : data === 1 ? l('Debitor') : '';
    }
    function setCategory(category) {
        $("#subjectCategoryCode").text(category.code || '');
        $("#name").text(category.name || '');
        $("#otherName").text(category.otherName || '');
        $("#debitorCreditor").text(debitorCreditorRender(category.debitorCreditor || ''));
        $("#accountType").text(category.accountTypeName || '');
        $("#showDetail").html(category.code ? boolRender(category.showDetail) : '');
        $("#parentCode").text(category.parnetName || '');
    }
    getCategories();
    const dataTable = $('#subjectTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(accounting.finance.subject.getFilteredQueryList, subjectInputAction),
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
                                        return l('DeletionConfirmationMessage', l('Menu:Subject'), data.record.code);
                                    },
                                    action: function (data) {
                                        accounting.finance.subject
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
                    title: l('AccountType'),
                    data: "accountTypeName",
                    orderable: false
                },
                {
                    title: l('DebitorCreditor'),
                    data: "debitorCreditor",
                    orderable: false,
                    render: debitorCreditorRender
                },
                {
                    title: l('Currency'),
                    data: "currencyCode",
                    orderable: false
                },
                {
                    title: l('IsSubSubjectType'),
                    data: "isSubSubjectType",
                    orderable: false,
                    render: boolRender
                },
                {
                    title: l('IsActive'),
                    data: "isActive",
                    orderable: false,
                    render: boolRender
                }, {
                    title: l('IsPayMethod'),
                    data: "isPayMethod",
                    orderable: false,
                    render: boolRender
                },
                {
                    title: l('Description'),
                    data: "description",
                    orderable: false,
                },
                {
                    title: l('SeqCode'),
                    data: "seqCode",
                    orderable: false,
                }
            ]
        })
    );
    const createModal = new abp.ModalManager(abp.appPath + 'GeneralLedger/ChartOfAccounts/CreateModal');
    createModal.onResult(function () {
        dataTable.ajax.reload();
    });
    $(document).on('click', '#newSubjectBtn', function (e) {
        e.preventDefault();
        createModal.open(selectedCategory && selectedCategory.id ? { subjectCategoryId: selectedCategory.id } : null);
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });
    $(document).on('abp-ajax-success', '#subjectForm', function () {
        $('#subjectForm').slideUp();
        var l = abp.localization.getResource('Accounting');
        abp.notify.success(l('SavedSuccessfully'));
    });
    $(document).on('nodeSelected', "#coaTree", function (e, data) {
        let newCategory = {}
        if (data.isCategory) {
            newCategory = data.item;
        } else {
            const obj = categories.find(o => o.item.id === data.item.subjectCategoryId);
            newCategory = obj ? obj.item : {};
        }
        if (newCategory.id !== selectedCategory.id) {
            selectedCategory = newCategory;
            dataTable.ajax.reload();
            setCategory(selectedCategory || {});
        }
    })
    const importModal = new abp.ModalManager(abp.appPath + 'GeneralLedger/ChartOfAccounts/ImportModal');
    $(document).on('click', '#importSubjectBtn', function () {
        importModal.open();
    });
    $(document).on('click', '#importSubjectsForm [type="submit"]', function (e) {
        e.preventDefault();
        const fileElement = document.querySelector('#importSubjectsForm #file');
        const formData = new FormData();
        formData.append('file', fileElement.files[0]);
        const bodySelector = '#importSubjectsForm .modal-body';
        abp.ui.setBusy(bodySelector)
        abp.ajax({
            url: abp.appPath + 'api/subjects',
            processData: false,
            contentType: false,
            method: 'POST',
            data: formData,
            success: function () {
                importModal.close();
                dataTable.ajax.reload();
                abp.notify.success(l('ImportDataSuccessfully'));
            },
            complete() {
                abp.ui.clearBusy(bodySelector)
            }
        });
    })
});