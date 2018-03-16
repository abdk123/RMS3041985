


function initialAutoComplete2() {
  
    
}

var ID = null;
var RecordData = null;
function afterLoad(o) {
   
    ID = $.getUrlVar("id", "")

    getData({
        data: { uID: ID },
        url: "/UserManagement/GetUserByID"
     , onsuccess: function (data) {
         debugger;
         RecordData = data;
     }
    })

    debugger;
    $("#REPWD").focusout(checkPWD);
    
};
function checkPWD() {
    // repwdcont
    var pwd = $("#PWD").val();
    var repwd = $("#REPWD").val();
   
    if (pwd != repwd) {

        $("#REPWD").css('border', '1px solid red');
        $("[errmsg]").css("visibility", "visible");
        // $("[savebtn]").prop('disabled', false);
        //$("[savebtn]").attr('disabled', 'disabled');
        //alert($("[savebtn]").id);
    } else {
        $("#REPWD").removeAttr("style");
        $("[errmsg]").css("visibility", "hidden");
        //$("[savebtn]").removeAttr('disabled');
    }
}

function Save(elem) {

    var container = $(elem).closest("[iscontainer]");
    var o = getDataObject({ container: container, data: RecordData })

    // alert(JSON.stringify(o));
    //alert(personID);
    //here get data
    RecordData.password = o.Pass;
    RecordData.UserID = RecordData.id;


    //$.ajax({
    //    url: '/UserManagement/UpdateUser',
    //    type: "POST",
    //    dataType: "json",
    //    data: RecordData,
    //    success: function (data) {
    //        alert("S");
           
    //    },
    //    error: function (x, y, z) {
    //        alert("e");
    //    }
    //});

    SaveData({
        url: '/UserManagement/ChangePassword'
        , container: container
         , data: RecordData
         , onsuccess: function (data) {
             var url = window.location.href;
             window.open(url, "_self");
             $('[iscreateroledlog]').dialog("close");
         }
    });

}



