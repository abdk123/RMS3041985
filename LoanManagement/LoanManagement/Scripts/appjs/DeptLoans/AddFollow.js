
var loanID = null;
var Record = null;
var ClientGrid = null;
var btnMore = null;
var btnLess = null;
var currentIndex = 2;
var NoShowItems = 2;
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
    ///////////
    var trs = $("[guranrowitem='true']");
    btnMore = $("#seeMoreRecords");
    btnLess = $("#seeLessRecords");
    var trsLength = trs.length;

    //btnMore.click(function (e) {
    //    e.preventDefault();
    //    $("[guranrowitem='true']").slice(currentIndex, trsLength).show();
    //    currentIndex = trsLength;
    //    // alert((currentIndex - trsLength))
    //    //alert(currentIndex)


    //    var trs = $("[guranrowitem='true']");

    //    var trsLength = trs.length;
    //    // alert("L"+trsLength)
    //    checkButton();
    //});

    //btnLess.click(function (e) {
    //    e.preventDefault();
    //    //alert((currentIndex - trsLength))
    //    //alert(currentIndex)
    //    //alert(trsLength)
    //    $("[guranrowitem='true']").slice(NoShowItems, trsLength).hide();
    //    currentIndex = NoShowItems;
    //    //  alert("m" + currentIndex)
    //    // alert((trsLength * -1)+":"+ (NoShowItems * -1))
    //    checkButton();
    //});

    //////////////
    $("#Receiving_Date").datepicker({ dateFormat: 'dd/mm/yy' });
    getData({
        data: { TrackID: loanID },
        url: "/FollowUp/GetDataToAddFollow"
       , onsuccess: function (data) {

           // alert(JSON.stringify(data))
           Record = data;
           // fillEditViewdata({ container: $("[iscontainer]"), data: data })
           fillEditViewdata({ container: $("[labelloancontainer]"), data: data["Loan_Info"] })
           //  fillEditViewdata({ container: $("[labelGuarancontainer]"), data: data["guarant"] })
           fillEditViewdata({ container: $("[clientinfocontainer]"), data: data["Client_Info"] })

           fillEditViewdata({ container: $("[Scheduledcontainer]"), data: data["Client_Loan_Info"] })
           //   alert(JSON.stringify(data["Client_Info"]))
           $("#dat_down").html(data["dat_down"]);
           if (data["Client_Loan_Info"].Scheduled != null) {
               $("#schedule-button").prop('disabled', true);
           }
           $("#ID").val(data.ID);
           getData({
               data: { ACCID: data["Loan_Info"].AccountID },
               url: "/FollowUp/GetGuruantees"
       , onsuccess: function (data1) {

           if (data1) {
               $("[guranrowitem='true']").detach();
               $("[gurancontrol='true']").detach();
               $("[guruanteeData='true']").after(data1.htdata);
               //Here Should Apply Solutions











           }
           // alert(JSON.stringify(data))


       }
           })

       }
    })

    $("[guranrowitem='true']").hide();
    $("[guranrowitem='true']").slice(0, NoShowItems).show();
    // alert($("[guranrowitem='true']").length + ":" + $("[guranrowitem='true']:visible").length)
    checkButton();


    initialAutoComplete2();
    $("input#Reminder_Date").datepicker({ dateFormat: 'dd/mm/yy' });
    initialGrid();
    $('#Scheduled').change(function () {
        if ($(this).is(":checked")) {
            //
            $("#Down_Payment").prop('disabled', false);
            $("#Reschedule_Amount").prop('disabled', false);
            $("#Monthly_Payment").prop('disabled', false);
            $("#Duration").prop('disabled', false);
            $("#Schedule_Date").prop('disabled', false);
        } else {
            $("#Down_Payment").prop('disabled', true);
            $("#Reschedule_Amount").prop('disabled', true);
            $("#Monthly_Payment").prop('disabled', true);
            $("#Duration").prop('disabled', true);
            $("#Schedule_Date").prop('disabled', true);
        }

    });
    $("input#Schedule_Date").datepicker({ dateFormat: 'dd/mm/yy' });
};

//////////////////////////////

function showmore() {
    $("[guranrowitem='true']").slice(currentIndex, trsLength).show();
    currentIndex = trsLength;
    // alert((currentIndex - trsLength))
    //alert(currentIndex)


    var trs = $("[guranrowitem='true']");

    var trsLength = trs.length;
    // alert("L"+trsLength)
    checkButton();
}

function showLess() {
    var trs = $("[guranrowitem='true']");

    var trsLength = trs.length;
    $("[guranrowitem='true']").slice(NoShowItems, trsLength).hide();
    currentIndex = NoShowItems;
    //  alert("m" + currentIndex)
    // alert((trsLength * -1)+":"+ (NoShowItems * -1))
    checkButton();
}


function checkButton() {
    var currentLength = $("[guranrowitem='true']:visible").length;
    var trsLength = $("[guranrowitem='true']").length;
    // alert(currentLength + ":" + trsLength);
    if (currentLength < trsLength) {
        $("#seeMoreRecords").show();
        $("#seeLessRecords").hide();


    } else {
        //   alert("Equal Or More");
        $("#seeMoreRecords").hide();
        $("#seeLessRecords").show();

    }



}



///////////////////////////








function ViewGurunteesData(elem) {
    var ACCID = $(elem).attr("AccID");
    openDialog({
        url: "/FollowUp/ViewGuruantees/?ACCID=" + ACCID,
        title: "Loan Guruantees",
        extraattr: { iscreateroledlog: "true" },
        position: {
            my: 'top',
            at: 'top',
            collision: 'flip',
            // ensure that the titlebar is never outside the document
            //using: function(pos) {
            //    var topOffset = $(this).css(pos).offset().top;
            //    if (topOffset < 0) {
            //        $(this).css('top', pos.top - topOffset);
            //    }
            //}
        },
        width: 800,
        height: 500

    })
    ViewGuruantees
    //  alert(ACCID);
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

function saveClientData(elem) {
    var o = getDataObject({ container: $("[clientinfocontainer]") });
    var client = o;
    //commented from abd alrahman
    // var res = CheckIfClientChange(Record["Client_Info"], client);

    //if (!res) {

    client["TrackID"] = $("#ID").val();
    client["FULLNAME"] = $("#FULLNAME").html();
    client["CLIENTID"] = Record.Client_Loan_Info.ClientID;
    client["ACCOUNTID"] = Record.Client_Info.ACCOUNTID;

    SaveData({
        url: '/ClientManagement/InsertNewClientInfo'
       , container: $("[clientinfocontainer]")
      , data: client
      , onsuccess: function (data) {

          showMessage({ message: data.Message, title: "Alert Message" });
          ClientGrid.setGridParam().trigger('reloadGrid');
          //  window.parent.ReloadGrid({ action: "closecreate", success: 1 });
          Record["Client_Info"] = o;
      }
    });
    //} else {
    //    showMessage({ message: "No Changes made", title: "Alert Message" });
    //}


}
function CheckIfClientChange(newClient, oldClient) {
    if (newClient["ADDR"] === oldClient["ADDR"] && newClient["MOBILE"] === oldClient["MOBILE"] && newClient["TEL"] === oldClient["TEL"] && newClient["ADDITIONINFO"] === oldClient["ADDITIONINFO"]) {
        // alert("Maping")
        return true;
    }
    //  alert("Not Mapping")
    return false;
}
function saveFollowData(elem) {
    var o = getDataObject({ container: $("[followupcontainer]") });
    var follow = o;
    //Follow_Action
    //   follow["Remainder_Date"] = $("input#Reminder_Date").datepicker({ dateFormat: 'dd/mm/yyyy' }).val()
    follow["TrackID"] = $("#ID").val();
    follow["Follow_Action"] = $("#Follow_Action").select2("val");
    follow["ACCOUNTID"] = Record.Client_Loan_Info.AccountID;
    // alert(JSON.stringify(Record.Client_Loan_Info));
    //AccountID
    //return;
    SaveData({
        url: '/FollowUp/InsertFollowUp'
       , container: $("[clientinfocontainer]")
      , data: follow
      , onsuccess: function (data) {

          showMessage({ message: data.Message, title: "Alert Message" });
          MyGrid.setGridParam().trigger('reloadGrid');
          $("#Follow_Note").val("");
          $("#Reminder_Date").val("");
          $("#Reminder_Note").val("");

          // $("[iscreateroledlog]").dialog("close");
      }
    })



}
function saveSchedule(elem) {

    var o = getDataObject({ container: $("[Scheduledcontainer]") });
    o["CLID"] = Record["Client_Loan_Info"].ID;
    o["Scheduled"] = $('#Scheduled').attr('checked') ? "True" : "False";
    o["ACCOUNTID"] = Record["Client_Loan_Info"].AccountID;
    alert(o["Scheduled"]);
    SaveData({
        url: '/FollowUp/InsertSchedule'
      , container: $("[Scheduledcontainer]")
     , data: o
     , onsuccess: function (data) {

         showMessage({ message: data.Message, title: "Alert Message" });
         if (data["LData"]) {
             fillEditViewdata({ container: $("[Scheduledcontainer]"), data: data["LData"] })
         }
         // window.parent.ReloadGrid({ action: "closecreate", success: 1 });
         //here should bind data


     }
    })

}

function PrintFollow(elem) {
    var o = getDataObject({ container: $("[followupcontainer]") });
    var follow = o;
    
    follow["ACCOUNTID"] = Record.Client_Loan_Info.AccountID;
    var url='/Report/GetTrackingForRCurrentRo?accountId='+ follow["ACCOUNTID"];
    window.open(url, "_blank")
    //$.ajax({
    //    url: '/Report/GetTrackingForRCurrentRo',
    //    type: 'GET',
    //    data: { accountId: follow["ACCOUNTID"] },
    //    success: function (data) {

    //    }
    //});
   
}

function initialGrid() {
    var tid = Record.ID;
    MyGrid.jqGrid({
        url: "/FollowUp/GetDataToGridAddFollow",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID', 'Follow Date', 'Follow Action', 'Follow Note', 'Remainder Date', 'Remainder Note','RO Name'],
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
             { key: false, name: 'RO_Name', index: 'RO_Name', editable: false }


        ],
        postData: { filters: tid },
        pager: jQuery('#pager'),
        rowNum: 10,

        rowList: [10, 20, 50, 100],
        height: '100%',
        viewrecords: true,
        caption: 'Daily to follow up ',
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

    //update by abd alrahman
    ClientGrid.jqGrid({
        url: "/ClientManagement/GetAllClientData",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID', 'Another Address', 'Another Mobile', 'Another TEL', 'Receiving Date', 'Previous RO', 'Remarks', 'Additional Info', 'Relation With Other Banks'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },



             { key: false, name: 'Another_Address', index: 'Another_Address', editable: true },
              { key: false, name: 'Another_Mobile', index: 'Another_Mobile', editable: true },
               { key: false, name: 'Another_TEL', index: 'Another_TEL', editable: true },
              {
                  key: false, name: 'Receiving_Date', index: 'Receiving_Date', editable: true, formatter: 'date',
                  formatoptions: {
                      srcformat: 'dd/mm/yy',

                  }
              },
               { key: false, name: 'Previous_RO', index: 'Previous_RO', editable: true },
               { key: false, name: 'Remarks', index: 'Remarks', editable: true },
             { key: false, name: 'ADDITIONINFO', index: 'ADDITIONINFO', editable: true },
             { key: false, name: 'Relation_With_Other_Banks', index: 'Relation_With_Other_Banks', editable: true },


        ],
        postData: { filters: loanID },
        pager: jQuery('#clientpager'),
        rowNum: 10,
        rowList: [10, 20, 50, 100],
        height: '100px',
        width: '250px',
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

