//checkboxFormatter to wire onclick event of checkbox

var userID = null;
function initialAutoComplete2() {
    $("#Typ").select2(
   {
       placeholder: "Chose User Type ",
       minimumInputLength: 0,
       multiple: false,
       maximumSelectionSize: 5,
       data: [
      { id: 1, text: "Track" },
      { id: 2, text: "Follow Up" }


       ]


   });

}


function afterLoad(o) {
    userID = $.getUrlVar("id", "")
  
    // alert(loanID);
    getData({
        data: { aID: userID },
        url: "/Indexs/GetActionByID"
       , onsuccess: function (data) {
           //  alert(JSON.stringify(data));
           Record = data;
        
           fillEditViewdata({ container: $("[iscontainer]"), data: data })
        
       }
    })
    initialAutoComplete2();
  
   
};



function saveAction(elem) {
    var o = getDataObject({ container: $("[iscontainer]") });
    o["ActionType"] = $("#Typ").select2("val");
    o["ID"] = userID;
    SaveData({
        url: '/Indexs/UpdateAction'
               , container: $("[iscontainer]")
       , data: o
       , onsuccess: function (data) {

           /// showMessage({ message: data, title: "Alert Message" });
           window.parent.ReloadGrid({ action: "closecreate", success: 1 });
       }
    });
   
}
function jsonEscape(str) {
    return str.replace(/\n/g, "").replace(/\r/g, "").replace(/\t/g, "").replace(" ", "");
}
