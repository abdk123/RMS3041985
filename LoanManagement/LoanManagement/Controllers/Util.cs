using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using LoanManagement.Models;
using System.IO;
namespace LoanManagement.Controllers
{
    public class Util
    {
        public static string GetNavigation() 
        {
            string menu = string.Empty;
            UserInfo user = TGSession.ClientInfo;
            LoanDataDataContext database=new LoanDataDataContext();
            List<Setting> menuItems = new List<Setting>();
            if (user!=null)
            {
                if (user.UserRole=="1")
                {
                    var uitems = from i in database.Settings where i.keyset == "admin" select i;
                    menuItems.AddRange(uitems.ToList());
                }
                else if (user.UserRole=="2" || user.UserRole=="3")
                {
                    var uitems = from i in database.Settings where i.keyset == "emp" select i;
                    menuItems.AddRange(uitems.ToList());
                }
                else if (user.UserRole=="4" )
                {
                     var uitems = from i in database.Settings where i.keyset == "legal" select i;
                    menuItems.AddRange(uitems.ToList());
                }
                else if (user.UserRole=="5")
                {
                     var uitems = from i in database.Settings  select i;
                    menuItems.AddRange(uitems.ToList());
                }
                foreach (var item in menuItems)
                {
                    menu += item.valueset;
                }
            }




            return menu;
        }
        public static string GetLegalStatus(int? state) 
        {
            string status = string.Empty;
            if (state.HasValue)
            {
                switch (state)
                {
                    case 1: status = "In Follow"; break;
                    case 2: status = "Pending To Legal"; break;
                    case 3: status = "Recived from Legal"; break;
                    case 4: status = "Returned from legal"; break;

                }
            }
            else
            {
                status = "In Follow";
            }
           
            return status;
        }
        /**
         * <div class="panel panel-primary" labelGuarancontainer="true">
                          <div class="panel-heading">Lawsuit Procedure</div>
                          <div class="panel-content">
                             <table class="table table-bordered table-striped">
                                 <tr procedureData="true">
                                     <th>
                                         Procedure
                                     </th>
                                   
                                     <th>
                                       Description
                                     </th>
                                    
                                 
                                    

                                 </tr>
                                @* <tr >
                                       <td>
                                         <span type="text" bindto="Gurantee_Type" id="Gurantee_Type" ></span>
                                     </td>
                                      <td>
                                         <span type="text" bindto="Applicant_Name" id="Applicant_Name" ></span>
                                     </td>

                                  
                                 </tr>*@
                                  
                             </table>

                          </div>
                 </div>
         * */
        public static string GetRoForms() 
        {
            string htmlTable = @"<div class='panel panel-primary' >
                          <div class='panel-heading'>RO Forms</div>
                          <div class='panel-content'>
                             <table class='table table-bordered table-striped'>
                                 <tr>
                                     <th>
                                        Documents Name
                                     </th>

                                 </tr>";

            string UploadPath = HttpContext.Current.Server.MapPath("~/FileUploaded/ROForms");
            string rtemplate = @"<tr>
                                              <td>
                                                 <a href='/FollowUp/Download?file={path}' target='_blank'>{Name}</a> 
                                              </td>
                                          </tr>";
            foreach (string fileName in Directory.GetFiles(UploadPath))
            {
                string path = fileName;
                string extension = Path.GetExtension(fileName);
                string fn = Path.GetFileName(fileName).Split('.')[0];

                string r = rtemplate.Replace("{path}",path);
                r = r.Replace("{Name}",fn);
                htmlTable += r;

            }
            htmlTable += "</table></div></div>";
            return htmlTable;
        }
        
       

    }
}