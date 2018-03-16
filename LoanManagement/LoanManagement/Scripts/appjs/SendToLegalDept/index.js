

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
        url: "/SendToLegalDept/GetDataToGrid",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID','Tools', 'Client Acc', 'Client Name', 'Send State','Receive Date','Sending Date', 'RO Note','Legal Note','Legal Name','Legal Action'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },
            
          { name: 'tools', index: 'tools', width: 60, align: "center", sortable: false, search: false },
             { key: false, name: 'Client_Info.AccountID', index: 'Loan_Info.AccountID', editable: true },
             { key: false, name: 'Client_Info.FULLNAME', index: 'Client_Info.FULLNAME', editable: true },
              { key: false, name: 'legal_state', index: 'legal_state', editable: true },
               {
                   key: false, name: 'tracks.LegalReceivDate', index: 'tracks.LegalReceivDate', editable: true, formatter: 'date',
                   formatoptions: {
                       srcformat: 'dd/mm/yy',

                   }
               },
               {
                   key: false, name: 'legalNote.SendingDate', index: 'legalNote.SendingDate', editable: true, formatter: 'date',
                   formatoptions: {
                       srcformat: 'dd/mm/yy',

                   }
               },
             { key: false, name: 'legalNote.EmpNotes', index: 'legalNote.EmpNotes', editable: true },
              { key: false, name: 'legalNote.LegalNote1', index: 'legalNote.LegalNote1', editable: true },
              { key: false, name: 'legal_usr.name', index: 'legal_usr.name', editable: true },
              { key: false, name: 'legalNote.Legal_Action', index: 'legalNote.Legal_Action', editable: true },
             

        ],
      //  postData:{filters:UID.UID},
        pager: jQuery('#pager'),
        rowNum: 10,
        rowList: [10, 20, 50, 100],
        height: '100%',
        viewrecords: true,
        caption: 'Tracks to follow up ',
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
    $(".context-menu-list").remove()
    var RowId = $(elem).attr("data-id")
    var rowData = MyGrid.getRowData(RowId);
   
    var itemMap = {};
    //  alert(IsCentAdmin);
    itemMap.close = { name: "close", icon: "close", _name: "Close Loan" }
   
    
   


    $("img[data-id=" + RowId + "]").closest("td").off('contextmenu')
    initialContextMenu({
        gridSelector: $("img[data-id=" + RowId + "]").closest("td")
                      , buttonSelector: "img[data-id=" + RowId + "]"
                      , trigger: "left"
                      , callback: function (key, options) {
                          MyGrid.setSelection(RowId);
                          // alert("In Intial Cont Invoke :" + RowId);
                          if (key == "close") {
                              close(RowId);
                          } 
                        
                        
                      }
                      , items: itemMap
    });
}

function close(rowID) {
    var trck = MyGrid.jqGrid('getCell', rowID, 'ID');
    var da = {TrackID:trck};
  //  CloseLoan
    SaveData({
        url: '/FollowUp/CloseLoan'
           , container: $("#container")
          , data: da
          , onsuccess: function (data) {

              showMessage({ message: "Loan Has Been Closed", title: "Alert Message" });
              MyGrid.setGridParam().trigger('reloadGrid');
              //  window.parent.ReloadGrid({ action: "closecreate", success: 1 });
             // ReloadGrid({ action: "closecreate", success: 1 });
          }
    })
}








