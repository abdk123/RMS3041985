//checkboxFormatter to wire onclick event of checkbox

var userID = null;
function initialAutoComplete2() {
    //get goups
   $("#Sys_Dept").select2(
      {
          placeholder: "Chose Department ",
          minimumInputLength: 0,
          multiple: false,
          maximumSelectionSize: 5,
          ajax: {
              url: '/UserManagement/getDeptsForUser',
              dataType: 'json',
              data: function (term, page) {
               
                  return {
                      searchTerm: term,
                      page_limit: 10,
                      page: page,
                  };
              },
              results: function (data, page) {
                //  alert(JSON.stringify(data))
                  var more = (page * 10) < data.total;
                  return { results: data, more: more };
              }
          }
      });

  
    $("#Typ").select2(
     {
         placeholder: "Chose User Type ",
         minimumInputLength: 0,
         multiple: false,
         maximumSelectionSize: 5,
         data: [
        { id: 1, text: "Administrator" },
        { id: 2, text: "Supervisior" },
        { id: 3, text: "RO" },
        { id: 4, text: "Legal" }

         ]


     });


}


function afterLoad(o) {
    userID = $.getUrlVar("id", "")
  
    // alert(loanID);
    getData({
        data: { uID: userID },
        url: "/UserManagement/GetUserByID"
       , onsuccess: function (data) {
           //  alert(JSON.stringify(data));
           Record = data;
           userID = data.id;
          // data.Dept = 2;
           //   alert(JSON.stringify(data));
        
           fillEditViewdata({ container: $("[iscontainer]"), data: data })
           //  RecordData = data.rows[0];
           // personID = RecordData["PERSONID"];
         
       }
    })
    initialAutoComplete2();
  
   
};

function CheckIfUserNameExit(userName) {
    var Exit = null;
    getData({
        data: { userName: userName },
        url: "/UserManagement/CheckIfUsrNameExist"
      , onsuccess: function (data) {

          Exit = data.Message;
          return Exit;
      }
    })
}

function updateUser(elem) {
    var o = getDataObject({ container: $("[iscontainer]") });
  
    var ValidUserName = CheckIfUserNameExit(o.user_name);
    if (ValidUserName === 'OK') {
        showMessage({ message: "Invalid User Name", title: "Error" })
        $("[bindto='user_name']").focus();
    } else {
        o["Dept"] = $("#Sys_Dept").select2("val");

        o["IsDeptAdmin"] = $('#IsDeptAdmin').attr('checked') ? "True" : "False";
        o["Activate"] = $('#Activate').attr('checked') ? "True" : "False";
        //alert($.param(o));

        //  o["GNDR"] = $('input[name=sex]:checked').val();
        
        o["UserID"] = Record.id;
       
        SaveData({
            url: '/UserManagement/UpdateUser'
              , container: $("[iscontainer]")
      , data: o
      , onsuccess: function (data) {
        
          /// showMessage({ message: data, title: "Alert Message" });
          window.parent.ReloadGrid({ action: "closecreate", success: 1 });
      }
        });

        //  o["PrivLages"]= $("#multiselectDDL").select2("val").toString();
        //o["PRNTDDL"] = $("#PRNTDDL").select2("val");


    }
}
function jsonEscape(str) {
    return str.replace(/\n/g, "").replace(/\r/g, "").replace(/\t/g, "").replace(" ", "");
}
function addPerm(per) {
    var selPerm = $("#multiselectDDL").select2("val");
    var arrChecked = new Array();
    var hiddenField = $("#PermColl");
    var checkedValues = $("#ActionList li");
    var allPerm = [];


    //get checked actions
    for (var i = 0; i < checkedValues.length; i++) {
        // alert(checkedValues[i].children[0].getAttribute("value"));
        var it = checkedValues[i].children[0];
        // alert("Check box is : "+it.checked);
        if (it.checked) {
            arrChecked.push(it.getAttribute("id"));
            // alert("is inserted : "+it.getAttribute("id"));
        }
    }
    if (arrChecked.length > 0) {

        MyPerm.Permsions.push({ "id": selPerm, "text": arrChecked.join(",") });
    }

    if (hiddenField.val() === "") {
        var arr = { "Permsions": [{ "id": selPerm, "text": arrChecked.join(",") }] };
        hiddenField.val(JSON.stringify(arr));
    } else {
        var valInput = hiddenField.val();
        var obj = JSON.parse(valInput);
        obj['Permsions'].push({ "id": selPerm, "text": arrChecked.join(",") });
        hiddenField.val(JSON.stringify(obj));
    }

    $("#permGrid").trigger('reloadGrid');


}

