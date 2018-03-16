using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
namespace LoanManagement.Controllers
{
    public class TemplateController : Controller
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (!TGSession.clientInfoExisted)
            {
                // RedirectToAction("login", "account");
                string queryString = requestContext.HttpContext.Request.Url.PathAndQuery;
                if (queryString.Contains("returnto="))
                    queryString = queryString.Substring(queryString.IndexOf("returnto="));
                else
                    queryString = "returnto=" + HttpContext.Server.UrlEncode(requestContext.HttpContext.Request.Url.PathAndQuery);

                RedirectToLoginPage(queryString);
                // requestContext.HttpContext.Response.Redirect("~/account/login/?returnto=" + HttpContext.Server.UrlEncode(requestContext.HttpContext.Request.Url.PathAndQuery));
            }
            else
            {
                var controlerName = requestContext.RouteData.Values["controller"].ToString().ToLower();
                if ((controlerName == "Dept".ToLower() || controlerName == "ImportData".ToLower() || controlerName == "Indexs".ToLower() || controlerName == "Tracks".ToLower() || controlerName == "TrackLoans".ToLower() || controlerName == "Report".ToLower() || controlerName == "UserManagement".ToLower()) && (TGSession.ClientInfo.UserRole != "1" && TGSession.ClientInfo.UserRole != "5"))
                {
                    if(requestContext.RouteData.Values["action"].ToString() != "ChangePWD" && requestContext.RouteData.Values["action"].ToString() != "GetUserByID")
                        System.Web.HttpContext.Current.Response.Redirect("~/Home", true);
                }
                else if ((requestContext.RouteData.Values["controller"].ToString() == "FollowUp" || requestContext.RouteData.Values["controller"].ToString() == "Reminder" || requestContext.RouteData.Values["controller"].ToString() == "SendToLegalDept" || requestContext.RouteData.Values["controller"].ToString() == "ClientManagement" || requestContext.RouteData.Values["controller"].ToString() == "DeptLoans") && (TGSession.ClientInfo.UserRole != "2" && TGSession.ClientInfo.UserRole != "3" && TGSession.ClientInfo.UserRole != "5"))
                {
                    System.Web.HttpContext.Current.Response.Redirect("~/Home", true);
                }
                else if ((requestContext.RouteData.Values["controller"].ToString() == "Legal") && (TGSession.ClientInfo.UserRole != "4" && TGSession.ClientInfo.UserRole != "5"))
                {
                     System.Web.HttpContext.Current.Response.Redirect("~/Home", true);
                }
                //else if (requestContext.RouteData.Values["controller"].ToString() == "UserManagement" && (TGSession.ClientInfo.UserRole != "3" || TGSession.ClientInfo.UserRole != "5"))
                //{
                //    if (requestContext.RouteData.Values["action"].ToString() != "ChangePWD" && requestContext.RouteData.Values["action"].ToString() != "GetUserByID")
                //        System.Web.HttpContext.Current.Response.Redirect("~/Home", true);
                //}
            }
            //Do Stuff
        }
        protected override void OnResultExecuting(ResultExecutingContext e)
        {

            base.OnResultExecuting(e);
            //System.Globalization.CultureInfo curInfo = new System.Globalization.CultureInfo("ar-SY", true);
            //e.RouteData.Values.Add("PageName", String.IsNullOrEmpty(PageName) ? "" : PageName);
            //e.RouteData.Values.Add("AppName", System.Configuration.ConfigurationManager.AppSettings["DbName"]);
            //e.RouteData.Values.Add("ServerDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm ss", curInfo));
            //e.RouteData.Values.Add("ServerTicks", DateTime.Now.Ticks.ToString());
            //var ShowMonitoring = Session["__ShowAudit__"] == null || Session["__ShowAudit__"].ToString() != "1" ? 0 : 1;
            //var jsonObj = new { ShowMonitoring = ShowMonitoring };
            //JavaScriptSerializer ser = new JavaScriptSerializer();
            //e.RouteData.Values.Add("ExtraData", ser.Serialize(jsonObj));
            // e.HttpContext.Response.Write("<script>ShowMonitoring=true</script>");
        }
      
        public static void RedirectToLoginPage(string QueryString = "")
        {
            if (System.Web.HttpContext.Current.Response.StatusDescription != "OK") return;
            string LoginUrl = FormsAuthentication.LoginUrl;
            string url = LoginUrl;
            if (!String.IsNullOrEmpty(QueryString))
            {
                if (QueryString.StartsWith("?"))
                    url += QueryString;
                else
                    url += "?" + QueryString;
            }
            System.Web.HttpContext.Current.Response.Redirect(url, true);

        }
     

    }
}
