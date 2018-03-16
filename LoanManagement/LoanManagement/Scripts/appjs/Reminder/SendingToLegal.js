
var loanID = null;
var Record = null;
function initialAutoComplete2() {
    $("#legal").select2(
      {
          placeholder: "Select Legal",
          minimumInputLength: 0,
          multiple: false,
          maximumSelectionSize: 5,
          ajax: {
              url: '/FollowUp/getLegal',
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
    $("#papers").select2(
   {
       placeholder: "Chose Papers to legal ",
       minimumInputLength: 0,
       multiple: true,
       maximumSelectionSize: 5,
       data: [
      { id: 1, text: "بيانات ارصدة" },
      { id: 2, text: "عقد عام" },
      { id: 3, text: "عقد القرض" },
      { id: 4, text: "سندات" },
       { id: 5, text: "سند كفالة" },
         { id: 6, text: "others" }

       ]


   });


}



function afterLoad(o) {
  //  initialAutoComplete2();
    loanID = $.getUrlVar("id", "")
    // $("#ID").val(loanID);
  
    initialAutoComplete2();
};


function ReloadGrid(options) {
    var action = options.action
    //switch (action) {
    //    case "closecreate":
    //        $("[iscreateroledlog]").dialog("close");
    //        //if (options.success)
    //        //    ShowNotificationMsg({ msg: "Operation Succ" });
    //        break;
    //}
    window.parent.ReloadGrid({ action: "closecreate", success: 1 });
   // MyGrid.setGridParam().trigger('reloadGrid');
}

function savetData(elem)
{
    var o = getDataObject({ container: $("[iscontainer]") });
    var papers = $("#papers").select2("data");
   // alert(JSON.stringify(papers));
    //return;
    o["papers"] = JSON.stringify(papers);
  //  o["legalID"] = $("#legal").select2("val");
    var d = $("#legal").select2("data");
    o["Legal_Action"] = d.text;
    var client = o;
   // alert( $("#ID").val());
    client["TrakID"] = loanID;
  
    SaveData({
        url: '/FollowUp/SendDataToLegal'
       , container: $("[iscontainer]")
      , data: client
      , onsuccess: function (data) {

          showMessage({ message: data.Message, title: "Alert Message" });
         ReloadGrid({ action: "closecreate", success: 1 });
      }
    })
    


}

