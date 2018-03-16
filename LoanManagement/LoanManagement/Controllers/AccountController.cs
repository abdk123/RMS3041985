using System.Web;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.IO;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System;
using System.Configuration;
using System.Text;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.Web.Security;
using System.Web;
using System.Linq;
using LoanManagement.Models;
using LoanManagement.Models.Security;

namespace LoanManagement.Controllers
{


    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public virtual ActionResult Login(string returnUrl)
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View("Login");
        }
        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult Login(FormCollection collection)
        {
            try
            {
                var res = LoginAndAuthenticate(collection.Get("userName"), collection.Get("password"));
                if (res.HasError)
                {
                    return Json(new
                    {
                        Fail = 1
                        ,
                        Error = res.Error
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new
                {
                    Success = 1
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    Fail = 1
                    ,
                    Error = exc.Message
                });
            }
        }
         [HttpPost]
        public virtual ActionResult GetUserInfo()
        {
            try
            {
                return Json(TGSession.ClientInfo);
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    Fail = 1
                    ,
                    Error = exc.Message
                });
            }
        }
        
        public static LoginResult LoginAndAuthenticate(string username, string password)
        {
            LoginResult res = new LoginResult();
            try
            {
                LoanDataDataContext database=new LoanDataDataContext();
                var usr = (from u in database.Sys_Users where u.user_name == username select u).FirstOrDefault();
                
                if (usr== null || (usr != null && password != RmsEncryption.Decrypt(usr.password)))
                {
                    res.Error = "Invalid  User Name or Password";
                   // res.HasError = true;
                    return res;
                }
                else if (usr != null && usr.Activate == false)
                {
                    
                    res.Error = "User not activated";
                    return res;
                }

                UserInfo info = new UserInfo();
                info.UserName = username;
                info.UID = usr.id.ToString();
                info.UserRole = usr.Typ.ToString();
                info.IsHeadEmp = usr.IsDeptAdmin.ToString();
                info.ROCode = usr.RO_Code;
                TGSession.ClientInfo = info;
              
                FormsAuthentication.SetAuthCookie(info.UserName, false);
            
            }
            catch (Exception exc)
            {
                res.Error = exc.Message;
            }
            return res;
        }
        [HttpPost]
        [AllowAnonymous]
        public virtual void LogOut()
        {
            ClearSessLog();
        }
        public static void ClearSessLog()
        {
            System.Web.HttpContext.Current.Session.RemoveAll();
            System.Web.HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();
            TemplateController.RedirectToLoginPage();
        }

      

    }


    /// هذا الصف يستخدم لمعلومات المستخدم الحالي للنظام.
    /// </summary>
    /// <remarks>
    /// Add more details here.
    /// </remarks>
    [Serializable()]
    public class UserInfo
    {
        /// <summary>
        /// اسم المستخدم.
        /// </summary>
        public string UserName { get; set; }
        public string ReqInfo { get; set; }
        public string Token { get; set; }
        public string UID { get; set; }
        public string UserRole { get; set; }
        public string IsHeadEmp { get; set; }
        public string ROCode { get; set; }

    }
    public class LoginResult
    {
        public bool HasError { get { return !String.IsNullOrEmpty(Error); } }
        public string Error { get; set; }
    }
    public class TGSession
    {
        # region Private constants
        // here to put uniqe string that represent entries in the session
        private const string clientInfoStr = "TGSessionUserInformation";
        private const string DSUserLoginInfoStr = "__DSUserInfo__";
        private const string NavStr = "__NAV__";

        #endregion

        #region Session Mangment
        internal static bool IsSessionValid
        {
            get
            {
                return (HttpContext.Current != null) && (HttpContext.Current.Session != null);
            }
        }

        public static void ClearSession()
        {
            if (IsSessionValid)
            {
                HttpContext.Current.Session.Abandon();
            }
        }
        #endregion

        #region Flags
        // just a flag to indicate if the data existe at the session
        public static bool DSUserLoginInfoExisted
        {
            get
            {
                if (!IsSessionValid)
                {
                    return false;
                }
                string key = System.Configuration.ConfigurationManager.AppSettings["DbName"];
                if (HttpContext.Current.Session[key + "__" + DSUserLoginInfoStr] != null)
                {
                    return true;
                }
                else
                {
                    if (clientInfoExisted)
                    {
                    //    var res = GetAppInfo();
                      //  HttpContext.Current.Session[key + "__" + DSUserLoginInfoStr] = res;
                        //if (res != null)
                            return true;
                    }
                    return false;
                }
            }
        }

        public static bool clientInfoExisted
        {
            get
            {
                if (!IsSessionValid)
                {
                    return false;
                }
                if (HttpContext.Current.Session[clientInfoStr] != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool NavExisted
        {
            get
            {
                if (!IsSessionValid)
                {
                    return false;
                }
                string key = System.Configuration.ConfigurationManager.AppSettings["DbName"] + "__" + NavStr;
                if (HttpContext.Current.Session[key] != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region Public properties
        public static UserInfo ClientInfo
        {
            set
            {
                if (IsSessionValid)
                {
                    /* string ReqInfo = "Ip:[{0}],User Agent:[{1}],Browser:{2},Version:{3},Domain:[{4}],Port:[{5}],SoftWare:[{6}],Dns:[{7}]";
                     ReqInfo = String.Format(ReqInfo, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                         , HttpContext.Current.Request.ServerVariables["http_user_agent"]
                          , HttpContext.Current.Request.Browser.Browser 
                           , HttpContext.Current.Request.Browser.Version 
                         , HttpContext.Current.Request.ServerVariables["server_name"]
                         , HttpContext.Current.Request.ServerVariables["server_port"]
                         , HttpContext.Current.Request.ServerVariables["server_software"]
                         , HttpContext.Current.Request.ServerVariables["REMOTE_HOST"]);
                     value.ReqInfo =ReqInfo;*/
                    HttpContext.Current.Session[clientInfoStr] = value;
                }
            }
            get
            {
                if (clientInfoExisted == false)
                {

                    //throw new Exception("LogIn:" + "Please Login");
                }
                return (UserInfo)HttpContext.Current.Session[clientInfoStr];
            }
        }
        public static DataSet DSUserLoginInfo
        {
            set
            {
                if (IsSessionValid)
                {
                    string key = System.Configuration.ConfigurationManager.AppSettings["DbName"];

                    HttpContext.Current.Session[key + "__" + DSUserLoginInfoStr] = value;
                }
            }
            get
            {
                string key = System.Configuration.ConfigurationManager.AppSettings["DbName"];
                if (DSUserLoginInfoExisted == false)
                {

                    if (clientInfoExisted)
                    {
                        //var res = GetAppInfo();

                        //HttpContext.Current.Session[key + "__" + DSUserLoginInfoStr] = res;
                        //return res;
                    }
                }

                return (DataSet)HttpContext.Current.Session[key + "__" + DSUserLoginInfoStr];
            }
        }
       
        #endregion
       


     
    }
}