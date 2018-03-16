
var legalNoteID = null;
var Record = null;
var accid = null;



function afterLoad(o) {
    //  initialAutoComplete2();
   
    legalNoteID = $.getUrlVar("id", "")
  //  accid = $.getUrlVar("AccID", "")
  //  alert(legalNoteID+":"+accid);

    ////Here Should 
    //getData({
    //    data: { LegID: accid },
    //     url: "/Legal/GetLegalNote"
    //  , onsuccess: function (data) {
    //      if (data) {
    //          Record = data;
    //          // fillEditViewdata({ container: $("[iscontainer]"), data: data })


    //          fillEditViewdata({ container: $("[labelrepcontainer]"), data: data })

    //          getData({
    //              data: { LegID: data.ID },
    //              url: "/Legal/GetLegalProcedure"
    //            , onsuccess: function (data) {
    //                $("[legproitem='true']").detach();

    //                // fillEditViewdata({ container: $("[iscontainer]"), data: data })
    //                //  alert(data.htdata);
    //                $("[procedureData]").after(data.htdata);




    //            }
    //          })
    //      }
    //     // alert(JSON.stringify(data))
        

    //  }
    //})
   
   
   
    
};


function ReloadGrid(options) {
    var action = options.action
    switch (action) {
        case "closecreate":
            $("[iscreateroledlog]").dialog("close");
            if (options.success)
                ShowNotificationMsg({ msg: "Operation Succ" });
            break;
    }
    MyGrid.setGridParam().trigger('reloadGrid');
}

function savetData(elem)
{
    var o = getDataObject({ container: $("[legalnotecontainer]") });
   
  
    o["LegalNoteID"] = legalNoteID;
   
  
    SaveData({
        url: '/Legal/AddLegalNote'
       , container: $("[iscontainer]")
      , data: o
      , onsuccess: function (data) {

        //  showMessage({ message: data.Message, title: "Alert Message" });
         window.parent.ReloadGrid({ action: "closecreate", success: 1 });
      }
    })
    


}

