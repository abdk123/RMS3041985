
var loanID = null;
var Record = null;
function initialAutoComplete2() {
  
    var ResultData = null;
    getData({
        data: { noteID: loanID },
        url: "/Legal/GetSendingPapersToLegal"
      , onsuccess: function (data) {
          ResultData = data;
        
      }
    })

   
    $("#papers").select2(
     {
         placeholder: "Papers Sended to legal ",
         minimumInputLength: 0,
         multiple: true,
         maximumSelectionSize: 5,
         data: ResultData


     });

  
}



function afterLoad(o) {
  //  initialAutoComplete2();
    loanID = $.getUrlVar("id", "")
    //Here Should 



    initialAutoComplete2();
    
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
    var o = getDataObject({ container: $("[iscontainer]") });
   
    var papers = $("#papers").select2("data");
    o["LegalNoteID"] = loanID;
    o["papers"] = JSON.stringify(papers);
  
    SaveData({
        url: '/Legal/ReceivePapers'
       , container: $("[iscontainer]")
      , data: o
      , onsuccess: function (data) {

        //  showMessage({ message: data.Message, title: "Alert Message" });
         window.parent.ReloadGrid({ action: "closecreate", success: 1 });
      }
    })
    


}

