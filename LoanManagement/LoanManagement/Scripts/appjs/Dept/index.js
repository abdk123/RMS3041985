var MyGrid = null;

function afterLoad(o) {
    MyGrid = $("#DTGrid")
    initialGrid();
    // addbtns();
   
}

function ReloadGrid(options) {
    var action = options.action
    $("[iscreateroledlog]").dialog("close")
  
    switch (action) {
        case "closecreate":
            $("[iscreateroledlog]").dialog("close")
            break;
    }
    MyGrid.setGridParam().trigger('reloadGrid');
}

function Search(elem) {
    var usrNam = $("#Name").val();
   
    // alert(accID);
    //  return;
    // var f = tbl.getFilter()
    //var grid = tbl.grid
    var postData = MyGrid.getGridParam('postData')
    postData.filters = usrNam ;
    MyGrid.setGridParam('postData', postData)
    MyGrid.trigger('reloadGrid');
    //MyGrid.(f.getFilter())
}
function Clear(elem) {
    $("#Name").val("");
   
    var postData = MyGrid.getGridParam('postData')
    postData.filters = "";
    MyGrid.setGridParam('postData', postData)
    MyGrid.trigger('reloadGrid');
}

function initialGrid() {

    MyGrid.jqGrid({
        url: "/Dept/GetDeptToGrid",
        datatype: 'json',
        mtype: 'Get',
        colNames: [ 'ID','Tools','Name', 'Note'],
        colModel: [
             { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },
              { name: 'tools', index: 'tools', width: 60, align: "center", sortable: false, search: false },
             { key: true, name: 'Name', index: 'Name', editable: true },
             { key: false, name: 'Note', index: 'Note', editable: true }
           
           ],
        pager: jQuery('#pager'),
        rowNum: 10,
        rowList: [10, 20, 50, 100],
        height: '100%',
        viewrecords: true,
        caption: 'Department Managements',
        emptyrecords: 'No Receords Exist',
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false,
            Id: "0"
        },

        autowidth: true,
        multiselect: false,
        afterInsertRow: function (RowId, currentData, jsondata) {
           tg_addImageButton({ grid: this, RowId: RowId, CellKey: "tools", title: "Tools", evt: "InitContext(this);return false;", src: 'Content/images/setting.ico' })

        }

    }).navGrid('#pager', { edit: false, add: false, del: false, search: false, refresh: true }
      );
    // end


}

function addDept(elem) {
    openDialog({
        url: "Dept/CreateWindow",
        title: "Add New Deprtment",
        extraattr: { iscreateroledlog: "true" },
        width: 500,
       height: 200
             
    })
}
function EditDept(rowID) {
    openDialog({
        url: "/Dept/UpdateWindow/?id=" + rowID,
        title: "Update Deprtment",
        extraattr: { iscreateroledlog: "true" },
        width: 500,
        height: 200

    })
}

function initialContextMenu(CMO) {
    $(CMO.gridSelector).contextMenu({
        selector: CMO.buttonSelector,
        trigger: CMO.trigger,
        delay: 300,
        callback: CMO.callback,
        items: CMO.items
    });
}
function InitContext(elem) {
    //
    $(".context-menu-list").remove()
    var RowId = $(elem).attr("data-id")
    var rowData = MyGrid.getRowData(RowId);

    var itemMap = {};
    //  alert(IsCentAdmin);
    itemMap.Update_Dept = { name: "Update_Dept", icon: "Update_Dept", _name: "Update Department" }
   // itemMap.CanActivate = { name: "CanActivate", icon: "CanActivate", _name: "Update Track" }




    $("img[data-id=" + RowId + "]").closest("td").off('contextmenu')
    initialContextMenu({
        gridSelector: $("img[data-id=" + RowId + "]").closest("td")
                      , buttonSelector: "img[data-id=" + RowId + "]"
                      , trigger: "left"
                      , callback: function (key, options) {
                          MyGrid.setSelection(RowId);
                          // alert("In Intial Cont Invoke :" + RowId);
                          if (key == "Update_Dept") {
                              EditDept(RowId);
                          }
                          //else if (key == "CanActivate") {
                          //    cancel_Activate(RowId);
                          //}

                      }
                      , items: itemMap
    });
}


   


   

