function GetPageSecurity() {
}
$(document).ready(function () {
    $(".username").focus(function () {
        $(".user-icon").css("left", "-48px");
    });
    $(".username").blur(function () {
        $(".user-icon").css("left", "0px");
    });

    $(".password").focus(function () {
        $(".pass-icon").css("left", "-48px");
    });
    $(".password").blur(function () {
        $(".pass-icon").css("left", "0px");
    });
    // for(var p in window.location)alert(p+"====="+window.location[p])





});

function Login(elem) {
   
     $(elem).attr("disabled", "disabled")
     $("#errorMsg").html("");
    
    var userName = $("#userName").val();
    var password = $("#password").val();
    if ((userName == "اسم المستخدم") || (userName == "") || (password == "")) {
        $("#errorMsg").append("Please Enter User name and password")
        $(elem).removeAttr("disabled")
    }
    else {
        var o = [];
        o["userName"] = userName
        o["password"] = password
       // alert("Log OK>>>>");
        //Here Should Debug Showloader method
       // showLoader({ container: $("[iscontainer]") })
       
        //alert(getValidUrl({url:"/Account/Login"}))
         $.ajax({
            url: getValidUrl({url:"/Account/Login"}),
            data: { "userName": userName, "password": password },
            type: "POST",
            error: function () {
              
                $(elem).removeAttr("disabled")
                hideLoader()
                showMessage({ message: "Requst Error", titel: "Error Message" });
            },
            success: function (data, textStatus) {
               
                $(elem).removeAttr("disabled")
              //  alert(JSON.stringify(data.PageError));
              //  return;
               // if (data.PageError) { showMessage({ message: data.PageError, titel: "رسالة خطأ" }); return };
               // if (data.Error) { showMessage({ message: data.Error, titel: "رسالة خطأ" }); return };
                hideLoader()
                if (data.PageError) { showMessage({  container: $("[iscontainer]") ,message: data.PageError, titel: "Error Message" }); return };
                if (data.Error) { showMessage({  container: $("[iscontainer]") ,message: data.Error, titel: "Error Message" }); return };
                if (data.Success) {
                    try {
                      
                        if ($.getUrlVar("returnto", "") && AppInfo && AppInfo.AppPath) {
                            // alert(decodeURIComponent($.getUrlVar("ReturnUrl", "")))
                          
                            var url = decodeURIComponent($.getUrlVar("returnto", ""))
                            url_redirect({ url: url, method: "post" })
                          
                            return
                            url = url.replace(AppInfo.AppPath, "")
                            if(url.indexOf("/")!=0) url="/"+url
                           // if (url) {
                            // url = "~" + url;
                           /* alert(" window.location.origin==" + window.location.origin)
                            alert("AppInfo.AppPath====" + AppInfo.AppPath)
                            alert("url==="+url)*/
                            document.location = window.location.origin + (AppInfo.AppPath=="/"?"":AppInfo.AppPath)  + url
                           // alert(newurl)
                                // alert(url)
                                //url_redirect({ url: url, method: "post" })
                                return
                           // }
                        }
                    } catch (c) { }
                  
                    url_redirect({url:"/Home/Index",method:"post"})
                   // window.location.href = "/Home/Index";
                }
                else {
                    $("#errorMsg").html("");
                    $("#errorMsg").append("unvalid user name or password")
                    $(elem).removeAttr("disabled")
                }
            }

        })
    }
}