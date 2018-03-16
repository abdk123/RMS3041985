
var loanID = null;
var Record = null;



function afterLoad(o) {
  //  initialAutoComplete2();
    loanID = $.getUrlVar("id", "")
  
    
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
    var client = o;
    client["TrakID"] = $("#ID").val();
  
    SaveData({
        url: '/Legal/SendDataToLegal'
       , container: $("[clientinfocontainer]")
      , data: client
      , onsuccess: function (data) {

          showMessage({ message: data.Message, title: "Alert Message" });
         // window.parent.ReloadGrid({ action: "closecreate", success: 1 });
      }
    })
    


}

