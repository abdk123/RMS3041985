//checkboxFormatter to wire onclick event of checkbox


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
    initialAutoComplete2();
   
};



function saveAction(elem) {
    var o = getDataObject({ container: $("[iscontainer]") });
    o["ActionType"] = $("#Typ").select2("val");
    SaveData({
        url: '/Indexs/CreateAction'
             , container: $("[iscontainer]")
     , data: o
     , onsuccess: function (data) {

         //  showMessage({ message: data, title: "Alert Message" });
         window.parent.ReloadGrid({ action: "closecreate", success: 1 });
     }
    });
   
}
function jsonEscape(str) {
    return str.replace(/\n/g, "").replace(/\r/g, "").replace(/\t/g, "").replace(" ", "");
}
