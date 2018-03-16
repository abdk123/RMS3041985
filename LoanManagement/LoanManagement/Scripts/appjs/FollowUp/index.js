﻿

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
    debugger;
    var accID = $("#ClientID").val();
    var cname = $("#ClientN").val();
    // alert(accID);
    //  return;
    // var f = tbl.getFilter()
    //var grid = tbl.grid
    var postData = MyGrid.getGridParam('postData')
    postData.filters = accID + "&" + cname;
    MyGrid.setGridParam('postData', postData)
    MyGrid.trigger('reloadGrid');
    // MyGrid.(f.getFilter())
}
function Clear(elem) {
    $("#ClientID").val("");
    $("#ClientN").val("");
    var postData = MyGrid.getGridParam('postData')
    postData.filters = "";
    MyGrid.setGridParam('postData', postData)
    MyGrid.trigger('reloadGrid');
}
function initialGrid() {
    MyGrid.jqGrid({
        url: "/FollowUp/GetDataToGrid",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID', 'status', 'lg_state', 'Tools', 'Client ID', 'Client Name', 'Tracking Deatils', 'Tracking Date', 'Branch', 'Legal Status', 'Legal Received Date','Date_Rec','CDate','TEL','ADDR'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },
            { key: true, hidden: true, name: 'lg_stae', index: 'lg_stae', editable: true },
            { key: true, hidden: true, name: 'tracks.Status', index: 'tracks.Status', editable: true },
          { name: 'tools', index: 'tools', width: 60, align: "center", sortable: false, search: false },
             { key: false, name: 'Loan_Info.AccountID', index: 'Loan_Info.AccountID', editable: true },
             { key: false, name: 'Client_Info.FULLNAME', index: 'Client_Info.FULLNAME', editable: true },
             { key: false, name: 'trackers.Admin_Notes', index: 'trackers.Admin_Notes', editable: true },
             { key: false, name: 'tracks.Tracking_Date', index: 'tracks.Tracking_Date', editable: true, 
                 formatter: 'date',formatoptions: {
                                                    srcformat: 'dd/mm/yy',
                                                  }
               },
              { key: false, name: 'loan.Branch', index: 'loan.Branch', editable: true },
              { key: false, name: 'legal_state', index: 'legal_state', editable: true},
              {
                  key: false, name: 'tracks.LegalReceivDate', index: 'tracks.LegalReceivDate', editable: true, formatter: 'date',
                  formatoptions: {
                      srcformat: 'dd/mm/yy',

                  }
              },
               {
                   key: false, name: 'Loan_Info.Date_Rec', index: 'Loan_Info.Date_Rec', editable: true,
                   formatter: 'date', formatoptions: {
                       srcformat: 'dd/mm/yy',
                   }
               },
               {
                   key: false, name: 'Loan_Info.CDate', index: 'Loan_Info.CDate', editable: true,
                   formatter: 'date', formatoptions: {
                       srcformat: 'dd/mm/yy',
                   }
               },
               { key: false, name: 'Client_Info.TEL', index: 'Client_Info.TEL', editable: true },
               { key: false, name: 'Client_Info.ADDR1', index: 'Client_Info.ADDR1', editable: true },
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
            tg_addImageButton({ grid: this, RowId: RowId, CellKey: "tools", title: "Tools", evt: "InitContext(this);return false;", src: '/Content/images/setting.ico' })

        }

    }).navGrid('#pager', { edit: false, add: false, del: false },
        {},
        {},
        {},
        {multipleSearch:true});

    
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
    // $(".context-menu-list").remove()
    var RowId = $(elem).attr("data-id")
    var rowData = MyGrid.getRowData(RowId);
    var leg_state = MyGrid.jqGrid('getCell', RowId, 'lg_stae');
    var state = MyGrid.jqGrid('getCell', RowId, 'tracks.Status');
    var itemMap = {};
    //  alert(IsCentAdmin);
    if (state != "5") {
        itemMap.add_follow = { name: "add_follow", icon: "add_follow", _name: "Follow Up Loan" }
        if (leg_state == "1" || leg_state == "0") {
            itemMap.send_legal = { name: "send_legal", icon: "send_legal", _name: "Sending loan to legal" }
        }
    }


    itemMap.view_legal = { name: "view_legal", icon: "view_legal", _name: "View Legal Notes" }
    itemMap.view_history = { name: "view_history", icon: "view_history", _name: "View Loan History" }



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
                          else if (key == "view_history") {
                              View_Loan_History(RowId);
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
            title: "Follow Up Loan_" + ACCID,
            extraattr: { iscreateroledlog: "true" },
            width: 1200,
            height: 400

        })

    }


}

function View_Loan_History(rowID) {
    var ACCID = MyGrid.jqGrid('getCell', rowID, 'Loan_Info.AccountID');
    if (ACCID != false) {
        openDialog({
            url: "/FollowUp/View_History_Notes/?ACCID=" + ACCID,
            title: "Loan History_" + ACCID,
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










