//checkboxFormatter to wire onclick event of checkbox

var zworkstatusID = $("#deviceId").val();


function afterLoad(o) {

};

function DeleteDevice(elem) {
    $.post('/device/ExecDeleteSubDevice', { "id": zworkstatusID }, function (data) {
        window.parent.ReloadGrid({ action: "closecreate", success: 1,message:data.test });
    });
}