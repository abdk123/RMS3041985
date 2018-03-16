

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
        url: "/Legal/GetCasesDataToGrid",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID', 'Tools', 'Account ID', 'Lawyer', 'Receive Paper Date', 'Lawsuit NO 1', 'Lawsuit NO 2', 'Court', 'Lawsuit Type', 'Travel ban', 'Preventing Date'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },
            
          { name: 'tools', index: 'tools', width: 60, align: "center", sortable: false, search: false },
             { key: false, name: 'AccountID', index: 'AccountID', editable: true },
             { key: false, name: 'Lawer', index: 'Lawer', editable: true },
               { key: false, name: 'Date_paper_lawsuit', index: 'Date_paper_lawsuit', editable: true },
              { key: false, name: 'lawsuitNOFirst', index: 'lawsuitNOFirst', editable: true },
               { key: false, name: 'lawsuitNOSec', index: 'lawsuitNOSec', editable: true },
                { key: false, name: 'Court', index: 'Court', editable: true },
                 { key: false, name: 'lawsuit_typ', index: 'lawsuit_typ', editable: true },
                  { key: false, name: 'PrevintingTravel', index: 'PrevintingTravel', editable: true },
                     { key: false, name: 'PrevTravDate', index: 'PrevTravDate', editable: true },
               
        ],
      //  postData:{filters:UID.UID},
        pager: jQuery('#pager'),
        rowNum: 10,
        rowList: [10, 20, 50, 100],
        height: '100%',
        viewrecords: true,
        caption: 'All Lawsuit Data ',
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
            tg_addImageButton({ grid: this, RowId: RowId, CellKey: "tools", title: "Tools", evt: "InitContext(this);return false;", src: '../Content/images/setting.ico' })

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
   
    
  
    itemMap.view_proc = { name: "view_proc", icon: "view_proc", _name: "View Procedure Actions" }
   
   // alert(JSON.stringify(itemMap));

    //$("img[data-id=" + RowId + "]").closest("td").off('contextmenu')
    initialContextMenu({
        gridSelector: $("img[data-id=" + RowId + "]").closest("td")
                      , buttonSelector: "img[data-id=" + RowId + "]"
                      , trigger: "left"
                      , callback: function (key, options) {
                          MyGrid.setSelection(RowId);
                         // alert("In Intial Cont Invoke :" + RowId);
                          if (key == "view_proc") {
                              view_proc(RowId);
                          }
                          
                        
                        
                      }///Here Error 2 click
                      , items: itemMap
    });
}





function view_proc(rowID) {

    //Need To Test
    var ACCID = MyGrid.jqGrid('getCell', rowID, 'ID');
    var acc = MyGrid.jqGrid('getCell', rowID, 'AccountID');
 //   alert(ACCID);
    if (ACCID != false) {
        openDialog({
            url: "/Legal/view_procedure/?id=" + ACCID,
            title: "View Procedure Actions_"+acc,
            extraattr: { iscreateroledlog: "true" },
            width: 700,
            height: 200

        })
    }
    //Acc_ID
   
}










