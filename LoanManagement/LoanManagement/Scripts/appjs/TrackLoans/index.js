

var MyGrid = null;
var loansGrid = null;

var UID = null;
function afterLoad(o) {
    MyGrid = $("#DTGrid")
    loansGrid=$("#LoansGrid")
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
   // MyGrid.(f.getFilter())
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
        url: "/TrackLoans/GetRODataToGrid",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID','RO Name','RO Code','No of Assign Trace', 'No of Traced'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },
             { key: false, name: 'RO_Data.RO_Name', index: 'RO_Data.RO_Name', editable: true },
              { key: false, name: 'RO_Data.RO_Code', index: 'RO_Data.RO_Code', editable: true },
           
             { key: false, name: 'NO_Tracks_U', index: 'NO_Tracks_U', editable: true },
              { key: false, name: 'NO_Tracked', index: 'NO_Tracked', editable: true },
              
            
        ],
      //  postData:{filters:UID.UID},
        pager: jQuery('#pager'),
        rowNum: 10,
        rowList: [10, 20, 50, 100],
        height: '100%',
        viewrecords: true,
        caption: 'Loan Trace for RO',
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
        onSelectRow: function (id) {
           // selectedROW = MyGrid.jqGrid('getGridParam', 'selrow');
            // loadiframe(currentKind, currentElem);
            selectMasterGrid();
        }
       
    }).navGrid('#pager', { edit: false, add: false, del: false, search: false, refresh: true });

    // end
    loansGrid.jqGrid({
        url: "/TrackLoans/GetLoanDataToGrid",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID', 'Client ID', 'Client Name', 'Tracking Notes','Assign Date','NO Follow' ,'NO of Call','NO of Visit','Status'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },
             { key: false, name: 'Account_ID', index: 'Loan_Info.AccountID', editable: true },
             { key: false, name: 'Client_Name', index: 'Client_Info.FULLNAME', editable: true },
              { key: false, name: 'Track_Note', index: 'Track_Note', editable: true },
               {
                   key: false, name: 'Tracking_Date', index: 'Tracking_Date', editable: true, formatter: 'date',
                   formatoptions: {
                       srcformat: 'dd/mm/yy',

                   }
               },
                 { key: false, name: 'NO_Follow', index: 'NO_Follow', editable: true },
             { key: false, name: 'NO_Call', index: 'NO_Call', editable: true },
              { key: false, name: 'NO_Visit', index: 'NO_Visit', editable: true },
               { key: false, name: 'Status', index: 'Status', editable: true }
             
             

        ],
        //  postData:{filters:UID.UID},
        pager: jQuery('#Loanspager'),
        rowNum: 10,
        rowList: [10, 20, 50, 100],
        height: '100%',
        viewrecords: true,
        caption: 'RO Detailed Loans',
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
           
        }
       
    }).navGrid('#Loanspager', { edit: false, add: false, del: false, search: false, refresh: true });

}
function selectMasterGrid() {
    selRowId = MyGrid.jqGrid('getGridParam', 'selrow')
    ID = MyGrid.jqGrid('getCell', selRowId, 'ID');
    
    var data = MyGrid.getRowData(selRowId);
    var postData = loansGrid.getGridParam("postData")
    //postData.filters = "CONTAINERID=" + data.ID
    postData.filters =  ID

    loansGrid.setGridParam({ postData: postData, page: 1 }).trigger('reloadGrid');
    loansGrid.setCaption('Loans Details For :&nbsp;&nbsp;&nbsp;' + data["RO_Data.RO_Name"])
  

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
   // $(".context-menu-list").remove()
    var RowId = $(elem).attr("data-id")
    var rowData = MyGrid.getRowData(RowId);
    var leg_state = MyGrid.jqGrid('getCell', RowId, 'lg_stae');
    var state = MyGrid.jqGrid('getCell', RowId, 'tracks.Status');
    var itemMap = {};
    //  alert(IsCentAdmin);
    if (state!="5") {
        itemMap.add_follow = { name: "add_follow", icon: "add_follow", _name: "Follow Up Loan" }
        if (leg_state == "1" || leg_state == "0") {
            itemMap.send_legal = { name: "send_legal", icon: "send_legal", _name: "Sending loan to legal" }
        }
    }
  
   
    itemMap.view_legal = { name: "view_legal", icon: "view_legal", _name: "View Legal Notes" }
   


    $("img[data-id=" + RowId + "]").closest("td").off('contextmenu')
    initialContextMenu({
        gridSelector: $("img[data-id=" + RowId + "]").closest("td")
                      , buttonSelector: "img[data-id=" + RowId + "]"
                      , trigger: "left"
                      , callback: function (key, options) {
                          MyGrid.setSelection(RowId);
                          // alert("In Intial Cont Invoke :" + RowId);
                          if (key == "add_follow") {
                              AddFollow(RowId);
                          } else if (key == "send_legal") {
                              SendToLegal(RowId);
                          } else if (key == "view_legal") {
                              view_legal(RowId);
                          }
                        
                        
                      }
                      , items: itemMap
    });
}

function AddFollow(rowID) {
   
    var ACCID = MyGrid.jqGrid('getCell', rowID, 'Loan_Info.AccountID');
    if (ACCID != false) {
        openDialog({
            url: "/FollowUp/AddFollow/?id=" + rowID,
            title: "Follow Up Loan_"+ACCID,
            extraattr: { iscreateroledlog: "true" },
            width: 1200,
            height: 400

        })

    }

  
}

function view_legal(rowID) {
    var ACCID = MyGrid.jqGrid('getCell', rowID, 'Loan_Info.AccountID');
    //   alert(ACCID);
    if (ACCID != false) {
        openDialog({
            url: "/FollowUp/View_Legal_Notes/?id=" + ACCID,
            title: "View Legal Notes",
            extraattr: { iscreateroledlog: "true" },
            width: 700,
            height: 200

        })
    }
}


function SendToLegal(rowID) {

    openDialog({
        url: "/FollowUp/SendToLegal/?id=" + rowID,
        title: "Sending To Legal Dept",
        extraattr: { iscreateroledlog: "true" },
        width: 700,
        height: 200

    })
}










