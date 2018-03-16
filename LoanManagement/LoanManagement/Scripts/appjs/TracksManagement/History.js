
var loanID = null;
var Record = null;
var ClientGrid = null;



function afterLoad(o) {
  //  initialAutoComplete2();
    loanID = $.getUrlVar("id", "")
    MyGrid = $("#DTGrid")
    ClientGrid = $("#ClientGrid")
   
 
    getData({
        data: { TrackID: loanID },
        url: "/TracksManagement/GetDataToHistory"
       , onsuccess: function (data) {
          
           Record = data;
           // fillEditViewdata({ container: $("[iscontainer]"), data: data })
           fillEditViewdata({ container: $("[labelloancontainer]"), data: data["Loan_Info"] })
         //  fillEditViewdata({ container: $("[labelGuarancontainer]"), data: data["guarant"] })
           fillEditViewdata({ container: $("[clientinfocontainer]"), data: data["Client_Info"] })
           fillEditViewdata({ container: $("[Scheduledcontainer]"), data: data["Client_Loan_Info"] })
           // alert(JSON.stringify(data["Client_Loan_Info"]))
           $("#dat_down").html(data["dat_down"]);
           if (data["Client_Loan_Info"].Scheduled != null) {
               $("#schedule-button").prop('disabled', true);
           }
           $("#ID").val(data.ID);
           getData({
               data: { ACCID: data["Loan_Info"].AccountID },
               url: "/TracksManagement/GetGuruantees"
       , onsuccess: function (data1) {
        
           if (data1) {
               $("[guranrowitem='true']").detach();
               $("[guruanteeData='true']").after(data1.htdata);
              
           }
           // alert(JSON.stringify(data))


               }
           })
           
       }
    })

  
    initialGrid();
    //$('#Scheduled').change(function () {
    //    if ($(this).is(":checked")) {
    //        //
    //        $("#Down_Payment").prop('disabled', false);
    //    } else {
    //        $("#Down_Payment").prop('disabled', true);
    //    }
       
    //});


    //Here Scripts For Legal
    //  initialAutoComplete2();
  //  alert(JSON.stringify(Record["Loan_Info"]))
    //return;
    var legalNoteID = Record["Loan_Info"].AccountID;

    //Here Should 
    getData({
        data: { LegID: legalNoteID },
        url: "/TracksManagement/GetLegalNote"
      , onsuccess: function (data) {
          if (!data.Res) {
           
              // fillEditViewdata({ container: $("[iscontainer]"), data: data })
            //  alert(JSON.stringify(data))

              fillEditViewdata({ container: $("[labelrepcontainer]"), data: data })

              getData({
                  data: { LegID: data.ID },
                  url: "/TracksManagement/GetLegalProcedure"
                , onsuccess: function (data2) {
                    $("[legproitem='true']").detach();
                    if (data2) {
                        var cont = $("[lawsuitProcedure]");
                        cont.find("[procedureData]").after(data2.htdata);
                    }
                   
                  
                   




                }
              })

          } 
      }

    })

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


function saveSchedule(elem)
{
    var o = getDataObject({ container: $("[Scheduledcontainer]") });
    o["CLID"] = Record["Client_Loan_Info"].ID;
    o["Scheduled"] = $('#Scheduled').attr('checked') ? "True" : "False";
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

 



}

