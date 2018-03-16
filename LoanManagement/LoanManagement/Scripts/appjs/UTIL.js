var Resources = {
    AddSuccess: "تمت عملية الإضافة بنجاح"
    , EditSuccess: "تمت عماية التعديل بنجاح"
     , DeleteSuccess: "تمت عماية الحذف بنجاح"
}

var BaseJQgrid = {
    rowNum:10
    ,rowList:[1,2,3,4,5,10, 20, 30, 40]
    ,jsonReader: {
        root: "rows",
        page: "page",
        total: "total",
        records: "records",
        repeatitems: false,
        Id: "0"
    }
    , emptyrecords: "لا يوجد بيانات لعرضها"
    , loadError: function (jqXHR, textStatus, errorThrown) {
        alert('HTTP status code: ' + jqXHR.status + '\n' +
              'textStatus: ' + textStatus + '\n' +
              'errorThrown: ' + errorThrown);
        alert('HTTP message body (jqXHR.responseText): ' + '\n' + jqXHR.responseText);
    }
    , loadComplete: function (data) {
        if (data.Error)
            showMessage({ message: data.Error, title: "رسالة خطأ" })
    }
}
var UserInfo = {};
$.fn.tlJqGrid = function (options) {
   return $(this).jqGrid(options);
}
$.wait = function (callback, seconds) {
    return window.setTimeout(callback, seconds * 1000);
}
$.extend({
    getUrlVars: function () {
        var vars = [], hash;
        var jikovars = {}
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
            jikovars[hash[0]] = hash[1]
        }
        return jikovars;
    },
    getUrlVar: function (name, undefinedvalue) {
        var res = $.getUrlVars()[name]
        if (res == undefined && typeof (undefinedvalue) != "undefined") {
            res = undefinedvalue;
        }
        return res;
    }
});
function getRandom() {
    var d = new Date();

    return d.getTime() + "-" + Math.random(); //  encodeURIComponent( d.toUTCString())
}
function openDialog(options) {
    var height = $(window).innerHeight()
    var width = 830;
    if (options.width) width = options.width;
    if (options.height) height = options.height;
    var url = "";
    var title = "Entering Form";
    if (typeof (options) == "string")
        url = options
    else {
        url = options.url;
        title = options.title ? options.title : title;
    }
    if (url.indexOf('?') > 0) url += "&nocach=" + getRandom();
    else url += "?nocach=" + getRandom()
    var dvdlog = $("<div isdvdlog  ></div>").appendTo($("body"))
    if (options.extraattr) {
        for (var p in options.extraattr) {
            dvdlog.attr(p,options.extraattr[p])
        }
    }
    var frame = $("<iframe onload=\"resizeDlog({f:this,dlg:'[isdvdlog]',diff:30});\" >").attr({
        "src": url
                   , "width": (width-50)+ "px"
                   , "height":(height-20) +"px"
                   , "frameborder": ""
    });
    dvdlog.append(frame);
   
  
    dvdlog.dialog({
        autoOpen: false, width: width, height: height, title: title, modal: true
            //     , position: 'center'
          , position: options.position ? options.position : 'center'
         , close: function () {
             try {
                 $(this).remove()
             } catch (exc) { alert(exc.message) }
         }
    });
    dvdlog.dialog("open")

}
function resizeDlog(o) {
    //$(this.contentWindow.document).width()

    var frame = o.f;
    var dlog = $(parent.document).find(o.dlg);
    var w = $(frame.contentWindow.document).width()
    var h = $(frame.contentWindow.document).height()
    var maxwidth = $(parent.window).innerWidth() - 80
    var maxheight = $(parent.window).innerHeight() - 100
    if (w > maxwidth) w = maxwidth;
    if (h > maxheight) h = maxheight;
    $(frame).width(w)
    $(frame).height(h + 10)
    dlog.dialog("option", "width", w + 50)
    dlog.dialog("option", "height", h + 80)
}

function getDataObject(option) {
    var container = option.container
    var flds = container.find("[bindto]");
    var o = {}
    for (var i = 0; i < flds.length; i++) {
        o[$(flds[i]).attr("bindto")] =$.trim( $(flds[i]).val());
    }
    return o;
}
function fillEditViewdata(options) {
    var container = options.container;
    var data = options.data
    var arr = container.find("[bindto]")
   // alert(JSON.stringify(arr))
    for (var i = 0; i < arr.length; i++) {
        var elem = $(arr[i])
        var bindto = $(elem).attr("bindto");
        var elemType = getType(arr[i]);
       // alert(JSON.stringify(data[bindto]));
        if (elemType == "checkbox") {

            var checkedvalue = $(elem).attr("checkedvalue");
            var uncheckedvalue = $(elem).attr("uncheckedvalue");
            if (typeof (checkedvalue) != "undefined") {
                if (checkedvalue == data[bindto]) {
                    elem.attr("checked", true)
                }
                continue
            }
            // if (typeof (data[bindto]) != "undefined" && data[bindto] != "null" && data[bindto] != null && (data[bindto].toLowerCase() == "1" || data[bindto].toLowerCase() == "true"))
            if (typeof (data[bindto]) != "undefined" && data[bindto] != "null" && data[bindto] != null)
                elem.attr("checked", true)
            continue
        }
        if (elemType == "date") {
            if (typeof (data[bindto]) == "undefined" || data[bindto] == null || data[bindto] == "")
                elem.val("");
            else {
                //  var dt = $.jgrid.parseDate('d/m/Y', data[bindto])
              //  var dt = $.jgrid.parseDate('yy/MM/dd', data[bindto])
               // var dat = new Date(parseInt(data[bindto].substr(6)));
                var strDate = convertDate(data[bindto]);
               // alert(strDate)
                // elem.val($.datepicker.formatDate('dd/mm/yy', dt))
                elem.val(strDate)
            }
            continue
        }

        if (elemType == "label") {
            if (typeof (data[bindto]) == "undefined" || data[bindto] == null)
                elem.html("");
            else
                if (elem.attr("datatype") == "date") {
                    if (typeof (data[bindto]) == "undefined" || data[bindto] == null || data[bindto] == "")
                        elem.html("");
                    else {
                        var dt = $.jgrid.parseDate('d/m/Y', data[bindto])
                       
                        elem.html($.datepicker.formatDate('dd/mm/yy', dt))
                    }
                }
                else
                    elem.html(data[bindto]);
            continue
        }
        elem.val(data[bindto])
        if (elem.attr("isselect2") && elem.attr("textfield")) {
            elem.attr("text-option", data[elem.attr("textfield")]).trigger('change')
        }
    }
    arr = container.find("[basebinding]")
    for (var i = 0; i < arr.length; i++) {
        var elem = $(arr[i])
        var bindto = elem.attr("basebinding");
        elem.val(data[bindto])

    }
  
}

function convertDate(inputFormat) {
    function pad(s) { return (s < 10) ? '0' + s : s; }
    var d = new Date(parseInt(inputFormat.substr(6)));
    return [pad(d.getDate()), pad(d.getMonth() + 1), d.getFullYear()].join('/');
}
function getType(elem) {
    if (elem.tagName.toLowerCase() == "input") {
        var type = elem.getAttribute("type");
        type = type == undefined || type == "undefined" ? "text" : type.toLowerCase()
        switch (type) {
            case "hidden":
            case "text":
                if ($(elem).attr("datatype") == "date" || $(elem).attr('dbtype') == 'date')
                    return "date";
                return "textbox";
                break;
            case "submit":
            case "button":
                return "button";
                break;
            case "checkbox": return "checkbox";
                break;
        }
    }
    if (elem.tagName.toLowerCase() == "textarea") return "textbox";
    if (elem.tagName.toLowerCase() == "select") {
        return "list"
    }
    if (elem.tagName.toLowerCase() == "label" || elem.tagName.toLowerCase() == "span") {
        return "label"
    }
    if (elem.tagName.toLowerCase() == "a") {
        return "link"
    }
    if (elem.tagName.toLowerCase() == "button") {
        return "button"
    }


}
function getData(options) {
    var data = options.data ? options.data : {};
    var url = options.url;
    if (!url) return;
  
    if (url.indexOf('?') > 0) url += "&nocach=" + getRandom();
    else url += "?nocach=" + getRandom()
    var ResultData = null;
    var container = options.container
    if (!container && $("[iscontainer]").length > 0) container = $("[iscontainer]")
    if (!container) container = $(window)
   

    //  showLoader({ container: container })
    $.ajax({
        type: "POST",
        url: getValidUrl({ url: url }),
        data: data,
        async: false,
        dataType: "json",
        error: function (jqXHR, ajaxOptions, thrownError) {
            hideLoader()
            getAjaxError(jqXHR, ajaxOptions, thrownError)
            //showMessage({ message: getAjaxError(xhr, ajaxOptions, thrownError), titel: "رسالة خطأ" });
        },
        success: function (dataset) {
            hideLoader()

            if (dataset.PageError) { showMessage({ message: dataset.PageError, titel: "Error Message" }); return };
            if (dataset.Error) { showMessage({ message: dataset.Error, titel: "Error Message" }); return };
            ResultData = dataset
            if (options.onsuccess)
                options.onsuccess(dataset)

        }
    });
    return ResultData;
}
$.extend({
    getUrlVars: function () {
        var vars = [], hash;
        var hanivars = {}
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
            hanivars[hash[0]] = hash[1]
        }
        return hanivars;
    },
    getUrlVar: function (name, undefinedvalue) {
        var res = $.getUrlVars()[name]
        if (res == undefined && typeof (undefinedvalue) != "undefined") {
            res = undefinedvalue;
        }
        return res;
    }
});
function showLoader(options) {
    var elem = options.elem
    var container = options.container
    var message = options.message
    // return;
    //alert(Page_ValidationActive)
    //if (typeof(ValidatorOnSubmit) == "function" && ValidatorOnSubmit() == true) return false;
    var strMsg = " <div isloader  style='position:fixed;zIndex:5000;display: none'>"
    strMsg += "<div align='center' style='margin-top: 13px;'>"
    strMsg += "<img src='" + getValidUrl({ url: "jsAndStyle/images/loading.gif" }) + "' />"
    strMsg += "<span>Loading ...</span>"
    strMsg += "</div></div>"
    var defaultAddendTo = options.defaultAddendTo || $("body");
    var progress = $(strMsg).appendTo(defaultAddendTo);
    if (!container) container = $(window)
    try {
        if (!message) message = ""// $(elem).html()
        progress.find("span").html(message + "")
    } catch (c) { }

    progress.css({
        position: 'fixed',
        zIndex: 5000

    });
    progress.position({
        of: container,
        my: "center center"
        // ,at: "center center"
        // ,offset: 0

        //, collision: "none none"
    });
    progress.show()

}
function hideLoader() {
    // $("[isloader]").bPopup().close()
    $("[isloader]").remove()
}
function getAjaxError(jqXHR, ajaxOptions, thrownError) {
    //  showMessage({ message: getAjaxError(xhr), titel: "رسالة خطأ" });
    var msg = "";
    if (jqXHR.status === 0) {
        msg = "<h1>" + "Network Error" + "</h1>";
        //  msg += "<br/>" + jqXHR.responseText

    } else if (jqXHR.status == 404) {
        msg = "<h1>" + "Request Not Found" + "</h1>";
        // msg += "<br/>" + jqXHR.responseText

        // showMessage({ message: "الصفحة التي تريد طلبها غير موجودة", titel: "رسالة خطأ" });
        // return
    } else if (jqXHR.status == 500) {
        //  return('Internal Server Error [500].');
        msg = "<h1>" + "Error In Server" + "</h1>";
        msg += "<br/>" + jqXHR.responseText
        // showMessage({ message: "خطأ داخلي في المخدم الرجاء التأكد", titel: "رسالة خطأ" });

    } else if (ajaxOptions === 'parsererror') {
        // return ('Requested JSON parse failed.');
        msg = "<h1>" + "Requested JSON parse failed." + "</h1>";
        msg += "<br/>" + jqXHR.responseText

        //  showMessage({ message: 'Requested JSON parse failed.', titel: "رسالة خطأ" });
        // return
    } else if (ajaxOptions === 'timeout') {
        //return('Time out error.');
        msg = "<h1>" + "Connection Timeout." + "</h1>";
        msg += "<br/>" + jqXHR.responseText

        //showMessage({ message: 'تم انتهاء وقت الاتصال.', titel: "رسالة خطأ" });
        // return
    } else if (ajaxOptions === 'abort') {
        msg = "<h1>" + "Request Stop." + "</h1>";
        msg += "<br/>" + jqXHR.responseText

        //showMessage({ message: 'تم إيقاف الطلب.', titel: "رسالة خطأ" });
        // return
        // return('Ajax request aborted.');
    } else {
        msg = "<h1>" + "Undefined Error." + "</h1>";
        msg += "<br/>" + jqXHR.responseText

        //showMessage({ message: 'خطأ غير متوقع.', titel: "رسالة خطأ" });
        // return ('Uncaught Error.\n' + jqXHR.responseText);
        // return
    }
    var message = msg;//+ "<br/>----------" + ajaxOptions + "<br/>---------------" + thrownError
    showMessage({ message: message, titel: "Error Message" });
}
function ValidateData(option) {
    var valid = true;
    var container = option.container
   container.find("[isvalidatormsg]").remove()
    var arr = container.find("[validation]")
    for (var i = 0; i < arr.length; i++) {
        var elem=$(arr[i]);
        var validators =eval(elem.attr("validation"))
        for (var v = 0; v < validators.length; v++) {
            var vld = validators[v];
            if (vld.isreq) {
                if (!$(elem).val()) {
                    var msg =vld.msg?vld.msg: "*"
                    $("<span isvalidatormsg='true' style='color:red'>"+msg+"</span>").insertAfter(elem)
                    valid=false
                }
            }
        }
    }
    return valid
}

function IntializeUserInfo() {

   
    $.ajax({
        url: getValidUrl({ url: "/Account/GetUserInfo" }),
        data: { },
        type: "POST",
        error: function () {

         
        },
        success: function (data) {
            UserInfo = data;
           
           // return UserInfo;
        }

    })
}

function GetUserInfoData()
{
   
    return UserInfo;
}
function afterLoad(o) {
  
    
}
//Not Applied
function IntialSec()
{
    var UserInfo = GetUserInfoData();
    if (UserInfo != null) {
        var menu = $("#sidebar").find("[role]");;
        //for (var i = 0; i < menu.length; i++) {
        //    alert($(menu[i]).html())
        //}

        for (var i = 0; i < menu.length; i++) {
            if ($(menu[i]).attr("role") != UserInfo.UserRole) {
                $(menu[i]).remove();
               // alert("Removed un")
            }
        }

    }
}
function GoToURL(elem)
{
    var u = $(elem).attr("url");
   // alert(u);
    url_redirect({ url: getValidUrl({ url: u }), method: "post" })
}
$(document).ready(function () {
    // Handler for .ready() called.

    afterLoad();
   // IntializeUserInfo();
   // alert(JSON.stringify(UserInfo))
 
 //   var ss = GetUserInfoData();
   // alert(JSON.stringify(ss));
});

function url_redirect(options) {
    var $form = $("<form />")
    var dirurl = options.staticurl ? options.staticurl : getValidUrl({ url: options.url })
    $form.attr("action", dirurl)
    // alert(options.url)
    // $form.attr("action", options.url)

    $form.attr("method", options.method || "post")
    for (var p in options.data) {
        $form.append("<input type='hidden' name='" + p + "' value='" + options.data[p] + "' />")
    }
    $("body").append($form);
    $form.submit();

}
function showMessage(o) {
    var message = o.message
    var onajaxerror = o.onajaxerror
    var title = o.title
    var titleClass = o.titleClass ? o.titleClass : "ErrorDlogTitle"
    if (message.indexOf("login") >= 0) {
        var objWindow = window.open("login.aspx", "", "status=1,width=400 , height=400, scrollbars= yes  , menubar=yes , resizable = yes  , titlebar=no,menubar=no, toolbar=no");
        return;
    }
    if (onajaxerror) message = message;
    
    var MyMsgDivStr = "<div dir=rtl error-message title='Error Message' style=\"border: 3px\">"
    MyMsgDivStr += "<p>"
    // MyMsgDivStr += "<span class=\"ui-icon ui-icon-circle-check\" style=\"float: right; margin: 0 7px 50px 0;\"></span>"
    MyMsgDivStr += "<span messagetext style=\"float: right;\"></span>"
    MyMsgDivStr += "</p></div>"
    var newdv = $(MyMsgDivStr)// $("#error-message").clone(true);
    if (title) newdv.attr("title", title);
    var msgSelectedElement = document.activeElement
    newdv.find("[messagetext]").html(message)
    newdv.addClass(titleClass)
    newdv.dialog({
        modal: true,
        dialogClass: titleClass,
        close: function () {
            try {
                $(msgSelectedElement).focus()
                $(this).remove()
            } catch (c) { }
        },
        buttons: {
            OK: function () {
                $(this).dialog("close");
            }
        }
    });
}


function SaveData(options) {
    var container = options.container
    var validate = typeof (options.validate) == "undefined" ? true : options.validate
    if (validate) {
        if (!ValidateData({ container: container })) {
            showMessage({ message: "Please Check of all Fields" })
            return
        }
    }
    var url = options.url;
   if (url.indexOf('?') > 0) url += "&nocach=" + getRandom();
    else url += "?nocach=" + getRandom()
    var data = options.data
    
    showLoader({ container: container })
    $.ajax({
        type: "POST",
        url: url,
        data: data,
        async: false,
        dataType: "json",
        error: function () {
            hideLoader()
            showMessage({ message: "Request Error", titel: "Error Message" });
        },
        success: function (dataset) {
           
            hideLoader()
            if (dataset.PageError) { showMessage({ message: dataset.PageError, titel: "Error Message" }); return };
            if (dataset.Error) { showMessage({ message: dataset.Error, titel: "Error Message" }); return };
            if(options.onsuccess)
                options.onsuccess(dataset)
        }
    });

}

function DeleteData(options) {

    var url = options.url ? options.url : '/Save/DeleteData';
    var tableName = options.tableName;
    var grid = options.grid;


    var selRowId = grid.jqGrid('getGridParam', 'selrow');
    var celValue = grid.jqGrid('getCell', selRowId, 'ID');
    if (celValue != false) {

        $("<div isdeldv>هل أنت متأكد من عملية الحذف</div>").dialog({
            resizable: false,
            height: 240,
            modal: true,
            buttons: {
                "متأكد من الحذف": function () {
                    SaveData({
                        url: url
                      , data: { tableName: tableName, opName: "delete", generate: true, data: { ID: celValue } }
                       , container: myGrid
                       , onsuccess: function (data) {
                           $("[isdeldv]").dialog("close");
                           $("[isdeldv]").remove();
                        grid.delRowData(selRowId)
                        ShowNotificationMsg({ msg: Resources.DeleteSuccess });
                        }//End save onsuccess
                    })//End save data
                    
                },
                تراجع: function () {
                    $(this).dialog("close");
                    $(this).remove();
                }
            }
        });


      
    }
    else {
        showMessage({ message: "الرجاء تحديد السجل من الجدول" })
    }


}



function ShowNotificationMsg(options) {

    var msg = options.msg;
    var str = "<div class='notificationmsg'>[msg]</div>"
    str=str.replace("[msg]",msg)
    var MyMsgDivStr = $(str).appendTo($("body"))
    MyMsgDivStr.delay(400).slideDown(400).delay(3000).slideUp(400).queue(function () {
        this.remove()
    });
  
}

function AddBtns(container, options, buttons) {
    var str = '<div class="toolbar RadToolBar RadToolBar_Horizontal RadToolBar_Office2007 RadToolBar_Office2007_Horizontal RadToolBar_rtl RadToolBar_Office2007_rtl" >'
    str += '<div class="rtbOuter">'
    str += '  <div class="rtbMiddle">'
    str += '    <div class="rtbInner">'
    str += '        <ul isbtnsul="true" class="rtbUL">'
    str += '        </ul>'
    str += '</div>'
    str += '  </div>'
    str += '  </div>'
    str += '  </div>'
    var cont = $(str).appendTo(container)

    var btnsul = cont.find("[isbtnsul]")

    var btnstr = '          <li class="rtbItem rtbBtn " ><a  title="[title]" [strattr] onclick="[clickevent]"'
    btnstr += '             class=" rtbWrap" href="#"><span class="rtbOut"><span class="rtbMid"><span class="rtbIn rtbVOriented_1">'
    btnstr += '               <img alt="[title]" src="[imgurl]" class="rtbIcon"><span class="rtbText">[text]</span></span></span></span></a></li>'

    for (var i = 0; i < buttons.length; i++) {
        var b = buttons[i];
        var text = b.text;

        var reg = /\[text\]/g
        var res = btnstr
        reg = /\[text\]/g
        res = res.replace(reg, text)

        var title = b.title;
        if (!title) title = text
        reg = /\[title\]/g
        res = res.replace(reg, title)

        var clickevent = b.clickevent
        reg = /\[clickevent\]/g
        res = res.replace(reg, clickevent)

        var imgurl = b.imgurl
        reg = /\[imgurl\]/g
        res = res.replace(reg, imgurl)

        var strattr = b.strattr ? b.strattr : "";
        reg = /\[strattr\]/g
        res = res.replace(reg, strattr)

        if (i < buttons.length - 1)
            res += '<li  class=" rtbSeparator"><span class="rtbText"></span></li>'
        $(res).appendTo(btnsul)
    }


}

function fillList(options) {
   var list = options.list
    var data = options.data ? options.data : {};
   
    var selectedValue = options.selectedValue
    var url = options.url;
    if (!url) url = "/devicetype/GetDevicesType/";
    var valueField = options.valueField ? options.valueField : "ID"
    var textField = options.textField ? options.textField : "NAME"
    if (typeof (options.data.fields) == "undefined" || !options.data.fields) options.data.fields = valueField + "," + textField
    if (typeof (options.data.page) == "undefined") options.data.page = 1;
    if (typeof (options.data.rows) == "undefined") options.data.rows = -1;

    if (url.indexOf('?') > 0) url += "&nocach=" + getRandom();
    else url += "?nocach=" + getRandom()
    showLoader({ container: list })
     $.ajax({
        type: "POST",
        url: url,
        data: data,
        async: false,
        dataType: "json",
        error: function () {
            hideLoader()
            showMessage({ message: "خطأ في الطلب", titel: "رسالة خطأ" });
        },
        success: function (dataset) {
            hideLoader()
            if (dataset.PageError) { showMessage({ message: dataset.PageError, titel: "رسالة خطأ" }); return };
            if (dataset.Error) { showMessage({ message: dataset.Error, titel: "رسالة خطأ" }); return };
            if (options.onsuccess)
                options.onsuccess(dataset)
            $.each(dataset.rows, function (i, value) {
                $(list).append($('<option>').text(value[textField]).attr({ 'value': value[valueField] }));
            });
            if (selectedValue) $(list).val(selectedValue)
        }
    });

}


function showLoader(options) {
    var elem = options.elem
    var container = options.container
    var message = options.message
    //alert(Page_ValidationActive)
    //if (typeof(ValidatorOnSubmit) == "function" && ValidatorOnSubmit() == true) return false;
    var strMsg = " <div isloader  style='position:fixed;zIndex:5000;display: none'>"
    strMsg += "<div align='center' style='margin-top: 13px;'>"
    strMsg += "<img width='60px' height='60px' src='/images/loading.gif' />"
    strMsg += "<span>Loading ...</span>"
    strMsg += "</div></div>"
    var defaultAddendTo = $("body");
    var progress = $(strMsg).appendTo(defaultAddendTo);
    if (!container) container = $(window)
    try {
        if (!message) message = ""// $(elem).html()
        progress.find("span").html(message + "")
    } catch (c) { }

    progress.css({
        position: 'fixed',
        zIndex: 5000

    });
    progress.position({
        of: container,
        my: "center center"
        // ,at: "center center"
        // ,offset: 0

        //, collision: "none none"
    });
    progress.show()
    
}
function LogOff() {
    url_redirect({ url: "/Account/LogOut", method: "post" })
}
function changePWD(elem) {
    var UID = $(elem).attr("userID");
   // alert(UID);
    openDialog({
        url: "/UserManagement/ChangePWD/?id=" + UID,
        title: "Change Password",
        extraattr: { iscreateroledlog: "true" },
        width: 450,
        height: 150

    })
}
function tg_addImageButton(o) {
    var grid = o.grid;
    var RowId = o.RowId;
    var evt = o.evt;
    var CellKey = o.CellKey;
    title = o.title || ""
    src = o.src
    var attr = o.attr || ""
    var strimg = "<img src='" + getValidUrl({ url: src }) + "' " + attr + " title='" + title + "' onclick='" + evt + "' style='width:16px' data-id='" + RowId + "' />";
    $(grid).setCell(RowId, CellKey, strimg);


}

function getValidUrl(o) {
    var url = typeof (o) == "string" ? o : o.url;
    //  if (typeof (IsWebForm) != "undefined" && IsWebForm == true) return url;
  
    if (typeof (AppInfo) == "undefined" || !AppInfo.AppPath) return url;
    if (url.indexOf("/") == 0 && AppInfo.AppPath != "/") {
        if ((url + "/") == AppInfo.AppPath) url = "/"
        else
            url = url.replace(AppInfo.AppPath, "/")
    }
    //if (url.indexOf("/") == 0) return AppInfo.AppPath + url;
    if (url.indexOf(".") == 0 || url.indexOf("~") == 0) {
        var arr = url.split('/')
        var idx = url.indexOf("/")
        var dot = arr[0];
        // for(var p in window.location)alert(p+"==="+window.location[p])
        // alert(dot + AppInfo.AppPath + url.substr(idx))
        url = url.substr(idx)
        // if (url.indexOf(AppInfo.AppPath) == 0) url = url.replace(AppInfo.AppPath, "")
        // return window.location.origin + AppInfo.AppPath + url.substr(idx)
        // return dot  + AppInfo.AppPath + url.substr(idx)
    }
    if (url.indexOf(AppInfo.AppPath) == 0 && AppInfo.AppPath != "/") url = url.replace(AppInfo.AppPath, "/")
    // alert(url+"====="+url.indexOf("//"))
    if (url.indexOf("/") == 0)
        url = url.replace("/", "")
    if (!window.location.origin) {
        window.location.origin = window.location.protocol + "//" + window.location.hostname + (window.location.port ? ':' + window.location.port : '');
    }
    return window.location.origin + AppInfo.AppPath + url
}

function hideLoader() {
    // $("[isloader]").bPopup().close()
    $("[isloader]").remove()
}



function FillSrchOptnLists(list) {
    var o = {
        stringarray: [
                        ["يشابه", "{0} like '%$%'"],
                        ["يطابق", "{0} like '$'"],
                        ["يبدأ بـ", "{0} like '$%'"],
                        ["ينتهي بـ", "{0} like '%$'"],
                        ["لا يشابه", "{0} not like '%$%'"],
                        ["فارغ", "isnull({0},'') like ''"],
                        ["ليس فارغاً", "isnull({0},'') not like ''"]
        ]
          , numbeerarray: [["يساوي", "{0} = $"],
                             ["لا يساوي", "{0} <> $"],
                             ["أكبر من", "{0} > $"],
                             ["أكبر أو يساوي", "{0} >= $"],
                              ["أصغر من", "{0} < $"],
                              ["أصغر أو يساوي", "{0} <= $"],
                              ["فارغ", "{0} is null"],
                               ["ليس فارغاً", "{0} is not null"]
          ]
    };
    for (var i = 0; i < list.length; i++) {
        var lst = list[i];
        var dbtype = $(lst).attr("dbtype") ? $(lst).attr("dbtype") : "string";
        var boundfld = $(lst).attr("boundfld") ? $(lst).attr("boundfld") : $(lst).attr("id").toLowerCase().replace("lstchoose_", "");
        var lstarray = o.stringarray;
        switch (dbtype) {
            case "number":
                lstarray = o.numbeerarray;
                break;
            default:
                lstarray = o.stringarray;
                break;
        }
        for (var t = 0; t < lstarray.length; t++) {
            var op = document.createElement("option");
            op.value = lstarray[t][1].replace("{0}", boundfld);
            $(op).html(lstarray[t][0]);
            lst.appendChild(op)
        }

    }
}
var ListOfCompareText = {
    comptxt: ' <tr srch srchtype="comptxt" [defaultopen]><td colspan="2">'
                + '<table border="0" ischeckfortextfilter="true"><tr><td colspan="2">'
                + '<span><input srchdef type="checkbox" srchid="optn[name]" /><label>[caption]</label></span></td></tr>'
                + '<tr><td><select srchcomp dbtype="[dbtype]"  boundfld="[boundfld]" srchid="lstChoose_[name]"></select></td>'
                + '<td><input type="text" dbtype="[dbtype]" style="width:100%" srchtxt srchid="txt[name]" /></td></tr></table></td></tr>'
    , complst: ' <tr srch srchtype="complst" [defaultopen]><td colspan="2">'
             + '<table border="0" ischeckforlistfilter="true">'
             + '<tr><td colspan="2"><span><input srchdef type="checkbox" srchid="optn[name]" /><label>[caption]</label></span></td></tr>'
             + '<tr><td colspan="2"><select srchlst [attr]  boundfld="[boundfld]" srchid="lst_[name]"></select></td></tr></table></td></tr>'
    , compdate: ' <tr srch srchtype="compdate" [defaultopen]><td colspan="2">'
             + '<table border="0" ischeckfordatefilter="true">'
             + '<tr><td colspan="2"><span><input srchdef type="checkbox"  srchid="optn[name]" /><label>[caption]</label></span></td></tr>'
             + '<tr><td><input srchdate type="text" style="width: 70px"  boundfld="[boundfld]" srchid="dtpfrom_[name]" dbtype="date" /></td>'
              + '<td><input srchdate type="text" style="width: 70px"  boundfld="[boundfld]" srchid="dtpto_[name]" dbtype="date" /></td></tr></table></td></tr>'

}
function BuildFilter(o) {
    var container = o.container;
    var flds = o.columns;
    var expanded = o.expanded ? "true" : "false";
    var getdataafterclear = o.getdataafterclear ? "true" : "false";
    var okevt = o.okevt ? o.okevt : "Search(this)"
    var cancelevt = o.cancelevt ? o.cancelevt : "clearSearch()"
    var str = '<fieldset expanded="[expanded]" srchcontainer="true" style="[style]">'
             + '<legend title="شروط البحث" onclick="ExpanCollapseFilter(this);" srchcopllapseimg ><span>شروط البحث</span></legend>'
             + '<div onclick="ExpanCollapseFilter(this);">شروط البحث</div>'
               + '<table srchcontent="true"  border="0" style="border: 0; width:100%;min-width:150px; color: Navy; text-align: right;"><caption ></caption>';

    var br = /\[expanded\]/g
    str = str.replace(br, expanded)

    var style = o.style ? o.style : "";
    br = /\[style\]/g
    str = str.replace(br, style)

    for (var i = 0; i < flds.length; i++) {
        var fld = flds[i]
        var srchtype = fld.srchtype;
        var caption = fld.caption;
        var name = fld.name;
        var defaultopen = fld.defaultopen ? "defaultopen=true" : "";
        var attr = fld.attr ? fld.attr : "";
        var dbtype = fld.dbtype ? fld.dbtype : "string";
        var boundfld = fld.boundfld ? fld.boundfld : name;
        var r = ListOfCompareText[srchtype]
        var reg = /\[name\]/g
        r = r.replace(reg, name)

        reg = /\[boundfld\]/g
        r = r.replace(reg, boundfld)

        reg = /\[caption\]/g
        r = r.replace(reg, caption)

        reg = /\[dbtype\]/g
        r = r.replace(reg, dbtype)

        reg = /\[attr\]/g
        r = r.replace(reg, attr)

        reg = /\[defaultopen\]/g
        r = r.replace(reg, defaultopen)
        str += r;
    }

    var endstr = ' <tr><td colspan="2">'
          + '<input type="button" okserachReagion="true"  value="بحث"  onclick="[okevt];return false;" style="margin-left:10px" />'
          + '<input type="button" cancelserachReagion="true"  value="مسح" onclick="[cancelevt];return false;" style="width: 60px" />'
          + '</td></tr></table></fieldset>';
    var er = /\[okevt\]/g
    endstr = endstr.replace(er, okevt)

    er = /\[cancelevt\]/g
    endstr = endstr.replace(er, cancelevt)

    str += endstr;
    $(str).appendTo($(container));
    $(container).attr("serachReagion", "true")
    $(container).attr("getdataafterclear", getdataafterclear)

    $(container).css("padding-left", 10)
}




