

var MyGrid = null;

function afterLoad(o) {
    MyGrid = $("#DTGrid")
    initialGrid();
  //  addbtns();
  //  $('#tab-container').easytabs({ uiTabs: true });
}

function ReloadGrid(options) {
    var action = options.action
    switch (action) {
        case "closecreate":
            $("[iscreateroledlog]").dialog("close");
            if (options.success)
                ShowNotificationMsg({ msg: "Operation Successes" });
            break;
    }
    MyGrid.setGridParam().trigger('reloadGrid');
}
function Search(elem) {
    var accID = $("#ClientID").val();
    var cname = $("#ClientN").val();
   // alert(accID);
  //  return;
   // var f = tbl.getFilter()
    //var grid = tbl.grid
    var postData = MyGrid.getGridParam('postData')
    postData.filters = accID+"&"+cname;
    MyGrid.setGridParam('postData', postData)
    MyGrid.trigger('reloadGrid');
    //MyGrid.(f.getFilter())
}
function Clear(elem)
{
     $("#ClientID").val("");
     $("#ClientN").val("");
    var postData = MyGrid.getGridParam('postData')
    postData.filters ="";
    MyGrid.setGridParam('postData', postData)
    MyGrid.trigger('reloadGrid');
}
function initialGrid() {
   
    MyGrid.jqGrid({
        url: "/TracksManagement/GetDataToGrid",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID','StatusID','Tools', 'Client Acc', 'Client Name', 'Status', 'Branch',  'Tracking Date',  'Current RO'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },
             { key: false, hidden: true, name: 'StatusID', index: 'StatusID', editable: true },
          { name: 'tools', index: 'tools', width: 60, align: "center", sortable: false, search: false },
             { key: false, name: 'Client_Acc', index: 'Client_Acc', editable: true },
             { key: false, name: 'Client_Name', index: 'Client_Name', editable: true },
              { key: false, name: 'Status', index: 'Status', editable: true },
               { key: false, name: 'Branch', index: 'Branch', editable: true },
          
              {
                  key: false, name: 'Tracking_Date', index: 'Tracking_Date', editable: true, formatter: 'date',
                  formatoptions: {
                      srcformat: 'dd/mm/yy',

                  }
              },
              
                { key: false, name: 'RO', index: 'RO', editable: true }
                

        ],
      //  postData:{filters:"Test Filters"},
        pager: jQuery('#pager'),
        rowNum: 10,
        rowList: [10, 20, 50, 100],
        height: '100%',
        viewrecords: true,
        caption: 'Remedial Loans',
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
            tg_addImageButton({ grid: this, RowId: RowId, CellKey: "tools", title: "الأدوات", evt: "InitContext(this);return false;", src: 'Content/images/setting.ico' })

        }
       
    }).navGrid('#pager', { edit: false, add: false, del: false, search: false, refresh: true }
      );
    // end
    

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
   // alert(JSON.stringify(rowData));
    if (rowData.StatusID=="1") {
        itemMap.add_track = { name: "add_track", icon: "add_track", _name: "Assign To User" }
    } else if(rowData.StatusID=="2") {
        itemMap.update_track = { name: "update_track", icon: "update_track", _name: "Update User" }
    }
   
    itemMap.History = { name: "History", icon: "History", _name: "Loan History" }
   
   


    $("img[data-id=" + RowId + "]").closest("td").off('contextmenu')
    initialContextMenu({
        gridSelector: $("img[data-id=" + RowId + "]").closest("td")
                      , buttonSelector: "img[data-id=" + RowId + "]"
                      , trigger: "left"
                      , callback: function (key, options) {
                          MyGrid.setSelection(RowId);
                          // alert("In Intial Cont Invoke :" + RowId);
                          if (key == "add_track") {
                              AddTrack(RowId);
                          }
                          else if (key == "update_track") {
                              UpdateTrack(RowId);
                          } else if (key == "History") {
                             
                              ViewHistory(RowId);
                          }
                        
                      }
                      , items: itemMap
    });
}

function AddTrack(rowID) {

    openDialog({
        url: "/TracksManagement/AddTrack/?id=" + rowID,
        title: "Assign Loan To User",
        extraattr: { iscreateroledlog: "true" },
        width: 450,
        height:400

    })
}
function UpdateTrack(rowID) {
    openDialog({
        url: "/TracksManagement/UpdateTrack/?id=" + rowID,
        title: "Re-Assign Loan To User",
        extraattr: { iscreateroledlog: "true" },
        width: 450,
        height: 400

    })
}

function ViewHistory(rowID) {

    openDialog({
        url: "/TracksManagement/History/?id=" + rowID,
        title: "Loan History",
        extraattr: { iscreateroledlog: "true" },
        width: 1200,
        height: 400

    })
}











