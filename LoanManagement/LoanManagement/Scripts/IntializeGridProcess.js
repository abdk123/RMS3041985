$(function () {
    $("#rounded-corner").jqGrid({
        url: "/Process/GetProcessToGrid",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID','المركز','نوع المعاملة','رقم الدور','رقم امر القبض','تاريخ بدء المعاملة','تاريخ إنتهاء المعاملة', 'الاسم', 'الكنية', 'اسم الاب', 'اسم الام', 'الرقم الوطني'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },
             { key: false, name: 'CenterName', index: 'CenterName', editable: true },
             { key: false, name: 'ProcessType', index: 'ProcessType', editable: true },
             { key: false, name: 'ORDERNO', index: 'ORDERNO', editable: true },
              { key: false, name: 'FEENO', index: 'FEENO', editable: true },
               { key: false, name: 'SDAT', index: 'SDAT', editable: true, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
                { key: false, name: 'EDAT', index: 'EDAT', editable: true },
            { key: false, name: 'FNAME', index: 'FNAME', editable: true },
            { key: false, name: 'LNAME', index: 'LNAME', editable: true },
            { key: false, name: 'FATHER', index: 'FATHER', editable: true },
            { key: false, name: 'MOTHER', index: 'MOTHER', editable: true },
            { key: false, name: 'SYRIAID', index: 'SYRIAID', editable: true }],
        pager: jQuery('#pager'),
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: '100%',
        viewrecords: true,
        caption: 'العمليات',
        emptyrecords: 'لا يوجد بيانات يمكن عرضها',
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false,
            Id: "0"
        },
        autowidth: true,
        multiselect: false
    }).navGrid('#pager', { edit: false, add: false, del: false, search: false, refresh: true },
        
        {
            // add options
            zIndex: 100,
            url: "/Process/Create",
            closeOnEscape: true,
            closeAfterAdd: true,
            afterComplete: function (response) {
                if (response.responseText) {
                    alert(response.responseText);
                }
            }
        }
       );
});