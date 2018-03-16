
var loanID = null;
var Record = null;
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
    
    //$("#GRNTENDDAT").datepicker();
    $("#TrackAction").select2(
       {
           placeholder: "Track Action",
           minimumInputLength: 0,
           multiple: false,
           maximumSelectionSize: 5,
           ajax: {
               url: '/TracksManagement/getTrackAction',
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
        url: "/TracksManagement/GetClientLoanInfoByID"
       , onsuccess: function (data) {
           //  alert(JSON.stringify(data));
           Record = data;
           fillEditViewdata({ container: $("[iscontainer]"), data: data })
         //  RecordData = data.rows[0];
           // personID = RecordData["PERSONID"];
          
       }
    })

    // initialPermGrid();
    
};

function saveTrack(elem) {
  

    //Set inserted vlaues

  
    var o = getDataObject({ container: $("[iscontainer]") });
    var Track = {};
    Track["ClientLoanID"] = loanID;
    Track["Tracking_Action"] = $("#TrackAction").select2("val");
    Track["Tracking_Details"] = o.TrackNote;
    Track["Tracking_USER"] = $("#User_Declared").select2("val");
    Track["Admin_Notes"] = o.Admin_Notes;
    Track["Admin_Declare"] = 1;
    Track["Status"] = 0;
  



    SaveData({
        url: '/TracksManagement/InsertTrack'
        , container: $("[iscontainer]")
       , data: Track
       , onsuccess: function (data) {

           window.parent.ReloadGrid({ action: "closecreate", success: 1 });
       }
    })
    // Call Create action method
  


}

