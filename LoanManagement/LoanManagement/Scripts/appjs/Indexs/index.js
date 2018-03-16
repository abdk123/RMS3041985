var MyGrid = null;

function afterLoad(o) {
    MyGrid = $("#DTGrid")
    initialGrid();
    // addbtns();
    $("#Typ").select2(
   {
       placeholder: "Choose Action Type ",
       minimumInputLength: 0,
       multiple: false,
       maximumSelectionSize: 5,
       data: [
      { id: 1, text: "Track" },
      { id: 2, text: "Follow Up" }
     

       ]


   });
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
    var name = $("#Typ").select2("val");
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
    $("#Name").val("");
    $("#Typ").select2("val");
    var postData = MyGrid.getGridParam('postData')
    postData.filters = "";
    MyGrid.setGridParam('postData', postData)
    MyGrid.trigger('reloadGrid');
}

function initialGrid() {

    MyGrid.jqGrid({
        url: "/Indexs/GetActionsToGrid",
        datatype: 'json',
        mtype: 'Get',
        colNames: [ 'ID','Tools','Name', 'Code','Type'],
        colModel: [
             { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },
              { name: 'tools', index: 'tools', width: 60, align: "center", sortable: false, search: false },
             { key: true, name: 'Name', index: 'Name', editable: true },
             { key: false, name: 'ACTION_CODE', index: 'ACTION_CODE', editable: true },
             { key: false, name: 'ActionType_Name', index: 'ActionType_Name', editable: true },
           ],
        pager: jQuery('#pager'),
        rowNum: 10,
        rowList: [10, 20, 50, 100],
        height: '100%',
        viewrecords: true,
        caption: 'Actions Managements',
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

function addAction(elem) {
    openDialog({
        url: "Indexs/CreateWindow",
        title: "Add New Action",
        extraattr: { iscreateroledlog: "true" },
        width: 650,
       height: 300
             
    })
}
function EditAction(rowID) {
    openDialog({
        url: "/Indexs/UpdateWindow/?id=" + rowID,
        title: "Update Action",
        extraattr: { iscreateroledlog: "true" }

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
    itemMap.Update_Action = { name: "Update_Action", icon: "Update_Action", _name: "Update Action" }
   // itemMap.CanActivate = { name: "CanActivate", icon: "CanActivate", _name: "Update Track" }




    $("img[data-id=" + RowId + "]").closest("td").off('contextmenu')
    initialContextMenu({
        gridSelector: $("img[data-id=" + RowId + "]").closest("td")
                      , buttonSelector: "img[data-id=" + RowId + "]"
                      , trigger: "left"
                      , callback: function (key, options) {
                          MyGrid.setSelection(RowId);
                          // alert("In Intial Cont Invoke :" + RowId);
                          if (key == "Update_Action") {
                              EditAction(RowId);
                          }
                          //else if (key == "CanActivate") {
                          //    cancel_Activate(RowId);
                          //}

                      }
                      , items: itemMap
    });
}


   


   

