

var MyGrid = null;

var UID = null;
function afterLoad(o) {
    MyGrid = $("#DTGrid")
 //   UID = GetUserInfoData();
    initialGrid();
  
  
}

function ReloadGrid(options) {
    var action = options.action
    switch (action) {
        case "closecreate":
            $("[iscreateroledlog]").dialog("close");
            if (options.success)
                ShowNotificationMsg({ msg: "تمت العملية بنجاح" });
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
  //  MyGrid.(f.getFilter())
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
        url: "/Legal/GetDataToGrid",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID', 'legalStateID', 'track_state', 'Tools', 'Client ID', 'Client Name', 'RO Name', 'Employee Note', 'Received Date','Sending Date','Status','Legal Action'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },
              { key: true, hidden: true, name: 'Track.SendLegalState', index: 'Track.SendLegalState', editable: true },
               { key: true, hidden: true, name: 'track_status', index: 'track_status', editable: true },
          { name: 'tools', index: 'tools', width: 60, align: "center", sortable: false, search: false },
             { key: false, name: 'Acc_ID', index: 'Loan_Info.AccountID', editable: true },
             { key: false, name: 'Client_Name', index: 'Client_Name', editable: true },
               { key: false, name: 'RO_Name', index: 'RO_Name', editable: true },
              { key: false, name: 'Note', index: 'Note', editable: true },
            {
                key: false, name: 'receiv_date', index: 'receiv_date', editable: true, formatter: 'date',
                formatoptions: {
                    srcformat: 'dd/mm/yy',

                }
            },
             {
                 key: false, name: 'SendingDate', index: 'SendingDate', editable: true, formatter: 'date',
                 formatoptions: {
                     srcformat: 'dd/mm/yy',

                 }
             },
                 { key: false, name: 'legal_state', index: 'legal_state', editable: true },
                  { key: false, name: 'Legal_Action', index: 'Legal_Action', editable: true }
               
        ],
      //  postData:{filters:UID.UID},
        pager: jQuery('#pager'),
        rowNum: 10,
        rowList: [10, 20, 50, 100],
        height: '100%',
        viewrecords: true,
        caption: 'Loans Send To Legals ',
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
  //  $(".context-menu-list").remove();
    var RowId = $(elem).attr("data-id")
    var rowData = MyGrid.getRowData(RowId);
    var st = MyGrid.jqGrid('getCell', RowId, 'Track.SendLegalState');
  //  alert(JSON.stringify(rowData));
    var itemMap = {};
    //  alert(IsCentAdmin);
  
   // alert(st)
    if (st=="2") {
        itemMap.Receive_Legal = { name: "Receive", icon: "Receive", _name: "Receive Papers" }
    }
    if (st == "3") {
        
        itemMap.add_legal = { name: "add_legal", icon: "add_legal", _name: "Add Legal Notes" }
    }
  
    itemMap.view_legal = { name: "view_legal", icon: "view_legal", _name: "View Legal Notes" }
   
   // alert(rowData.legal_state);

    //$("img[data-id=" + RowId + "]").closest("td").off('contextmenu')
    initialContextMenu({
        gridSelector: $("img[data-id=" + RowId + "]").closest("td")
                      , buttonSelector: "img[data-id=" + RowId + "]"
                      , trigger: "left"
                      , callback: function (key, options) {
                          MyGrid.setSelection(RowId);
                         // alert("In Intial Cont Invoke :" + RowId);
                          if (key == "Receive_Legal") {
                              Receive(RowId);
                          } else if (key == "view_legal") {
                              view_legal(RowId);
                          } else if (key == "add_legal") {
                              add_legal(RowId);
                          }
                        
                        
                      }///Here Error 2 click
                      , items: itemMap
    });
}




function Receive(rowID) {
    var ACCID = MyGrid.jqGrid('getCell', rowID, 'Acc_ID');
    if (ACCID != false) {

        openDialog({
            url: "/Legal/Receive/?id=" + rowID,
            title: "Receive Papers_" + ACCID,
            extraattr: { iscreateroledlog: "true" },
            width: 700,
            height: 300

        })

    }
   
}
function view_legal(rowID) {

    //Need To Test
    var ACCID = MyGrid.jqGrid('getCell', rowID, 'Acc_ID');
    var legID = MyGrid.jqGrid('getCell', rowID, 'ID');
 //   alert(ACCID);
    if (ACCID != false) {
        openDialog({
            url: "/Legal/View_Legal_Notes/?AccID=" + ACCID+"&id="+legID,
            title: "View Legal Notes_" + ACCID,
            extraattr: { iscreateroledlog: "true" },
            width: 700,
            height: 200

        })
    }
    //Acc_ID
   
}
function add_legal(rowID) {
  //  alert("kkkkkk")
    //Need To Test
   var ACCID = MyGrid.jqGrid('getCell', rowID, 'Acc_ID');
    var legID = MyGrid.jqGrid('getCell', rowID, 'ID');
    //   alert(ACCID);
    if (legID != false) {
        openDialog({
            url: "/Legal/Add_Legal_Notes/?id=" + legID,
            title: "Add Legal Notes_" + ACCID,
            extraattr: { iscreateroledlog: "true" },
            width: 500,
            height: 200

        })
    }
    //Acc_ID

}









