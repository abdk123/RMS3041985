
var ACCID = null;
var Record = null;




function afterLoad(o) {
    //  initialAutoComplete2();
   
    ACCID = $.getUrlVar("ACCID", "")
   
    getData({
        data: { ACCID: ACCID },
        url: "/FollowUp/GetGuruantees"
      , onsuccess: function (data1) {

          if (data1) {
              $("[guranrowitem='true']").detach();
              $("[guruanteeData='true']").after(data1.htdata);

          }
          // alert(JSON.stringify(data))


      }
    })
   
    
};






