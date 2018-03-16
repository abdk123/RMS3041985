var MyGrid = null;

function afterLoad(o) {
    MyGrid = $("#DTGrid")
    initialGrid();
   // addbtns();
}

function ReloadGrid(options) {
    var action = options.action
    $("[iscreateroledlog]").dialog("close")
  // alert("OKKKK")
    switch (action) {
        case "closecreate":
            $("[iscreateroledlog]").dialog("close")
            break;
    }
    MyGrid.setGridParam().trigger('reloadGrid');
}

function Search(elem) {
    var usrNam = $("#UsrNam").val();
    var name = $("#Name").val();
    // alert(accID);
    //  return;
    // var f = tbl.getFilter()
    //var grid = tbl.grid
    var postData = MyGrid.getGridParam('postData')
    postData.filters = usrNam + "&" + name;
    MyGrid.setGridParam('postData', postData)
    MyGrid.trigger('reloadGrid');
    //MyGrid.(f.getFilter())
}
function Clear(elem) {
    $("#UsrNam").val("");
    $("#Name").val("");
    var postData = MyGrid.getGridParam('postData')
    postData.filters = "";
    MyGrid.setGridParam('postData', postData)
    MyGrid.trigger('reloadGrid');
}

function initialGrid() {

    MyGrid.jqGrid({
        url: "/UserManagement/GetUserToGrid",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID', 'Tools', 'User Name', 'Full Name', 'Department', 'RO Name', 'RO Code', 'Is Active'],
        colModel: [
             { key: true, hidden: true, name: 'id', index: 'id', editable: true },
              { name: 'tools', index: 'tools', width: 60, align: "center", sortable: false, search: false },
             { key: true, name: 'user_name', index: 'user_name', editable: true },
             { key: false, name: 'name', index: 'name', editable: true },
             { key: false, name: 'dept_name', index: 'dept_name', editable: true },
              { key: false, name: 'RO_Name', index: 'RO_Name', editable: true },
             
        { key: false, name: 'RO_Code', index: 'RO_Code', editable: true },
        { key: false, name: 'Activate', index: 'Activate', editable: true }],
        pager: jQuery('#pager'),
        rowNum: 10,
        rowList: [10, 20, 50, 100],
        height: '100%',
        viewrecords: true,
        caption: 'Users Managements',
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

function addUser(elem) {
    openDialog({
        url: "UserManagement/CreateWindow",
        title: "Add New User",
        extraattr: { iscreateroledlog: "true" },
        width: 650,
       height: 300
             
    })
}
function EditUser(rowID) {

    var ID = MyGrid.jqGrid('getCell', rowID, 'id');
    openDialog({
        url: "/UserManagement/UpdateWindow/?id=" + ID,
        title: "Update User",
        extraattr: { iscreateroledlog: "true" }

    })
}

function DeleteUser(rowID) {

    var ID = MyGrid.jqGrid('getCell', rowID, 'id');
    openDialog({
        url: "/UserManagement/DeleteWindow/?id=" + ID,
        title: "Delete User",
        extraattr: { iscreateroledlog: "true" },
       
        width: 550,
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
    itemMap.Update_User = { name: "Update_User", icon: "Update_User", _name: "Update User" }
    itemMap.Delete_User = { name: "Delete_User", icon: "Delete_User", _name: "Delete User" }




    $("img[data-id=" + RowId + "]").closest("td").off('contextmenu')
    initialContextMenu({
        gridSelector: $("img[data-id=" + RowId + "]").closest("td")
                      , buttonSelector: "img[data-id=" + RowId + "]"
                      , trigger: "left"
                      , callback: function (key, options) {
                          MyGrid.setSelection(RowId);
                          
                          if (key == "Update_User") {
                              EditUser(RowId);
                          } else if (key == "Delete_User")
                          {
                              DeleteUser(RowId);
                          }

                      }
                      , items: itemMap
    });
}


   


   

