

function initialAutoComplete2() {
    $("#Ro").select2(
       {
           placeholder: "Employee",
           minimumInputLength: 0,
           multiple: false,
           maximumSelectionSize: 5,
           ajax: {
               url: '/UserManagement/getRoTrack',
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
       }).on('select2-selecting', function (e) {
         //  alert(JSON.stringify(e))
             $("[roreport]").attr("href", "/Report/GetTrackingForROReport" + "/?ROCode="+e.object.id);
             $("[roreport]").attr("target", "_blank");
       });
      

    //$("#GRNTENDDAT").datepicker();
  


}


function afterLoad(o) {
  
    initialAutoComplete2();
  //  addbtns();
  //  $('#tab-container').easytabs({ uiTabs: true });
}










