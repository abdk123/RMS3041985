
var loanID = null;
var Record = null;
var ClientGrid = null;
function initialAutoComplete2() {
    $("#Follow_Action").select2(
      {
          placeholder: "Follow Action",
          minimumInputLength: 0,
          multiple: false,
          maximumSelectionSize: 5,
          ajax: {
              url: '/FollowUp/getFollowAction',
              dataType: 'json',
              data: function (term, page) {
                  return {
                      searchTerm: term,
                      page_limit: 10,
                      page: page,
                  };
              },
              results: function (data, page) {

                  var more = (page * 10) < data.total;
                  return { results: data, more: more };
              }
          }
      });
    
    
   
}


function afterLoad(o) {
  //  initialAutoComplete2();
    loanID = $.getUrlVar("id", "")
    MyGrid = $("#DTGrid")
    ClientGrid = $("#ClientGrid")
   
 
    getData({
        data: { TrackID: loanID },
        url: "/FollowUp/GetDataToAddFollow"
       , onsuccess: function (data) {
          
           Record = data;
           // fillEditViewdata({ container: $("[iscontainer]"), data: data })
           fillEditViewdata({ container: $("[labelloancontainer]"), data: data["Loan_Info"] })
           fillEditViewdata({ container: $("[labelGuarancontainer]"), data: data["guarant"] })
           fillEditViewdata({ container: $("[clientinfocontainer]"), data: data["Client_Info"] })
           $("#ID").val(data.ID);
        
           
       }
    })

    initialAutoComplete2();
    $("input#Reminder_Date").datepicker({ dateFormat: 'dd/mm/yy' });
    initialGrid();
};


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

function saveClientData(elem)
{
    var o = getDataObject({ container: $("[clientinfocontainer]") });
    var client = o;
    client["TrackID"] = $("#ID").val();
    client["FULLNAME"] = $("#FULLNAME").html();
    client["CLIENTID"] = Record.Client_Info.ID;
    SaveData({
        url: '/ClientManagement/InsertNewClientInfo'
       , container: $("[clientinfocontainer]")
      , data: client
      , onsuccess: function (data) {

          showMessage({ message: data.Message, title: "Alert Message" });
          window.parent.ReloadGrid({ action: "closecreate", success: 1 });

      }
    })
    


}

function saveFollowData(elem) {
    var o = getDataObject({ container: $("[followupcontainer]") });
    var follow = o;
    //Follow_Action
 //   follow["Remainder_Date"] = $("input#Reminder_Date").datepicker({ dateFormat: 'dd/mm/yyyy' }).val()
    follow["TrackID"] = $("#ID").val();
    follow["Follow_Action"] = $("#Follow_Action").select2("val");
  
    SaveData({
        url: '/FollowUp/InsertFollowUp'
       , container: $("[clientinfocontainer]")
      , data: follow
      , onsuccess: function (data) {

          showMessage({ message: data.Message, title: "Alert Message" });
          MyGrid.setGridParam().trigger('reloadGrid');
         // $("[iscreateroledlog]").dialog("close");
      }
    })



}
function initialGrid() {
    var tid = Record.ID;
    MyGrid.jqGrid({
        url: "/FollowUp/GetDataToGridAddFollow",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID', 'Follow Date', 'Follow Action', 'Follow Note', 'Remainder Date', 'Remainder Note'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },

         
             {
                 key: false, name: 'Follow_Date', index: 'Follow_Date', editable: true, formatter: 'date',
                 formatoptions: {
                     srcformat: 'dd/mm/yy',

                 }
             },
             { key: false, name: 'Follow_Action', index: 'Follow_Action', editable: true },
              { key: false, name: 'Follow_Note', index: 'Follow_Note', editable: true },
               {
                   key: false, name: 'Remainder_Date', index: 'Remainder_Date', editable: true, formatter: 'date',
                   formatoptions: {
                       srcformat: 'dd/mm/yy',

                   }
               },
             { key: false, name: 'Remainder_Note', index: 'Remainder_Note', editable: true },


        ],
        postData: { filters: tid },
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
          //  tg_addImageButton({ grid: this, RowId: RowId, CellKey: "tools", title: "الأدوات", evt: "InitContext(this);return false;", src: 'Content/images/setting.ico' })
         
        }

    }).navGrid('#pager', { edit: false, add: false, del: false, search: false, refresh: true }
      );
    // end

    //Client Grid
    ClientGrid.jqGrid({
        url: "/ClientManagement/GetAllClientData",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID', 'Full Name', 'Address', 'Mobile', 'Tel', 'Additional Info'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },


             { key: false, name: 'FULLNAME', index: 'FULLNAME', editable: true },
             { key: false, name: 'ADDR', index: 'ADDR', editable: true },
              { key: false, name: 'MOBILE', index: 'MOBILE', editable: true },
               { key: false, name: 'TEL', index: 'TEL', editable: true },
             { key: false, name: 'ADDTIONINFO', index: 'ADDTIONINFO', editable: true },


        ],
          postData:{filters:loanID},
        pager: jQuery('#pager'),
        rowNum: 10,
        rowList: [10, 20, 50, 100],
        height: '100%',
        viewrecords: true,
        caption: 'Previous Client Info',
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
            //  tg_addImageButton({ grid: this, RowId: RowId, CellKey: "tools", title: "الأدوات", evt: "InitContext(this);return false;", src: 'Content/images/setting.ico' })

        }

    }).navGrid('#clientpager', { edit: false, add: false, del: false, search: false, refresh: true }
     );



}

