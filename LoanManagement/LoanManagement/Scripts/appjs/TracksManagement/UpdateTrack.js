
var loanID = null;
var Record = null;
var TracksID = null;
function initialAutoComplete2() {
    $("#User_Declared").select2(
       {
           placeholder: "Employee",
           minimumInputLength: 0,
           multiple: false,
           maximumSelectionSize: 5,
           ajax: {
               url: '/UserManagement/getEmpdataToTrack',
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
    initialAutoComplete2();
    loanID = $.getUrlVar("id", "")
   
    // alert(loanID);
    getData({
        data: { tID: loanID },
        url: "/TracksManagement/GetTrackByLoanID"
       , onsuccess: function (data) {
           //  alert(JSON.stringify(data));
           Record = data;
           TracksID = data.ID;
           fillEditViewdata({ container: $("[iscontainer]"), data: data })
           //  RecordData = data.rows[0];
           // personID = RecordData["PERSONID"];

       }
    })

    // initialPermGrid();

};
//saveTrack
function ChageTrack(elem) {


    //Set inserted vlaues


    var o = getDataObject({ container: $("[iscontainer]") });
    var Tracker = {};
    Tracker["TrackID"] = TracksID;
    Tracker["Admin_Notes"] = o.Admin_Notes;
  
    Tracker["Tracking_USER"] = $("#User_Declared").select2("val");;
    //  Track["Tracking_Date"] = loanID;
    Tracker["Admin_Declare"] = 1;
   




    SaveData({
        url: '/TracksManagement/UpdateUserTracker'
        , container: $("[iscontainer]")
       , data: Tracker
       , onsuccess: function (data) {
         //  alert(JSON.stringify(data));
           window.parent.ReloadGrid({ action: "closecreate", success: 1 });
       }
    })
    // Call Create action method



}

